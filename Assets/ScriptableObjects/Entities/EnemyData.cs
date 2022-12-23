using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth;
    public bool isDead;

    private void Awake()
    {
        currentHealth = 50.0f;
        maxHealth = currentHealth;
    }

    private void Start()
    {
        currentHealth = 50.0f;
        maxHealth = currentHealth;
    }

}
