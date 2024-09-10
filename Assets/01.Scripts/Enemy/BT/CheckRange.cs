using System.Numerics;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class CheckRange : Conditional
{
    public SharedTransform target;
    public SharedEnemy enemy;
    
    private EnemyDataSO _enemyData;

    public override void OnAwake()
    {
        _enemyData = enemy.Value.enemyData;
    }

    public override TaskStatus OnUpdate()
    {
        Vector3 playerPosition = target.Value.position;
        Vector3 myPosition = transform.position;
        Vector3 direction = playerPosition - myPosition;

        if (direction.magnitude < _enemyData.range)
        {
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failure;
        }
    }

    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _enemyData.range);
    }
}
