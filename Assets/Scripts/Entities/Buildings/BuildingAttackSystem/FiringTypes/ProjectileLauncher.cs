using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


public class ProjectileLauncher : IFiringType //Consider renaming to fire mechanism.
{
    public IFireTargetType fireTargetType;

    private void Start()
    {
        

    }

    public override void fire(Vector2 towerPosition, Vector2 towerDirection, Enemy p_targetEnemy)
    {
        Vector2 projectileDirection = fireTargetType.GetDirectionOfProjectile(towerPosition,p_targetEnemy);
        GameObject projectile = EntityManager.instance.projectilePool.Get();

        #if (UNITY_EDITOR)
        if (!projectile.GetComponent<ProjectileMover>())
        {
            Debug.LogError("Projectile does not have a projectileMover");
        }
        #endif

        projectile.transform.position = towerPosition;

        ProjectileMover projectileMover = projectile.GetComponent<ProjectileMover>();

        projectileMover.move(projectileDirection, 10.0f, 10.0f, () => {
            EntityManager.instance.projectilePool.Release(projectile);
        });
    }
}
