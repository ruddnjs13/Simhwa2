using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAim : MonoBehaviour, IPlayerComponent
{
    [SerializeField] private Transform _weaponHolder;
    private Player _player;
    private InputReaderSO _inputReader;

    [Header("SightInfo")] [Range(0f, 360f)]
    public float viewAngle;

    [Range(1f, 12f)] public float viewRadius;

    public Vector3 HolderPosition => _weaponHolder.position;

    public void Initialize(Player player)
    {
        _player = player;
        _inputReader = _player.GetCompo<InputReaderSO>();
    }

    private void Update()
    {
        UpdateAim();
    }

    private void UpdateAim()
    {
        Vector3 worldPos = _inputReader.MousePosition;
        Vector3 lookDirection = (worldPos - _weaponHolder.position).normalized;
        _weaponHolder.right = lookDirection;
    }

    public Vector3 DirectionFromAngle(float degree, bool isGlobalAngle)
    {
        if(!isGlobalAngle)
        {
            degree += _weaponHolder.eulerAngles.z;
        }
        float rad = degree * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
    }
}
