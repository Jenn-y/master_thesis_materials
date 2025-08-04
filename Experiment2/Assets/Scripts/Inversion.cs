using UnityEngine;
 
[RequireComponent(typeof(MeshFilter))]
public class Inversion : MonoBehaviour {
    void Start() {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++) normals[i] = -normals[i];
        mesh.normals = normals;
 
        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3) {
            int temp = triangles[i];
            triangles[i] = triangles[i + 1];
            triangles[i + 1] = temp;
        }
        mesh.triangles = triangles;
    }
}