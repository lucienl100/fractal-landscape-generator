using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DiamondSquareV2 : MonoBehaviour
{
    public float baseMaxHeight = 5.0f;
    [Range(2, 20)] public int nVal = 6;
    [Range(1.0f, 100.0f)] public float mapScalar = 10.0f;
    [Range(0.0f, 1.0f)] public float heightDecrement = 0.5f;
    private int gridSize;
    private Mesh mesh;
    private Vector3[] verts;
    private int[] triangles;
    private Vector2[] uvs;
    private int windowWidth = 9;
    private float maxHeight = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        
        gridSize = (int)Math.Pow(2, nVal) + 1;
        verts = new Vector3[gridSize * gridSize];
        GenerateMesh();
    }

    void GenerateMesh()
	{
        mesh = GetComponent<MeshFilter>().mesh;
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        verts = new Vector3[gridSize * gridSize];
        triangles = new int[gridSize * gridSize * 6];
        uvs = new Vector2[(int)Math.Pow(gridSize, 2)];
        maxHeight = baseMaxHeight;
        //  Generate x and z coords for all points in the grid, connect triangles with each other
        GenerateVertsTriangles();

        //  Go through each point and change the y coord using the Diamond Square Algorithm
        DiamondSquare();
        MedianFilter();
        mesh.vertices = verts;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
    void GenerateVertsTriangles()
	{
        //  Creates all of the verticies, triangles
        //  Allows for us to change the x/z distance between points
        float dIncrement = mapScalar / gridSize;
        int triIndex = 0;

        for (int i = 0; i < gridSize; i++)
        {
            for(int j = 0; j < gridSize; j++)
			{
                //  Don't try to create triangles if at the right/top of the grid
                if(i < gridSize-1 && j < gridSize - 1)
				{
                    //Triangle 1
                    //
                    //|\
                    //| \
                    //|__\
                    triangles[triIndex] = (i + 1) * gridSize + j;
                    triangles[triIndex + 1] = i * gridSize + j + 1;
                    triangles[triIndex + 2] = i * gridSize + j;
                    //Triangle 2
                    // __
                    //\  |
                    // \ |
                    //  \|
                    triangles[triIndex + 3] = (i + 1) * gridSize + j;
                    triangles[triIndex + 4] = (i + 1) * gridSize + j + 1;
                    triangles[triIndex + 5] = i * gridSize + j + 1;
                    triIndex += 6;
                }
                verts[(i * gridSize) + j] = new Vector3(j, 0.0f, i) * dIncrement;
                uvs[(i * gridSize) + j] = new Vector2((float)j / (gridSize - 1), (float)i / (gridSize - 1));
			}
        }
        
        
	}
    void MedianFilter()
    {
        float[] newHeights = new float[gridSize * gridSize];
        var window = new List<float>();
        int edgeSizeX;
        int edgeSizeY;
        float newHeight;
        int adjustedWindowWidthX;
        int adjustedWindowWidthY;
        for (int x = 0; x < gridSize; x++)
        {
            adjustedWindowWidthX = getAdjustedWindowWidth(x);
            edgeSizeX = getEdgeSize(x);
            for (int y = 0; y < gridSize; y++)
            {
                adjustedWindowWidthY = getAdjustedWindowWidth(y);
                edgeSizeY = getEdgeSize(y);
                for (int fx = 0; fx < adjustedWindowWidthX; fx++)
                {
                    for (int fy = 0; fy < adjustedWindowWidthY; fy++)
                    {
                        window.Add(GetHeight(new Vector2(x + fx - edgeSizeX, y + fy - edgeSizeY)));
                    }
                }
                window.Sort();
                newHeight = window[adjustedWindowWidthX * adjustedWindowWidthY / 2];
                window.Clear();
                newHeights[y * gridSize + x] = newHeight;
            }
        }
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                SetHeight(new Vector2(x, y), newHeights[y * gridSize + x]);
            }
        }

    }
    int getEdgeSize(int i)
    {
        if (i == 0)
        {
            return 0;
        }
        else if (i == gridSize - 1)
        {
            return windowWidth / 2;
        }
        int adjustedWindowWidthI = Mathf.Min(windowWidth, i + 1, gridSize - i + 1);
        int edgeSizeI = (int)Mathf.Ceil((float)adjustedWindowWidthI / 2);
        return edgeSizeI;
    }
    int getAdjustedWindowWidth(int i)
    {
        if (i == 0 || i == gridSize - 1)
        {
            return windowWidth / 2;
        }
        int adjustedWindowWidthI = Mathf.Min(windowWidth, i + 1, gridSize - i + 1);
        return adjustedWindowWidthI;
    }
    void DiamondSquare()
	{
        /*
         * Uses the diamond square algorithm
         * Sets the 4 corners of the grid to a random value
         * Each time the code iterates through the i variable, there will be 2^i squares that the program will have to iterate through
         * Each square is passed throught the Step method, which will then set the designated height values
         */

        //  Set the heights of the corners
        Vector2[] vs = new[]
        {
            new Vector2(0, 0),
            new Vector2(gridSize-1, 0),
            new Vector2(gridSize-1, gridSize-1),
            new Vector2(0, gridSize-1)
        };
        
        foreach (Vector2 v in vs)
        {
            SetHeight(v, RandomInitialHeight());
        }
        LowerHeight();
        for(int i = 0; i < nVal; i++)
		{
            int sqrtSquares = (int)Math.Pow(2, i);
            float divided = (gridSize - 1) / sqrtSquares;
            for(int j = 0; j < sqrtSquares; j++)
			{
                for(int k = 0; k < sqrtSquares; k++)
				{
                    Vector2 p1 = new Vector2(k, j) * divided;
                    Vector2 p3 = new Vector2(k + 1, j + 1) * divided;
                    Step(p1, p3);
				}
			}
            LowerHeight();
		}
    }

    void Step(Vector2 p1, Vector2 p3)
	{
        //  Performs diamond step and square step given an inital square

        /* 
         * Diamond step
         *
         * p4- - - p3
         * - \ - / -
         * - -mp - -
         * - / - \ -
         * p1- - - p2
         * 
         */

        /*
         * We can calculate the midpoint of the square by only using p1 and p3
         * Then, calculate the height of the midpoint by getting the averages of the four corners, and adding a random value
         */

        Vector2 mp = (p1 + p3) / 2;
        SetHeight(mp, AverageHeight(mp, p1) + RandomHeight());

        /*  Square step
        *
        * p4->p7<-p3
        * v - ^ - v
        * p8<mp > p6
        * ^ - v - ^
        * p1->p5<-p2
        */

        /*
         * Next, we need to find and calculate each midpoint and its height of each diamond
         */

        Vector2[] diamondCorners = new[]
        {
             //p5
             new Vector2(mp.x, p1.y), 
             // p6
             new Vector2(p3.x, mp.y),
             // p7
             new Vector2(mp.x, p3.y),
             // p8
             new Vector2(p1.x, mp.y)
        };

        foreach (Vector2 v in diamondCorners)
        {
            SetHeight(v, AverageHeight(v, mp) + RandomHeight());
        }
    }

    float AverageHeight(Vector2 mp, Vector2 corner)
	{
        float sum = 0;
        int count = 0;

        //  Get the position vector between the mid point and the corner point
        Vector2 pV = mp - corner;

        Vector2[] transformations = new[]
        {
            new Vector2(1, 1),
            new Vector2(1, -1),
            new Vector2(-1 , -1),
            new Vector2(-1, -1)
        };
        

        foreach (Vector2 v in transformations)
        {
            Vector2 tempV = mp + v * pV;
            if (inBounds(tempV))
			{
                sum += GetHeight(tempV);
                count++;
			}
		}
        if(count == 0)
		{
            return 0;
		}
        return sum / count;
	}

    bool inBounds(Vector2 v)
	{
        //  Checks if the point is in the bounds of the grid
        if(v.x < 0 || v.x >= gridSize || v.y < 0 || v.y >= gridSize)
		{
            return false;
		}
        return true;
	}
    float RandomInitialHeight()
	{
        //  Returns a random result for the first corners
        return Random.Range(-maxHeight, maxHeight);
	}
    float RandomHeight()
	{
        //  Returns a random float within the parameter
        return Random.Range(-maxHeight, maxHeight);
	}

    void LowerHeight()
	{
        //  Lowers the max height by multiplying by the scalar, heightDecrement
        maxHeight *= heightDecrement;
	}

    void SetHeight(Vector2 v, float h)
	{
        //  Sets the height of a vector3 by a given vector2 (x, z), (y) => (x, y, z)
        verts[gridSize * (int)v.y + (int)v.x].y = h;
	}

    float GetHeight(Vector2 v)
	{
        //  Returns the height of a given Vector2 from its respective Vector3 in verts
        return verts[gridSize * (int)v.y + (int)v.x].y;
	}
    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space))
		{
            GenerateMesh();
		}
    }
}
