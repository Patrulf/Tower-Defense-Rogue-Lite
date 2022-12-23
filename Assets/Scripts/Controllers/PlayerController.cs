using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;



public class PlayerController : MonoBehaviour, PlayerControls.IPlayerMapActions
{
    private PlayerControls controls;

    private Vector2 mousePosition;


    private void Awake()
    {

        if (controls == null)
        {
            controls = new PlayerControls();
            controls.PlayerMap.SetCallbacks(this);
        }
        controls.PlayerMap.Enable();

    }

    void Update()
    {


    }


    private void Start()
    {
        GameEvents.instance.onClickExpandButton += onExpandButtonClicked;
    }

    public void onExpandButtonClicked()
    {
        //Todo: start game and stuff here I guess.

        //Debug.Log("Expansion button was clicked");
    }

    public void onBuildButtonClicked() 
    {
        //TODO: IMPLEMENT EVENT, WITH BUILDSYSTEM
    }




    public void OnSelect(InputAction.CallbackContext context)
    {
        //Todo, maybe some kind of event system?

        if (context.performed) //on press
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            GameEvents.instance.select(mousePosition);
           
        } else if (context.canceled) //on release
        {
            //do other stuff.
        }
    }

    public void OnDrag(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnCameraMovementUp(InputAction.CallbackContext context)
    {
        if (context.performed) //on press
        {

            GameEvents.instance.cameraMoveUpStart();

        }
        else if (context.canceled) //on release
        {

            GameEvents.instance.cameraMoveUpEnd();
        }
    }

    public void OnCameraMovementDown(InputAction.CallbackContext context)
    {
        if (context.performed) //on press
        {
            GameEvents.instance.cameraMoveDownStart();

        }
        else if (context.canceled) //on release
        {
            GameEvents.instance.cameraMoveDownEnd();
        }
    }

    public void OnCameraMovementLeft(InputAction.CallbackContext context)
    {
        if (context.performed) //on press
        {
            GameEvents.instance.cameraMoveLeftStart();

        }
        else if (context.canceled) //on release
        {
            GameEvents.instance.cameraMoveLeftEnd();
        }
    }

    public void OnCameraMovementRight(InputAction.CallbackContext context)
    {
        if (context.performed) //on press
        {
            GameEvents.instance.cameraMoveRightStart();

        }
        else if (context.canceled) //on release
        {
            GameEvents.instance.cameraMoveRightEnd();
        }
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = context.action.ReadValue<Vector2>();
        GameEvents.instance.mouseMoved(mousePosition);
    }




}
