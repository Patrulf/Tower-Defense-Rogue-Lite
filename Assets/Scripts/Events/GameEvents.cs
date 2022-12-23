using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{

    public static GameEvents instance;

    public event Action onClickExpandButton; //event for generating buttons.

    public event Action<Vector2, uint> onNewRoundStarted; //position / roundIndex.

    public event Action<Vector2> onMouseMoved;
    //TODO: IMPLEMENTATION.

    public event Action<Vector2> onSelect;

    public event Action<Enemy> onEnemyKilled;

    //PLAYER CAMERA.
    public event Action onDragCameraStart;
    public event Action onDragCameraEnd;
    public event Action onCameraMoveUpStart;
    public event Action onCameraMoveUpEnd;
    public event Action onCameraMoveDownStart;
    public event Action onCameraMoveDownEnd;
    public event Action onCameraMoveLeftStart;
    public event Action onCameraMoveLeftEnd;
    public event Action onCameraMoveRightStart;
    public event Action onCameraMoveRightEnd;
    public event Action onCameraMoved;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Debug.LogError("GameEvents must be a singleton");
        }
    }

    public void enemyKilled(Enemy p_enemy)
    {
        if (onEnemyKilled != null)
        {
            onEnemyKilled(p_enemy);
        }
    }

    public void select(Vector2 mousePosition)
    {
        if (onSelect != null)
        {
            onSelect(mousePosition);
        }
    }

    public void newRoundStarted(Vector2 position, uint roundIndex)
    {
        if (onNewRoundStarted != null)
        {
            onNewRoundStarted(position, roundIndex);
        }
    }
    public void expandButtonGeneration()
    {
        if (onClickExpandButton != null)
        {
            onClickExpandButton();
        }
    }

    public void mouseMoved(Vector2 mousePosition) //mouse pos in camera coords.
    {
        if (onMouseMoved != null)
        {
            onMouseMoved(mousePosition);
        }
    }

    //TODO: add camera as parameter.
    public void cameraMoved()
    {
        if (onCameraMoved != null)
        {
            onCameraMoved();
        } 
    }

    public void dragCameraStart()
    {
        if (onDragCameraStart != null)
        {
            onDragCameraStart();
        }
    }
    public void dragCameraEnd()
    {
        if (onDragCameraEnd != null)
        {
            onDragCameraEnd();
        }
    }

    public void cameraMoveUpStart()
    {
        if (onCameraMoveUpStart != null)
        {
            onCameraMoveUpStart();
        }
    }
    public void cameraMoveUpEnd()
    {
        if (onCameraMoveUpEnd != null)
        {
            onCameraMoveUpEnd();
        }
    }

    public void cameraMoveLeftStart()
    {
        if (onCameraMoveLeftStart != null)
        {
            onCameraMoveLeftStart();
        } 
    }
    public void cameraMoveLeftEnd()
    {
        if (onCameraMoveLeftEnd != null)
        {
            onCameraMoveLeftEnd();
        }
    }

    public void cameraMoveRightStart()
    {
        if (onCameraMoveRightStart != null)
        {
            onCameraMoveRightStart();
        }
    }

    public void cameraMoveRightEnd()
    {
        if (onCameraMoveRightEnd != null)
        {
            onCameraMoveRightEnd();
        }
    }
    public void cameraMoveDownStart()
    {
        if (onCameraMoveDownStart != null)
        {
            onCameraMoveDownStart();
        }
    }
    public void cameraMoveDownEnd()
    {
        if (onCameraMoveDownEnd != null)
        {
            onCameraMoveDownEnd();
        }
    }




}
