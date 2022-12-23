using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class ProjectileMover : MonoBehaviour
{



    public void move(Vector2 direction, float range, float speed, System.Action onFinished)
    {
        StartCoroutine(movementCoroutine(direction, range, speed, onFinished));
    }

    public IEnumerator movementCoroutine(Vector2 direction, float range, float speed, System.Action onFinished) //could put a destruction callback here for less spaghetti i guess.
    {
        float distanceTraveled = 0.0f;


        while(distanceTraveled < range)
        {
            float delta = Time.deltaTime;


            float middleOffsetY = 0.5f;
            float rangeRectRadius = 1.0f;
            Rect rangeRect = new Rect(this.transform.position.x, this.transform.position.y - 1, rangeRectRadius, rangeRectRadius);
            rangeRect = MathHelper.rectIsometricToCartesian(rangeRect);

            List<Enemy> enemiesWithinRangeSquare = EntityManager.instance.getEnemyPositionsWithinRange(rangeRect);
            if (enemiesWithinRangeSquare.Count > 0)
            {
                Vector2 centeredPosition = new Vector2(this.transform.position.x, this.transform.position.y - 1);

                List<Enemy> filteredEnemies = enemiesWithinRangeSquare.Where(x => Vector2.Distance(centeredPosition, new Vector2(x.transform.position.x, x.transform.position.y)) < rangeRectRadius).ToList();
                if (filteredEnemies.Count > 0)
                {
                    for (int i = 0; i < filteredEnemies.Count; i++)
                    {
                        //foreach enemy reduce hp.
                        filteredEnemies[i].OnTakingDamage(25.0f);
                        onFinished();
                        yield return null; 
                    }
                }
            }
            this.transform.position = new Vector2(transform.position.x + direction.x * delta * speed, transform.position.y + direction.y * delta * speed);
            distanceTraveled += delta * speed;
            yield return 0; 
        }

        onFinished();
        yield return null;
    }


}
