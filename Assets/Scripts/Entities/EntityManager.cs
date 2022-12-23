using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;


public class EntityManager : MonoBehaviour
{
    public static EntityManager instance;
    public GameObject projectilePrefab;
    public ObjectPool<GameObject> projectilePool;

    public GameObject enemyPrefab;
    public ObjectPool<GameObject> enemyPool;



    private Dictionary<int,GameObject> currentlyActiveEnemies;  
    private List<int> EnemiesMarkedForDeath;
    

    public GameGrid gameGrid; 
    private QuadTree tree;



    // Start is called before the first frame update
    void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("GameEvents must be a singleton");
        }
        this.currentlyActiveEnemies = new Dictionary<int,GameObject>();
        this.EnemiesMarkedForDeath = new List<int>();

        Vector2Int dimensions = gameGrid.getDimensions();
        Rect quadTreeRect = new Rect(0,0, dimensions.x,dimensions.y);
        tree = new QuadTree(quadTreeRect);

        GameEvents.instance.onEnemyKilled += onEnemyKilled;

        this.projectilePool = new ObjectPool<GameObject>(() =>
        {
            return Instantiate(projectilePrefab);
        }, (entity) =>
        {
            entity.gameObject.SetActive(true);
        }, (entity) =>
        {
            entity.gameObject.SetActive(false);
        }, (entity) =>
        {
            Destroy(entity);
        });

        this.enemyPool = new ObjectPool<GameObject>(
        () => {
            GameObject gameObject = Instantiate(enemyPrefab);
            return gameObject;
        }, (entity) => {
           entity.SetActive(true);
            int instanceID = Mathf.Abs(entity.gameObject.GetInstanceID());
            if (currentlyActiveEnemies.ContainsKey(instanceID))
            {
                Debug.LogError("Already contains key.");
            }
            currentlyActiveEnemies.Add(entity.gameObject.GetInstanceID(), entity.gameObject);
        }, (entity) => {
            int instanceID = Mathf.Abs(entity.gameObject.GetInstanceID());
            if (currentlyActiveEnemies.ContainsKey(instanceID))
            {
                currentlyActiveEnemies.Remove(instanceID);
            }

            entity.SetActive(false);
        }, (entity) => {
          Destroy(entity);
        });


    }


    public void onEnemyKilled(Enemy p_enemy)
    {
        int key = p_enemy.gameObject.GetInstanceID();
        if (this.currentlyActiveEnemies.ContainsKey(key) )
        {
            this.EnemiesMarkedForDeath.Add(key);
        }
    }

    public List<Enemy> getEnemyPositionsWithinRange(Rect p_range)
    {
        return tree.queryRange(p_range);
    }

    public List<GameObject> getEnemies() //Should maybe reconsider my dictionary approach.
    {
        return this.currentlyActiveEnemies.Values.ToList();
    }

    private void clearMarkedEnemiesForDeath()
    {
        for (int i = 0; i < this.EnemiesMarkedForDeath.Count; i++)
        {
            int key = this.EnemiesMarkedForDeath[i];
            this.enemyPool.Release(this.currentlyActiveEnemies[key]);
            this.currentlyActiveEnemies.Remove(key);
        }
        this.EnemiesMarkedForDeath.Clear();
    }

    private void UpdateTree() 
    {
        this.clearMarkedEnemiesForDeath();

        tree.clear(); //Need to keep in mind the isometric view conversion.
        foreach(KeyValuePair<int,GameObject> entry in this.currentlyActiveEnemies)
        {
            GameObject currentEntity = entry.Value;
            Enemy enemy = currentEntity.GetComponent<Enemy>();
            this.tree.insert(enemy);
        }

    }

    void LateUpdate()
    {
        //Consider early update, not sure where to place it yet.
        this.UpdateTree();

    }
}
