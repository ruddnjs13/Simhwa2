using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour,IPlayerComponent
{
    [SerializeField] private float _moveSpeed = 5f;

    private Rigidbody2D _rbCompo;
    private InputReaderSO _inputReader;
    private Player _player;
    
    public void Initialize(Player player)
    {
        _player = player;
        _inputReader = _player.GetCompo<InputReaderSO>();
        _rbCompo = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        Vector2 movement = _inputReader.Movement;
        _rbCompo.velocity = movement * _moveSpeed;
    }
}
