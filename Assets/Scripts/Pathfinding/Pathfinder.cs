using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathTile
{

    public int gScore;

    public int hScore;

    public int fScore;

    public Vector2 position;

    public PathTile parent;

}

public class Pathfinder : MonoBehaviour
{

    private List<PathTile> _openList;
    private List<PathTile> _closedList;

    public GameGrid gameGrid;


    private List<PathTile> _nodes;

    private Coroutine movementCoroutine;



    // Start is called before the first frame update
    void Awake() 
    {

        _openList = new List<PathTile>();
        _closedList = new List<PathTile>();
        _nodes = new List<PathTile>();

        Vector2Int dimensions = gameGrid.getDimensions();
        createNodes((uint)dimensions.x, (uint)dimensions.y);


        transform.position = gameGrid.cartesianToIsometric(new Vector2(0,0) );

    }

    private void createNodes(uint p_width, uint p_height)
    {
        for (int y = 0; y < p_height; y++)
        {
            for (int x = 0; x < p_width; x++)
            {
                PathTile node = new PathTile();
                Vector2 isometric = gameGrid.cartesianToIsometric(new Vector2(x, y));
                node.position = new Vector2(isometric.x, isometric.y);
                node.fScore = 0;
                node.hScore = 0;
                node.gScore = 0;
                _nodes.Add(node);
            }
        }
    }

    private void resetNodes()
    {
        for (int i = 0; i < _nodes.Count; i++)
        {
            _nodes[i].fScore = 0;
            _nodes[i].gScore = 0;
            _nodes[i].hScore = 0;
            _nodes[i].parent = null;
        }
    }


    private PathTile getLowestFCostInList(List<PathTile> p_list)
    {
        int currentLowestFCost = int.MaxValue;
        int currentLowestHCost = int.MaxValue;
        PathTile lowestNode = null;
        foreach (PathTile node in p_list)
        {
            if (node.fScore < currentLowestFCost)
            {
                currentLowestFCost = node.fScore;
                currentLowestHCost = node.hScore;
                lowestNode = node;
            } else if (node.fScore == currentLowestFCost && node.hScore < currentLowestHCost)
            {
                currentLowestFCost = node.fScore;
                currentLowestHCost = node.hScore;
                lowestNode = node;
            }
        }
        return lowestNode;
    }

    private void updateNeighbours(PathTile p_currentNode, List<PathTile> p_nodes, Vector2 p_targetPosition)
    {
        Vector2 initialPosition = p_currentNode.position;

        Vector2Int dimensions = gameGrid.getDimensions();

        for (int xOffset = -1; xOffset < 2; xOffset++)
        {
            for (int yOffset = -1; yOffset < 2; yOffset++)
            {
                if (xOffset == 0 && yOffset == 0)
                {
                    continue;
                }

                Vector2 currentCartesian = gameGrid.isometricToCartesian(initialPosition);
                currentCartesian = new Vector2(currentCartesian.x + xOffset, currentCartesian.y + yOffset);

                Vector2Int initialPositionCartesian = Vector2Int.FloorToInt(gameGrid.isometricToCartesian(initialPosition));

                if (gameGrid.isValidTilePosition(initialPositionCartesian.x + xOffset, initialPositionCartesian.y + yOffset) == false)
                {
                    continue;
                }



                PathTile currentNeighbour = p_nodes[(initialPositionCartesian.x + xOffset) +
                    (yOffset + initialPositionCartesian.y) * dimensions.x];




                if (!gameGrid.isValidTilePosition((int)currentCartesian.x, (int)currentCartesian.y))
                {
                    continue;
                }
                if (gameGrid.getTileOccupation((int)currentCartesian.x, (int)currentCartesian.y) != OccupiedState.Path)
                {
                    continue;
                }

                bool isNeighbourDiagonal = xOffset != 0 && yOffset != 0;
                bool isSameWallAdjacentToBothIfNeighbourDiagonal =
                    gameGrid.getTileOccupation(new Vector2Int(initialPositionCartesian.x + xOffset, initialPositionCartesian.y)) != OccupiedState.Path
                    || gameGrid.getTileOccupation(new Vector2Int(initialPositionCartesian.x, initialPositionCartesian.y + yOffset)) != OccupiedState.Path;
                if (isNeighbourDiagonal && isSameWallAdjacentToBothIfNeighbourDiagonal)
                {
                    continue;
                }


               if (_closedList.Contains(currentNeighbour))
                {
                    continue;
                }



                const float D = 10.0f;
                const float D2 = 14.0f;

                float gX = Mathf.Abs(currentNeighbour.position.x - p_currentNode.position.x);
                float gY = Mathf.Abs(currentNeighbour.position.y - p_currentNode.position.y);
                int tempGScore = p_currentNode.gScore + (int)(D * (gX + gY) + (D2 - 2*D) * Mathf.Min(gX,gY) );


                float hX = Mathf.Abs(currentNeighbour.position.x - p_targetPosition.x);
                float hY = Mathf.Abs(currentNeighbour.position.y - p_targetPosition.y);
                int tempHScore = (int)(D * (hX + hY) + (D2 - 2 * D) * Mathf.Min(hX, hY));


                int tempFScore = tempGScore + tempHScore;
                if (tempGScore < currentNeighbour.gScore || !_openList.Contains(currentNeighbour))
                {
                    currentNeighbour.gScore = tempGScore;
                    currentNeighbour.fScore = tempFScore;
                    currentNeighbour.hScore = tempHScore;
                    currentNeighbour.parent = p_currentNode;

                    if (!_openList.Contains(currentNeighbour))
                    {
                        _openList.Add(currentNeighbour);
                    }
                }

            }
        }
    }

    private PathTile pathFind(Vector2 p_target)
    {
        resetNodes();
        _openList.Clear();
        _closedList.Clear();


        Vector2 startPosition = gameGrid.isometricToCartesian(transform.position);

        int width = gameGrid.getDimensions().x;
        var firstNode = _nodes[(int)startPosition.x + (int)startPosition.y * width];

        _openList.Add(firstNode);


        PathTile currentNode = null;
        int iterations = 0;
        while (_openList.Count != 0)
        {
            iterations++;
            currentNode = getLowestFCostInList(_openList);
            _openList.Remove(currentNode);
            _closedList.Add(currentNode);


            if (Vector2.Distance(currentNode.position,p_target) < Config.epsilon )
            {
                break; //done.
            }

            updateNeighbours(currentNode, _nodes, p_target);

            if (_openList.Count == 0 || _closedList.Count == 0)
            {
                break;//error, path not found.
            }
        }
        //Debug.Log("Total iterations: " + iterations);
        return currentNode;
    }

    public Stack<Vector2> getPath(Vector2 p_endPoint)
    {
        Vector2 target = p_endPoint;

        PathTile endNode = pathFind(target);

        Stack<Vector2> path = new Stack<Vector2>();
        PathTile currentNode = endNode;
        while (currentNode != null)
        {
            path.Push(currentNode.position);
            currentNode = currentNode.parent;
        }
        return path;
    }

    public void Move(Vector2 p_target)
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }
        if (gameGrid.isValidTilePositionIsometric(p_target) && gameGrid.getTileOccupationIsometric(p_target) == OccupiedState.Path)
        {
            movementCoroutine = StartCoroutine(moveAlongPath(p_target));
        }
    }

    private IEnumerator moveAlongPath(Vector2 p_target)
    {
        Stack<Vector2> path = getPath(p_target);


        Vector2 currentWayPoint = path.Pop();
        while (true)
        {
            float deltaX = currentWayPoint.x - transform.position.x;
            float deltaY = currentWayPoint.y - transform.position.y;
            Vector2 direction = new Vector2(deltaX, deltaY).normalized;
            Vector3 nextStep = new Vector3(direction.x, direction.y, 0.0f) * Time.deltaTime * 5.0f;

            float distanceToTarget = Vector2.Distance(currentWayPoint, transform.position);
            float epsilon = 0.01f;
            if (System.Math.Abs(nextStep.magnitude - distanceToTarget) < epsilon || nextStep.magnitude > distanceToTarget)
            {
                transform.position = new Vector2(currentWayPoint.x, currentWayPoint.y);
                if (path.Count == 0)
                {
                    yield break; // exit coroutine.
                }
                currentWayPoint = path.Pop();
            }
            else
            {
                if (nextStep == Vector3.zero) //this is issue.
                {
                    Debug.LogError("NEXT STEP 0, ERROR");
                    Debug.LogError(nextStep.magnitude);
                    Debug.LogError(distanceToTarget);
                }
                transform.position += nextStep;
            }
            yield return null; // wait for next frame.
        }
    }
}
