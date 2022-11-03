using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;


public class EnemyAI : MonoBehaviour
{
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    void Start()
    {
        StartCoroutine("FindTargetsWithDelay", .2f);
    }


    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}

[CustomEditor(typeof(EnemyAI))]
public class ExampleEditor : Editor
{

    public void OnSceneGUI()
    {
        EnemyAI enemy = target as EnemyAI;

        Handles.color = Color.white;
        Handles.DrawWireArc(enemy.transform.position, Vector3.up, Vector3.forward, 360, enemy.viewRadius);

        Vector3 viewAngleA = enemy.DirFromAngle(-enemy.viewAngle / 2, false);
        Vector3 viewAngleB = enemy.DirFromAngle(enemy.viewAngle / 2, false);
        Handles.DrawLine(enemy.transform.position, enemy.transform.position + viewAngleA * enemy.viewRadius);
        Handles.DrawLine(enemy.transform.position, enemy.transform.position + viewAngleB * enemy.viewRadius);

        Handles.color = Color.red;
        foreach (Transform visibleTarget in enemy.visibleTargets)
        {
            Handles.DrawLine(enemy.transform.position, visibleTarget.position);
        }
    }
}