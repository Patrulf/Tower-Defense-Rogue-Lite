using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class HealthBarSystem : MonoBehaviour
{

    private EntityManager entityManager;

    public GameObject healthBarPrefab;

    //pool of healthbars, return upon enemy dying.

    private ObjectPool<GameObject> healthBarPool;

    private List<GameObject> _currentlyActive;

    public Canvas canvas;


    void Start()
    {
        this._currentlyActive = new List<GameObject>();
        this.entityManager = EntityManager.instance;

        this.healthBarPool = new ObjectPool<GameObject>(() =>
        {
            GameObject prefab = Instantiate(healthBarPrefab);
            prefab.transform.parent = canvas.transform;
            return prefab;
        }, (healthBar) =>
         {
             healthBar.SetActive(true);
         }, (healthBar) =>
         {
             healthBar.SetActive(false);
         }, (healthBar) =>
         {
             Destroy(healthBar);
         });
    }

    void LateUpdate()
    {
        
        //later on wait for event.
        for (int i = 0; i < this._currentlyActive.Count; i++)
        {
            this.healthBarPool.Release(this._currentlyActive[i]);
        }
        this._currentlyActive.Clear();

        List<GameObject> enemies = this.entityManager.getEnemies();

        for (int i = 0; i < enemies.Count; i++)
        {
            Enemy enemyScript = enemies[i].GetComponent<Enemy>();

            GameObject healthBar = this.healthBarPool.Get();
            RectTransform healthBarTransform = healthBar.GetComponentInChildren<RectTransform>();


            RectTransform healthBarCurrent = healthBar.GetComponentsInChildren<RectTransform>()[1];


            Vector3 imageScale = new Vector3(enemyScript.enemyData.currentHealth / enemyScript.enemyData.maxHealth, healthBarCurrent.localScale.y, healthBarCurrent.localScale.z); //TODO: MAX HEALTH VARIABLE.
            healthBarCurrent.localScale = imageScale;
            Vector3 canvasPosition = Camera.main.WorldToScreenPoint(new Vector3(enemyScript.transform.position.x, enemyScript.transform.position.y + 1.0f, -10.0f));
            healthBarTransform.position = canvasPosition;

            this._currentlyActive.Add(healthBar);
        }


    }
}
