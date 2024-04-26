using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[ExecuteAlways]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class NavMeshDebugRender : MonoBehaviour
{
    [SerializeField] Material material;
    private void Awake()
    {
        CalculateNavMeshVisual();
    }

    void Start()
    {
        CalculateNavMeshVisual();
    }

    public void CalculateNavMeshVisual()
    {
        NavMeshTriangulation meshData = NavMesh.CalculateTriangulation();

        // Create a new mesh and chuck in the NavMesh's vertex and triangle data to form the mesh.
        Mesh mesh = new Mesh();
        mesh.vertices = meshData.vertices;
        mesh.triangles = meshData.indices;

        // Assigns the newly-created mesh to the MeshFilter on the same GameObject.
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = material;
    }
}
