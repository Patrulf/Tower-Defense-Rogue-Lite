using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private Camera camera;


    private bool isDragMoving = false;
    private bool isMovingUp = false;
    private bool isMovingLeft = false;
    private bool isMovingRight = false;
    private bool isMovingDown = false;

    private float speed = 10.0f;

    private void Start()
    {
        camera = Camera.main;
        GameEvents.instance.onCameraMoveUpStart += onMoveUpStart;
        GameEvents.instance.onCameraMoveUpEnd += onMoveUpEnd;
        GameEvents.instance.onCameraMoveDownStart += onMoveDownStart;
        GameEvents.instance.onCameraMoveDownEnd += onMoveDownEnd;
        GameEvents.instance.onCameraMoveLeftStart += onMoveLeftStart;
        GameEvents.instance.onCameraMoveLeftEnd += onMoveLeftEnd;
        GameEvents.instance.onCameraMoveRightStart += onMoveRightStart;
        GameEvents.instance.onCameraMoveRightEnd += onMoveRightEnd;
    }

    private void Update()
    {
        Vector2 direction = Vector2.zero;
        if (isMovingRight)
        {
            direction.x += 1.0f;
        }
        if (isMovingLeft)
        {
            direction.x -= 1.0f;
        }
        if (isMovingUp)
        {
            direction.y += 1.0f;
        }
        if (isMovingDown)
        {
            direction.y -= 1.0f;
        }
        direction = direction.normalized;
        Vector3 movementDelta = new Vector3(direction.x, direction.y, 0.0f) * speed * Time.deltaTime;

        if (Mathf.Abs(movementDelta.x) > Config.epsilon || Mathf.Abs(movementDelta.y) > Config.epsilon )
        {
            GameEvents.instance.cameraMoved();
        }
        camera.transform.position += movementDelta;
    }

    public void onMoveUpStart()
    {
        isMovingUp = true;
    }
    public void onMoveUpEnd()
    {
        isMovingUp = false;
    }
    public void onMoveDownStart()
    {
        isMovingDown = true;
    }
    public void onMoveDownEnd()
    {
        isMovingDown = false;
    }
    public void onMoveLeftStart()
    {
        isMovingLeft = true;
    }
    public void onMoveLeftEnd()
    {
        isMovingLeft = false;
    }
    public void onMoveRightStart()
    {
        isMovingRight = true;
    }
    public void onMoveRightEnd()
    {
        isMovingRight = false;
    }






}
