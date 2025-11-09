using System.Collections;
using UnityEngine;

public class TestBox2 : MonoBehaviour
{
    [SerializeField] Vector3 size;
    [SerializeField] Quaternion orientation;
    Vector3 orientVector;
    [SerializeField] Vector3 expectedPosition;

    [SerializeField] GameObject roomPrefab;

    [SerializeField] GameObject dummy;
    [SerializeField] GameObject dummyRoom;

    BoxCollider box;
    Vector3 boxSize;

    Vector3 boxCenter;
    Vector3 boxPosition;

    private void Start()
    {
        orientVector = orientation.eulerAngles;

        box = roomPrefab.GetComponent<BoxCollider>();
        boxSize = box.size;

        boxCenter = box.transform.TransformPoint(box.center); // try doing it without using spawned room
        boxPosition = expectedPosition + boxCenter;
        Debug.Log("expected pos: " + expectedPosition + "  boxcenter: " + boxCenter);
        Debug.Log("boxPosition: " + boxPosition); // gives correct position without rotation

        // set dummy
        dummy.transform.position = boxPosition;
        dummy.transform.rotation = orientation;
        dummy.transform.localScale = boxSize;

        // check if correct with dummy room
        dummyRoom.transform.position = expectedPosition;
        dummyRoom.transform.rotation = orientation;

        // set room to rotation
        
        dummy.transform.position = RotatePointAroundPivot(dummy.transform.position, expectedPosition, orientVector);
    }

    void Update()
    {
        boxCenter = box.transform.TransformPoint(box.center);
        boxPosition = expectedPosition + boxCenter;

        Vector3 x = RotatePointAroundPivot(boxPosition, expectedPosition, orientVector);
        dummy.transform.position = x;
        dummyRoom.transform.position = expectedPosition;
        dummyRoom.transform.rotation = orientation;

        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("try find");
            Collider[] hitColliders = Physics.OverlapBox(x, boxSize / 2, orientation);
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

    float rot = 0;
    IEnumerator RotateDummy()
    {
        Vector3 rotation;
        yield return new WaitForSeconds(1f);
        while (true)
        {
            rot += 90 * Time.deltaTime;
            rotation = new Vector3(0, rot, 0);
            dummy.transform.position = RotatePointAroundPivot(dummy.transform.position, expectedPosition, rotation);
            // dummyRoom.transform.rotation = Quaternion.Euler(rotation);
            yield return new WaitForSeconds(0.4f);
        }
    }

    // good but doesn't rotate gizmo
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        DrawBox(RotatePointAroundPivot(boxPosition, expectedPosition, orientVector), orientation, boxSize, Color.red);
    }

    /// <summary>
    /// https://stackoverflow.com/questions/65982489/predict-transform-position-after-rotation
    /// </summary>
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
