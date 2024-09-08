using System.Numerics;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class CheckRange : Conditional
{
    public SharedTransform target;
    public float radius = 5f;

    public override TaskStatus OnUpdate()
    {
        Vector3 playerPosition = target.Value.position;
        Vector3 myPosition = transform.position;

        Vector3 direction = playerPosition - myPosition;

        if (direction.magnitude < radius)
        {
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failure;
        }
    }
}
