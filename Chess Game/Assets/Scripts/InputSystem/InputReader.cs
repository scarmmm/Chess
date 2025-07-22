using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, Controls.IPlayerActions
{
    private Controls _controls;
    public Vector2 MovementValue { get; private set; }
    public event Action OnMoveEvent;
    private void Start()
    {
      _controls = new Controls();
      _controls.Player.SetCallbacks(this);
      _controls.Player.Enable();
    }
    private void OnDestory()
    {
        _controls.Player.Disable();
    }
 
      
    public void OnMove(InputAction.CallbackContext context)
    {
        MovementValue = context.ReadValue<Vector2>();
    }

    public void OnMouseLook(InputAction.CallbackContext context)
    {

    }

}