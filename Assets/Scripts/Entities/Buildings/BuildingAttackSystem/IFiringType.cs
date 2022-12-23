using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IFiringType : MonoBehaviour
{

    public abstract void fire(Vector2 towerPosition, Vector2 towerDirection, Enemy p_targetEnemy);
}
