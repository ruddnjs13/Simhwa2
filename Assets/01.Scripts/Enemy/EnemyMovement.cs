using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour,IEnemyComponent
{
    private Enemy _enemy;
    private Rigidbody2D _rbCompo;
    
    private Vector2 _velocity;
    
    public void Initialize(Enemy enemy)
    {
        _enemy = enemy;
        _rbCompo = GetComponent<Rigidbody2D>();
    }
    public void SetVelocity(Vector2 velocity)
    {
        _velocity = velocity;
    }
    private void FixedUpdate()
    {
        _rbCompo.velocity = _velocity;
    }
}
