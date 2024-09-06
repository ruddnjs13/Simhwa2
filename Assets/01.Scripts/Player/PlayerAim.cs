using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAim : MonoBehaviour, IPlayerComponent
{
    [SerializeField] private Transform _weaponHolder;
    private Player _player;
    private InputReaderSO _inputReader;

    [Header("SightInfo")] [Range(0f, 360f)]
    public float viewAngle;
    [Range(1f, 12f)] public float viewRadius;
    public Vector3 HolderPosition => _weaponHolder.position;

    [Header("RayCastInfo")] 
    [SerializeField] private ContactFilter2D _enemyFilter;

    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private float _enemyFindDelay = 0.3f;
    [SerializeField] private int _maxCheckCount = 10;
     public List<Transform> visibleTargets = new List<Transform>();
     private Collider2D[] _enemiesInView;

    public void Initialize(Player player)
    {
        _player = player;
        _inputReader = _player.GetCompo<InputReaderSO>();
        _enemiesInView = new Collider2D[_maxCheckCount];
    }

    private void Start()
    {
        StartCoroutine(FindEnemyWithDelay());
    }

    private IEnumerator FindEnemyWithDelay()
    {
        var time = new WaitForSeconds(_enemyFindDelay);
        while (true)
        {
            yield return time;
            FindVisibleElement();
        }
        
    }

    private void FindVisibleElement()
    {
        visibleTargets.Clear();
        int cnt = Physics2D.OverlapCircle(HolderPosition, viewRadius, _enemyFilter, _enemiesInView);
        for (int i = 0; i < cnt; i++)
        {
            Transform enemy = _enemiesInView[i].transform;
            Vector3 direction = enemy.position - HolderPosition;

            if (Vector3.Angle(_weaponHolder.right, direction.normalized) < viewAngle * 0.5f)
            {
                if (!Physics2D.Raycast(HolderPosition, direction.normalized, direction.magnitude,
                        _obstacleMask))
                {
                    visibleTargets.Add(enemy);
                }
            }
        }
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
