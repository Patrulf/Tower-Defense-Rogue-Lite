using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Building : MonoBehaviour
{

    //TODO: add stuff here.


    private IFiringType _firingType;
    private IFireTargetType _targetingType;

    public BuildingData buildingData;


    private bool isReadyToFire;


    // Start is called before the first frame update
    void Start()
    {
        this.isReadyToFire = true;

        _firingType = this.GetComponent<IFiringType>();
        _targetingType = this.GetComponent<IFireTargetType>();
    }


    private void Update()
    {
        float range = this.buildingData.range;


        //Just to center on sprite.
        float middleOffsetY = 0.5f;
        float enemyMiddleOffsetY = 0.5f; 


        //TODO: THIS VERY WELL COULD BE WRONG, AND SHOULD BE FUCKING REFACTORED ANYWAYS. FUCK THESE FUCKING OFFSETS MAN.
        //range rect needs to be in isometric coordinates.
        Rect rangeRect = new Rect(this.transform.position.x ,this.transform.position.y + middleOffsetY , range, range); //this fucking offset is retarded?
        rangeRect = MathHelper.rectIsometricToCartesian(rangeRect);
        rangeRect.x -= range * 0.5f;
        rangeRect.y -= range * 0.5f; //I think? I have no fucking idea.

        List<Enemy> enemies = EntityManager.instance.getEnemyPositionsWithinRange(rangeRect);

        if (enemies.Count > 0)
        {
            Vector2 centeredPosition = new Vector2(this.transform.position.x, this.transform.position.y + middleOffsetY);
            //Todo: make sure it is centered on middle of our actual sprite. Should probably add as a property on our enemies as well.
            List<Enemy> filteredEnemies = enemies.Where(x => Vector2.Distance(centeredPosition, new Vector2(x.transform.position.x,x.transform.position.y + enemyMiddleOffsetY)) < range).ToList();

            //TODO: FIRE MY LAZER.

            if (filteredEnemies.Count == 0)
            {
                return;
            }

            float closestDistance = float.MaxValue;
            Enemy currentTarget = null;
            for (int i = 0; i < filteredEnemies.Count; i++)
            {

                float currentDistance = Vector2.Distance(this.transform.position, filteredEnemies[i].transform.position); //here using transform.position still holds true.
                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    currentTarget = filteredEnemies[i];
                }
            }

            Vector2 towerDirection = new Vector2(currentTarget.transform.position.x - transform.position.x,
                currentTarget.transform.position.y - transform.position.y).normalized;
            //For now  we just make something up, should probably not send this in here but idk.

            //Now we fire, however we do that.

            if (this.isReadyToFire)
            {
                this.isReadyToFire = false;
                this._firingType.fire(centeredPosition, towerDirection, currentTarget);
                StartCoroutine(awaitReadyToFire());
            }
        }
    }

    private IEnumerator awaitReadyToFire()
    {
        yield return new WaitForSeconds(1.0f);
        this.isReadyToFire = true;
        yield return null;
    }



}


