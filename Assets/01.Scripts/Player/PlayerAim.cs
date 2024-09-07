using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public struct ViewCastInfo
{
    public bool isHit;
    public Vector3 point;
    public float distane;
    public float angle;
}

public struct EdgeInfo
{
    public Vector3 pointA;
    public Vector3 pointB;
}


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
    [Range(0.2f,2f)] [SerializeField] private float _meshResolution;
    [SerializeField] private int _edgeIterationCount = 4;
    [SerializeField] private float _edgeThreshold;
    [SerializeField] private Light2D _fovLight;
    
    
     public List<Transform> visibleTargets = new List<Transform>();
     private Collider2D[] _enemiesInView;

     private Transform _viewVisual;
     private MeshFilter _meshFilter;
     private Mesh _viewMesh;

    public void Initialize(Player player)
    {
        _player = player;
        _inputReader = _player.GetCompo<InputReaderSO>();
        _enemiesInView = new Collider2D[_maxCheckCount];
        
        _viewVisual = transform.Find("ViewVisual");
        _meshFilter = _viewVisual.GetComponent<MeshFilter>();
        _viewMesh = new Mesh();
        _viewMesh.name = "ViewMesh";
        _meshFilter.mesh = _viewMesh;

        MeshRenderer renderer = _viewVisual.GetComponent<MeshRenderer>();
        renderer.sortingLayerName = "Agent";
        renderer.sortingOrder = 20;
    }

    [ContextMenu("Adjust Light Angle")]
    private void AdjustLightAngle()
    {
        _fovLight.pointLightOuterRadius = viewRadius;
        _fovLight.pointLightInnerRadius = viewRadius - 1f;
        _fovLight.pointLightOuterAngle = viewAngle;
        _fovLight.pointLightInnerAngle = viewAngle - viewAngle * 0.2f;
    }

    private void Start()
    {
        StartCoroutine(FindEnemyWithDelay());
    }

    #region FindEnemyRegion

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
    #endregion

    private void Update()
    {
        UpdateAim();
    }

    private void LateUpdate()
    {
        DrewFieldOfView();
    }

    private void DrewFieldOfView()
    {
        int stepcount = Mathf.RoundToInt(viewAngle * _meshResolution);
        float stepAngle = viewAngle / stepcount;

        List<Vector3> viewPoints = new List<Vector3>();

        ViewCastInfo oldCastInfo = new ViewCastInfo();
        
        
        for (int i = 0; i <= stepcount; i++)
        {
            float angle = _weaponHolder.eulerAngles.z + viewAngle * 0.5f - stepAngle * i;
            ViewCastInfo castInfo = ViewCast(angle);

            if (i > 0)
            {
                bool edgeThresholdExeed = Mathf.Abs(oldCastInfo.distane - castInfo.distane) > _edgeThreshold;

                if (oldCastInfo.isHit != castInfo.isHit 
                    || oldCastInfo.isHit && castInfo.isHit && edgeThresholdExeed)
                {
                    EdgeInfo edge = FindEdge(oldCastInfo, castInfo);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }
            
            viewPoints.Add(castInfo.point);

            oldCastInfo = castInfo;
        }

        int vertCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertCount];
        int[] triangles = new int[(vertCount - 2) * 3];
        vertices[0] = Vector3.zero;

        for (int i = 0; i < vertCount - 1; i++)
        {
            vertices[i + 1] = _viewVisual.InverseTransformPoint(viewPoints[i]);

            if (i < vertCount - 2)
            {
                int tIndex = i * 3;
                triangles[tIndex + 0] = 0;
                triangles[tIndex + 1] = i + 1;
                triangles[tIndex + 2] = i +2 ;
            }
        }
        _viewMesh.Clear();
        _viewMesh.vertices = vertices;
        _viewMesh.triangles = triangles;
        _viewMesh. RecalculateNormals();
    }

    private EdgeInfo FindEdge(ViewCastInfo minCastInfo, ViewCastInfo maxCastInfo)
    {
        float minAngle = minCastInfo.angle;
        float maxAngle = maxCastInfo.angle;

        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < _edgeIterationCount; i++)
        {
            float angle = (minAngle + maxAngle) * 0.5f;
            var castInfo = ViewCast(angle);

            bool edgeThresholdExceed = Mathf.Abs(minCastInfo.distane - castInfo.distane) > _edgeThreshold;

            if (castInfo.isHit == minCastInfo.isHit && edgeThresholdExceed == false)
            {
                minAngle = angle;
                minPoint = castInfo.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = castInfo.point;
            }
        }

        return new EdgeInfo { pointA = minPoint, pointB = maxPoint };
    }

    private ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 direction = DirectionFromAngle(globalAngle, true);
        var hitInfo = Physics2D.Raycast(HolderPosition, direction, viewRadius, _obstacleMask);

        if (hitInfo.collider != null)
        {
            return new ViewCastInfo
                { 
                    isHit = true, 
                    point = hitInfo.point, 
                    angle = globalAngle, 
                    distane = hitInfo.distance 
                };
        }
        else
        {
            return new ViewCastInfo
            {
                isHit = false,
                point = HolderPosition + direction * viewRadius,
                angle = globalAngle,
                distane = viewRadius
            };
        }
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
