using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IFireTargetType : MonoBehaviour
{
    public abstract Vector2 GetDirectionOfProjectile(Vector2 towerPosition, Enemy p_target);
}
