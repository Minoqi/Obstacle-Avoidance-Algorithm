using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVTool : MonoBehaviour
{
    public GameObject player;

    [Header("FOV Tool")]
    public float distance, angle, height;
    public Color meshColor;
    private Mesh fovMesh;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Mesh CreateFOVMesh()
    {
        // Variables
        Mesh mesh = new Mesh();

        int segments = 10; // Each segment has 4 triangles, 2 for the far side and 2 for top/bottom each
        int numTriangles = (segments * 4) + 2 + 2; // 2 + 2 is for the left & right side
        int numVertices = numTriangles * 3; // Each triangle has 3 vertices
        Vector3[] vertices = new Vector3[numVertices];
        int vert = 0; // Keeps track of loc in vertices[]
        int[] triangles = new int[numVertices];

        // Find points of the wedge
        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

        Vector3 topCenter = bottomCenter + Vector3.up * height; // Same as bottom just shifted up
        Vector3 topLeft = bottomLeft + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;

        // Left side (2 triangles)
        vertices[vert++] = bottomCenter; // 1st triangle
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft; // 2nd triangle
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        // Right side (2 triangles)
        vertices[vert++] = bottomCenter; // 1st triangle
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight; // 2nd triangle
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        // Calculate sides of each segment
        float currentAngle = -angle;
        float deltaAngle = (angle * 2) / segments;

        for (int i = 0; i < segments; i++)
        {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;

            topLeft = bottomLeft + Vector3.up * height;
            topRight = bottomRight + Vector3.up * height;

            // Far side (2 triangles)
            vertices[vert++] = bottomLeft; // 1st triangle
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight; // 2nd triangle
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;

            // Top (1 triangle)
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            // Bottom (1 triangle)
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;

            currentAngle += deltaAngle;
        }

        for (int i = 0; i < numVertices; i++) // Can just loop over # of vertices since no vertex sharing
        {
            triangles[i] = i;
        }

        // Assign values to mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        // Make mesh for collider & update collider
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;

        return mesh;
    }

    // Updates values from inspector
    private void OnValidate()
    {
        fovMesh = CreateFOVMesh();
    }

    private void OnDrawGizmos()
    {
        if (fovMesh)
        {
            Gizmos.color = meshColor;
            Gizmos.DrawMesh(fovMesh, transform.position, transform.rotation);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        player.GetComponent<ObstalceAvoidance>().AvoidObstacle(other.transform.gameObject);   
    }

    private void OnTriggerExit(Collider other)
    {
        player.GetComponent<ObstalceAvoidance>().debugLine.enabled = false;
        player.GetComponent<ObstalceAvoidance>().obstacleHitBool = false;
    }
}
