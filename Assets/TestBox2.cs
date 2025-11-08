using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestBox2 : MonoBehaviour
{
    [SerializeField] Vector3 size;
    [SerializeField] Quaternion orientation;
    [SerializeField] GameObject room;

    [SerializeField] Vector3 expectedPosition;
    [SerializeField] GameObject dummy;
    BoxCollider box;
    Vector3 boxSize;

    Vector3 boxPosition;

    private void Start()
    {
        box = room.GetComponent<BoxCollider>();
        boxSize = box.size;

        Vector3 localCenter = box.center;
        boxCenter = box.transform.TransformPoint(localCenter);
        boxPosition = transform.position + expectedPosition + boxCenter;
        Debug.Log("boxPosition: " + boxPosition);

        Vector3 assume = RotatePointAroundPivot(dummy.transform.position, boxPosition, new Vector3(0, 90, 0));
        Debug.Log("box Position: " + RotatePointAroundPivot(dummy.transform.position, boxPosition, new Vector3(0, 90, 0)));
        Debug.Log("box center: " + RotatePointAroundPivot(dummy.transform.position, boxCenter, new Vector3(0, 90, 0)));
        Debug.Log("expectedPosition: " + RotatePointAroundPivot(dummy.transform.position, expectedPosition, new Vector3(0, 90, 0)));
        Debug.Log("transform.position: " + RotatePointAroundPivot(dummy.transform.position, transform.position, new Vector3(0, 90, 0)));
        Debug.Log("box.position: " + RotatePointAroundPivot(dummy.transform.position, box.transform.position, new Vector3(0, 90, 0)));
        Debug.Log("room.position: " + RotatePointAroundPivot(dummy.transform.position, room.transform.position, new Vector3(0, 90, 0)));
        dummy.transform.position = assume;
        dummy.transform.localScale = boxSize;
        dummy.transform.Rotate(new Vector3(0, 90, 0));
    }

    Vector3 boxCenter;
    void Update()
    {
        Vector3 localCenter = box.center;
        boxCenter = box.transform.TransformPoint(localCenter);
        boxPosition = transform.position + expectedPosition + boxCenter;

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("try find");
            Collider[] hitColliders = Physics.OverlapBox(boxPosition, boxSize / 2, orientation);
            foreach (Collider hit in hitColliders)
            {
                Debug.Log("hit: " + hit.name);
            }
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("boxPosition: " + boxPosition);
        }

    }

    // good but doesn't rotate gizmo
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        DrawBox(boxPosition, orientation, boxSize, Color.red);
    }

    private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }

    public void DrawBox(Vector3 pos, Quaternion rot, Vector3 scale, Color c)
    {
        // create matrix
        Matrix4x4 m = new Matrix4x4();
        m.SetTRS(pos, rot, scale);

        var point1 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, 0.5f));
        var point2 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, 0.5f));
        var point3 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, -0.5f));
        var point4 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, -0.5f));

        var point5 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, 0.5f));
        var point6 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, 0.5f));
        var point7 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, -0.5f));
        var point8 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, -0.5f));

        Debug.DrawLine(point1, point2, c);
        Debug.DrawLine(point2, point3, c);
        Debug.DrawLine(point3, point4, c);
        Debug.DrawLine(point4, point1, c);

        Debug.DrawLine(point5, point6, c);
        Debug.DrawLine(point6, point7, c);
        Debug.DrawLine(point7, point8, c);
        Debug.DrawLine(point8, point5, c);

        Debug.DrawLine(point1, point5, c);
        Debug.DrawLine(point2, point6, c);
        Debug.DrawLine(point3, point7, c);
        Debug.DrawLine(point4, point8, c);
    }
}
