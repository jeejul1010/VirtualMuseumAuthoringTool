using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SphereGenerator : MonoBehaviour
{
    public PrimitiveType primitiveType;
    public string filePath;

    void Start()
    {
        GameObject spheres = new GameObject("spheres");
        // Read the text file line by line
        string[] lines = File.ReadAllLines(filePath);

        List<string> labels = new List<string>();
        List<Vector3> points = new List<Vector3>();

        // Iterate through each line
        foreach (string line in lines)
        {
            // Split the line into label, x, y, z
            string[] values = line.Split(',');

            // Parse the x, y, z values as floats
            string label = values[0];
            float x = float.Parse(values[1]);
            float y = float.Parse(values[2]);
            float z = float.Parse(values[3]);

            labels.Add(label);
            points.Add(new Vector3(x, y, z));
        }

        GameObject planes = new GameObject("planes");
        for (var i = 0; i < labels.Count; i += 2)
        {
            var p1 = Quaternion.Euler(0, 90, 0) * points[i];
            var p2 = Quaternion.Euler(0, 90, 0) * points[i + 1];
            p1.y = 0;
            p2.y = 0;
            // Debug.DrawLine(p1, p2, Color.red, 100);

            // Create the mesh vertices, triangles, and UVs
            Vector3[] vertices = new Vector3[4];
            int[] triangles = new int[6];
            Vector2[] uvs = new Vector2[4];

            vertices[0] = p1;
            vertices[1] = p2;
            vertices[2] = p2 + new Vector3(0, 4, 0);
            vertices[3] = p1 + new Vector3(0, 4, 0);

            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;
            triangles[3] = 0;
            triangles[4] = 2;
            triangles[5] = 3;

            // Create the mesh object and assign its vertices, triangles, and UVs
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;

            // Create plane object
            GameObject plane = new GameObject($"plane{i}");
            MeshFilter meshFilter;
            MeshRenderer meshRenderer;
            meshFilter = plane.AddComponent<MeshFilter>();
            meshRenderer = plane.AddComponent<MeshRenderer>();
            meshFilter.mesh = mesh;
            // Set the material of the mesh renderer component
            meshRenderer.material = new Material(Shader.Find("Unlit/Color"));
            meshRenderer.material.color = new Color(1, 0, 0);
            meshRenderer.material.doubleSidedGI = true;

            plane.transform.parent = planes.transform;
        }
        // if (region) { }
        // else
        {
            for (var i = 0; i < labels.Count; ++i)
            {
                // Instantiate a primitive object at the x, y, z position
                GameObject primitive = GameObject.CreatePrimitive(primitiveType);
                primitive.name = labels[i];
                primitive.transform.position = points[i];
                primitive.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                primitive.transform.parent = spheres.transform;

                var p = primitive.transform.position;
                var p2 = Quaternion.Euler(0, 90, 0) * p;
                p2.y = 0;
                Debug.Log(labels[i] + ": " + p2.ToString("F2"));
            }
        }

        spheres.transform.localRotation = Quaternion.Euler(0, 90, 0);
    }
}
