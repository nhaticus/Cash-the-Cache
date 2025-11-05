using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBox2 : MonoBehaviour
{
    [SerializeField] Vector3 size;
    [SerializeField] Quaternion orientation;
    [SerializeField] GameObject room;

    [SerializeField] Vector3 position;
    [SerializeField] bool UseVector2 = false;
    BoxCollider box;
    Vector3 boxSize;

    private void Start()
    {
        box = room.GetComponent<BoxCollider>();
        boxSize = box.size;
    }

    Vector3 worldCenter;
    void Update()
    {
        Vector3 localCenter = box.center;
        worldCenter = box.transform.TransformPoint(localCenter);

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("try find");
            Collider[] hitColliders = Physics.OverlapBox(room.transform.position, size / 2, orientation);
            foreach (Collider hit in hitColliders)
            {
                Debug.Log("hit: " + hit.name);
            }
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, orientation, Vector3.one);
        if (UseVector2 == false)
            DrawBox(worldCenter, orientation, boxSize, Color.red);
        else
            Gizmos.DrawWireCube(position, size);
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
