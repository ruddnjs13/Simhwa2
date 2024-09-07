using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerAim))]
public class CustomPlayerAim  : Editor
{
    private void OnSceneGUI()
    {
        PlayerAim aim = target as PlayerAim;
        Vector3 center = aim.HolderPosition;

        Handles.color = Color.white;
        
        Handles.DrawWireArc(center,Vector3.forward, Vector3.right,360f,aim.viewRadius);
        Vector3 viewAngleA = aim.DirectionFromAngle(-aim.viewAngle * 0.5f, false);
        Vector3 viewAngleB = aim.DirectionFromAngle(aim.viewAngle * 0.5f, false);
        
        
        Handles.DrawLine(center,center+ viewAngleA * aim.viewRadius);
        Handles.DrawLine(center,center+ viewAngleB * aim.viewRadius);


        Handles.color = Color.red;

        foreach (Transform trm in aim.visibleTargets)
        {
            Handles.DrawLine(center,trm.position);
        }
    }
}
