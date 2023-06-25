using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GeneratePlanesFromCSV : MonoBehaviour
{
    public string planesFile;

    public float width = 2.73984f;
    public float height = 0.493213f;
    public Vector3 center = new Vector3(2.04446f, 1.39046f, 10.6672f);
    public Vector3 normal = new Vector3(0.616617f, 2.28907e-05f, 0.787263f);
    public float dipAngle = 38.0695f;
    public float dipDirection = 89.9979f;

    void Start()
    {
        GameObject parent = new GameObject("planes");
        // Convert dip angle and direction to radians
        float dipAngleRadians = dipAngle * Mathf.Deg2Rad;
        float dipDirectionRadians = dipDirection * Mathf.Deg2Rad;

        // Calculate dip vector
        Vector3 dipVector = new Vector3(
            Mathf.Sin(dipAngleRadians) * Mathf.Sin(dipDirectionRadians),
            Mathf.Sin(dipAngleRadians) * Mathf.Cos(dipDirectionRadians),
            Mathf.Cos(dipAngleRadians)
        ).normalized;

        // Calculate strike vector
        Vector3 strikeVector = Vector3.Cross(normal, dipVector).normalized;

        // Calculate half-width and half-height
        float halfWidth = width / 2;
        float halfHeight = height / 2;

        // Calculate corner points
        Vector3 corner1 = center + halfWidth * strikeVector + halfHeight * dipVector;
        Vector3 corner2 = center - halfWidth * strikeVector + halfHeight * dipVector;
        Vector3 corner3 = center - halfWidth * strikeVector - halfHeight * dipVector;
        Vector3 corner4 = center + halfWidth * strikeVector - halfHeight * dipVector;
        Debug.Log(corner1);
        Debug.Log(corner2);
        Debug.Log(corner3);
        Debug.Log(corner4);

        // Debug draw rectangle
        Debug.DrawLine(corner1, corner2, Color.red, Mathf.Infinity);
        Debug.DrawLine(corner2, corner3, Color.red, Mathf.Infinity);
        Debug.DrawLine(corner3, corner4, Color.red, Mathf.Infinity);
        Debug.DrawLine(corner4, corner1, Color.red, Mathf.Infinity);

        Vector3[] vertices = { corner1, corner2, corner3, corner4 };
        int[] triangles = { 0, 1, 2, 0, 2, 3 };

        GameObject planeObject = new GameObject("plane");
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        MeshRenderer meshRenderer = planeObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
        meshRenderer.sharedMaterial.color = new Color(1, 0, 0);

        MeshFilter mf = planeObject.AddComponent<MeshFilter>();
        mf.mesh = mesh;

        // Add to parent
        planeObject.transform.parent = parent.transform;
        // parent.transform.localRotation = Quaternion.Euler(0, 90, 0);
    }

    // private void Start()
    // {
    //     List<PlaneData> planes = LoadPlaneData();

    //     GameObject parent = new GameObject("planes");

    //     foreach (PlaneData data in planes)
    //     {
    //         Vector3 right = Vector3.Cross(data.normal, Vector3.up).normalized;
    //         Vector3 up = Vector3.Cross(right, data.normal).normalized;

    //         Vector3 upperLeft = (up * data.height / 2) - (right * data.width / 2);
    //         Vector3 upperRight = (up * data.height / 2) + (right * data.width / 2);
    //         Vector3 lowerLeft = -(up * data.height / 2) - (right * data.width / 2);
    //         Vector3 lowerRight = -(up * data.height / 2) + (right * data.width / 2);

    //         // Calculate rotation based on dipAngle and dipDirection
    //         Quaternion rotation =
    //             Quaternion.AngleAxis(data.dipAngle, data.normal);
    //             // * Quaternion.AngleAxis(data.dipAngle, right);

    //         // Quaternion rotation = Quaternion.Euler(-data.dipAngle, data.dipDirection, 0);
    //         // Apply rotation to the corner points
    //         upperLeft = data.center + /* rotation * */ upperLeft;
    //         upperRight = data.center + /* rotation * */ upperRight;
    //         lowerLeft = data.center + /* rotation * */ lowerLeft;
    //         lowerRight = data.center + /* rotation * */ lowerRight;

    //         // upperLeft = new Vector3(1.670131f, 1.206187f, 10.665615f);
    //         // upperRight = new Vector3(2.418789f, 1.206187f, 10.667754f);
    //         // lowerLeft = new Vector3(1.670131f, 0.776733f, 10.665615f);
    //         // lowerRight = new Vector3(2.418789f, 0.776733f, 10.667754f);

    //         Vector3[] vertices = { upperLeft, upperRight, lowerLeft, lowerRight };
    //         int[] triangles = { 0, 1, 2, 2, 1, 3 };

    //         GameObject planeObject = new GameObject(data.name);
    //         Mesh mesh = new Mesh();
    //         mesh.vertices = vertices;
    //         mesh.triangles = triangles;
    //         mesh.RecalculateNormals();

    //         MeshRenderer meshRenderer = planeObject.AddComponent<MeshRenderer>();
    //         meshRenderer.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
    //         meshRenderer.sharedMaterial.color = new Color(1, 0, 0);

    //         MeshFilter mf = planeObject.AddComponent<MeshFilter>();
    //         mf.mesh = mesh;

    //         // // Rotate
    //         // Vector3 right2 = Vector3.Cross(data.normal, Vector3.up).normalized;
    //         // Vector3 dipDirectionVector =
    //         //     Quaternion.AngleAxis(data.dipDirection, data.normal) * right2;
    //         // Vector3 up2 = Vector3.Cross(dipDirectionVector, data.normal).normalized;
    //         // up2 = Quaternion.AngleAxis(-data.dipAngle, dipDirectionVector) * up2;
    //         // planeObject.transform.localRotation = Quaternion.LookRotation(data.normal, up2);

    //         // Add to parent
    //         planeObject.transform.parent = parent.transform;
    //     }
    //     parent.transform.localRotation = Quaternion.Euler(0, 90, 0);
    // }

    private List<PlaneData> LoadPlaneData()
    {
        List<PlaneData> planes = new List<PlaneData>();

        using (StreamReader reader = new StreamReader(planesFile))
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                Debug.Log(line);
                if (line.Trim().Length == 0) // Skip empty lines
                    continue;

                string[] data = line.Split(';');
                PlaneData planeData = new PlaneData
                {
                    name = data[0],
                    width = float.Parse(data[1]),
                    height = float.Parse(data[2]),
                    center = new Vector3(
                        float.Parse(data[3]),
                        float.Parse(data[4]),
                        float.Parse(data[5])
                    ),
                    normal = new Vector3(
                        float.Parse(data[6]),
                        float.Parse(data[7]),
                        float.Parse(data[8])
                    ).normalized,
                    dipAngle = float.Parse(data[9]),
                    dipDirection = float.Parse(data[10])
                };
                planes.Add(planeData);
            }
        }

        return planes;
    }

    private class PlaneData
    {
        public string name;
        public float width;
        public float height;
        public Vector3 center;
        public Vector3 normal;
        public float dipAngle;
        public float dipDirection;
    }
}
