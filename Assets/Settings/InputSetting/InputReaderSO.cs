using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;

[CreateAssetMenu(menuName = "SO/InputReader")]
public class InputReaderSO : ScriptableObject, IPlayerActions,IPlayerComponent
{
    private Player _player;
    public Vector2 Movement { get; private set; }
    public Vector3 MousePosition { get; private set; }
    
    private Controls _controls;
    
    private void OnEnable()
    {
        if (_controls == null)
        {
            _controls = new Controls();
        }
        _controls.Player.Enable();
        _controls.Player.SetCallbacks(this);
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        Movement = context.ReadValue<Vector2>();
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        Vector2 screenPos = context.ReadValue<Vector2>();
        Vector3 worlPos = Camera.main.ScreenToWorldPoint(screenPos);

        worlPos.z = 0;
        MousePosition = worlPos;
    }

    public void Initialize(Player player)
    {
        _player = player;
    }
}
