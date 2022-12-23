using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class MathHelper
{


    public static Vector2 cartesianToIsometric(float x, float y)
    {
        return new Vector2(x - y, (x + y) * 0.5f);
    }


    public static Rect rectCartesianToIsometric(Rect p_rect)
    {
        Vector2 positionIso = cartesianToIsometric(p_rect.x, p_rect.y);

        return new Rect(positionIso.x, positionIso.y, p_rect.width, p_rect.height);
    }
    public static Rect rectIsometricToCartesian(Rect p_rect)
    {
        Vector2 positionIso = isometricToCartesian(p_rect.x, p_rect.y);

        return new Rect(positionIso.x, positionIso.y, p_rect.width, p_rect.height);
    }





    public static Vector2 cartesianToIsometric(Vector2 p)
    {
        //move on x positive.
        //(add 1x to x.
        //add 0.5 to y.) ---x

        //move on y positive.
        //(subtract 1x to x.
        //add 0.5 to y.) ---y

        //therefore xIso = 1x - 1y
        //and therefore yIso = 0.5x + 0.5y.
        return new Vector2(p.x - p.y, (p.x + p.y) * 0.5f);
    }

    public static Vector2 isometricToCartesian(Vector2 p)
    {
        //move on x positive.
        //xIso = xCart - yCart
        //yIso = (xCart + yCart)*0.5

        //Final values.
        //xCart = xIso/2 + yIso
        //yCart = yIso - xIso/2

        return new Vector2(p.x * 0.5f + p.y, p.y - p.x * 0.5f);
    }
    public static Vector2 isometricToCartesian(float x, float y)
    {
        //move on x positive.
        //xIso = xCart - yCart
        //yIso = (xCart + yCart)*0.5

        //Final values.
        //xCart = xIso/2 + yIso
        //yCart = yIso - xIso/2

        return new Vector2(x * 0.5f + y, y - x * 0.5f);
    }



}
