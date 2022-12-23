using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pathfinder))]
public class Enemy : MonoBehaviour
{
    private Pathfinder pathfinder;
    public EnemyData enemyData;

    //Todo: enemy scriptableobject here for getting data about enemy.


    private void Awake()
    {
        pathfinder = GetComponent<Pathfinder>();
        enemyData = GetComponent<EnemyData>(); //TODO: make read only?
    }

    public void OnTakingDamage(float p_damage)
    {
        this.enemyData.currentHealth = Mathf.Max(this.enemyData.currentHealth - p_damage, 0.0f);
        if (this.enemyData.currentHealth <= 0.0f + Mathf.Epsilon)
        {
            GameEvents.instance.enemyKilled(this);
            //Todo: fire kill event, notifying EntityManager to kill him.
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
