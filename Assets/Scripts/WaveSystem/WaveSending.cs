using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class WaveSending : MonoBehaviour
{
    public GameGrid gameGrid;

    private bool isRoundInProgress;

    private void Awake()
    {
        isRoundInProgress = false;
    }
    private void Start()
    {
        GameEvents.instance.onNewRoundStarted += OnStartNewRound;
    }

    IEnumerator sendUnits(Vector2 startPosition, uint p_count)
    {
        isRoundInProgress = true;
        for (int i = 0; i < p_count; i++)
        {
            Vector2 dimensions = gameGrid.getDimensions();

            GameObject unit = EntityManager.instance.enemyPool.Get();
            unit.transform.position = new Vector3(startPosition.x, startPosition.y);
            Pathfinder pathFinder = unit.GetComponent<Pathfinder>();
            Vector2 basePosition = new Vector2( 4* LevelModuleData.width, 4*LevelModuleData.height);
            basePosition = MathHelper.cartesianToIsometric(basePosition.x,basePosition.y);
            pathFinder.Move(basePosition);
            yield return new WaitForSeconds(0.5f);
        }
        isRoundInProgress = false;
        yield return null; 
    }

    public void OnStartNewRound(Vector2 startPosition,uint roundIndex)
    {
        uint amountOfUnits = 15; //Todo: event for when to return enemies.
        StartCoroutine(sendUnits(startPosition,amountOfUnits)); //todo: need to await this coroutine.
    }
}
