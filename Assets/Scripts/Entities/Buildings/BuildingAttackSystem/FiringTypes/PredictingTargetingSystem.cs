using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PredictingTargetingSystem : IFireTargetType
{

    public override Vector2 GetDirectionOfProjectile(Vector2 towerPosition, Enemy p_targetEnemy)
    {
        float enemyOffsetY = 0.5f;

        return new Vector2(p_targetEnemy.transform.position.x - towerPosition.x, (p_targetEnemy.transform.position.y + enemyOffsetY ) - towerPosition.y);
    }
}
