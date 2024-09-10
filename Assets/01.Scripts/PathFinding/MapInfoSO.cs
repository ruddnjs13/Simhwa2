using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "SO/PathFinding/MapInfo")]
public class MapInfoSO : ScriptableObject
{
    public Tilemap _floor, _collider;
    private List<Vector3Int> _directions;

    private void Initialize(Tilemap floorTile, Tilemap colliderTile)
    {
        _floor = floorTile;
        _collider = colliderTile;
        _directions = new List<Vector3Int>(8);

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;
                _directions.Add(new Vector3Int(j,i));
            }
        }
    }

    // public Vector3 GetRandomOneTile(Vector3 from)
    // {
    //     Vector3Int fromTilePos = GetTilePosFromWorldPos(from);
    //     for (int i = _directions.Count - 1; i >= 0; i--)
    //     {
    //         int idx = Random.Range(0, i + 1);
    //         Vector3Int nextPos = fromTilePos + _directions[idx];
    //         if (CanMove(nextPos))
    //         {
    //             return GetCellCenterToWorld(nextPos);
    //         }
    //     }
    // }

    private bool CanMove(Vector3Int nextPos)
    {
        return _floor.GetTile(nextPos) != null && _collider.GetTile(nextPos) == null;
    }

    private Vector3Int GetTilePosFromWorldPos(Vector3 from) => _floor.WorldToCell(from);
    public Vector3 GetCellCenterToWorld(Vector3Int cellCenter) => _floor.GetCellCenterWorld(cellCenter);

}
