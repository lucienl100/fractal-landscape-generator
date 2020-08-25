using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waves : MonoBehaviour
{
    // Start is called before the first frame update
    public int dim;
    public float scale;
    void Start()
    {
        
        Mesh water = new Mesh();
        water.name = "waterMesh";
        water.vertices = GenVertices();
        water.triangles = GenTriangles(water);

        MeshFilter meshFilter = this.gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = water;
        MeshRenderer renderer = this.gameObject.AddComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private Vector3[] GenVertices()
    {
        var vertices = new Vector3[(dim + 1) * (dim + 1)];
        int idx = 0;
        for (int x = 0; x < dim + 1; x++)
        {
            for (int z = 0; z < dim + 1; z++)
            {
                vertices[idx] = new Vector3((float)x * scale, 0, (float)z * scale);
                idx++;
            }
        }
        return vertices;
    }
    private int[] GenTriangles(Mesh mesh)
    {
        var triangles = new int[mesh.vertices.Length * 6];
        int idx = 0;
        for (int x = 0; x < dim; x++)
        {
            for (int z = 0; z < dim; z++)
            {
                //Triangle 1
                //
                //|\
                //| \
                //|__\
                triangles[idx] = (x * (dim + 1)) + z + 1;
                triangles[idx + 1] = (x + 1)*(dim + 1) + z;
                triangles[idx + 2] = (x * (dim + 1)) + z;
                //Triangle 2
                // __
                //\  |
                // \ |
                //  \|
                triangles[idx + 3] = (x * (dim + 1)) + z + 1;
                triangles[idx + 4] = ((x + 1) * (dim + 1)) + z + 1;
                triangles[idx + 5] = (x + 1)*(dim + 1) + z;
                idx += 6;
            }
        }
        return triangles;
    }
}
