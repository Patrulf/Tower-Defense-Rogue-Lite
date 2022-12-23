using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//based on pseudocode on wikipedia: https://en.wikipedia.org/wiki/Quadtree
public class QuadTree
{

    private const int NODE_CAPACITY = 4;
    private Rect boundary;
    private QuadTree northWest;
    private QuadTree northEast;
    private QuadTree southWest;
    private QuadTree southEast;

    private List<Enemy> enemies; //Dictionary instead?

    public QuadTree(Rect p_boundary) 
    {
        enemies = new List<Enemy>();
        boundary = p_boundary;
        northWest = northEast = southWest = southEast = null;



    }

    public List<Enemy> queryRange(Rect range)
    {
        List<Enemy> pointsInRange = new List<Enemy>();


        if (!this.boundary.Overlaps(range))
        {
            return pointsInRange;
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            Vector2 carthesianPos = MathHelper.isometricToCartesian(enemies[i].transform.position);
            if (range.Contains(carthesianPos))
            {
                pointsInRange.Add(enemies[i]);
            }
        }

        if (northWest == null)
        {
            return pointsInRange;
        }

        pointsInRange.AddRange(northWest.queryRange(range));
        pointsInRange.AddRange(northEast.queryRange(range));
        pointsInRange.AddRange(southWest.queryRange(range));
        pointsInRange.AddRange(southEast.queryRange(range));

        return pointsInRange;
    }

    void subdivide()
    {

        northWest = new QuadTree(
            new Rect(
                     boundary.x,
                     boundary.y + boundary.height * 0.5f,
                     boundary.width * 0.5f, boundary.height * 0.5f)
            );
        northEast = new QuadTree(
            new Rect(
                     boundary.x + boundary.width * 0.5f,
                     boundary.y + boundary.height * 0.5f,
                     boundary.width * 0.5f, boundary.height * 0.5f)
            );
        southEast = new QuadTree(
            new Rect(
                     boundary.x + boundary.width * 0.5f,
                     boundary.y,
                     boundary.width * 0.5f, boundary.height * 0.5f)
            );
        southWest = new QuadTree(
            new Rect(
                     boundary.x,
                     boundary.y,
                     boundary.width * 0.5f, boundary.height * 0.5f)
            );
    }

    public bool insert(Enemy p_enemy)
    {

        Vector2 carthesianPos = MathHelper.isometricToCartesian(p_enemy.transform.position);


        if (!boundary.Contains(carthesianPos))
        {
            return false;
        }

        if (enemies.Count < NODE_CAPACITY && northWest == null)
        {
            enemies.Add(p_enemy);
            return true;
        }

        if (northWest == null)
        {
            subdivide();
        }

        if (northWest.insert(p_enemy))
        {
            return true;
        }
        if (northEast.insert(p_enemy))
        {
            return true;
        }
        if (southWest.insert(p_enemy))
        {
            return true;
        }
        if (southEast.insert(p_enemy))
        {
            return true;
        }
        return false; 
    }

    public void clear()
    {
        enemies.Clear();
        if (northWest != null)
        {
            northWest.clear();
            northWest = null;
        }
        if (northEast != null)
        {
            northEast.clear();
            northEast = null;
        }
        if (southWest != null)
        {
            southWest.clear();
            southWest = null;
        }
        if (southEast != null)
        {
            southEast.clear();
            southEast = null;
        }
    }
}
