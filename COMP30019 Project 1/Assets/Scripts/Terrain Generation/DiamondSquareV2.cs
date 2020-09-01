using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DiamondSquareV2 : MonoBehaviour
{
    public float baseMaxHeight = 10.0f;
    [Range(2, 20)] public int nVal = 6;
    [Range(1.0f, 256.0f)] public float mapScalar = 192.0f;
    [Range(0.0f, 1.0f)] public float heightDecrement = 0.5f;
    private int gridSize;
    private float highestCornerHeight;
    private Mesh mesh;
    private Transform generator;
    private HeightGrid verts;
    private int[] triangles;
    private Vector2[] uvs;
    private int windowWidth = 9;
    private float maxHeight;
    public float randomTHeight;
    public MeshCollider meshCollider;
    public Material material;
    public bool useMedianFilter = true;
    
    void Awake()
    {
        generator = GetComponent<Transform>();
        gridSize = (int)Math.Pow(2, nVal) + 1;
        mesh = GetComponent<MeshFilter>().mesh;
        meshCollider = GetComponent<MeshCollider>();
        MeshRenderer renderer = this.gameObject.AddComponent<MeshRenderer>();
        renderer.material = material;
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        verts = new HeightGrid(gridSize);
        triangles = new int[gridSize * gridSize * 6];
        uvs = new Vector2[(int)Math.Pow(gridSize, 2)];
        maxHeight = baseMaxHeight;
    }
    public float GetAvgHeight()
    {
        return verts.GetAvgHeight();
    }
    public float GetGridSize()
    {
        return verts.GetGridSize();
    }
    public void GenerateMesh()
	{
        maxHeight = baseMaxHeight;
        //  Generate x and z coords for all points in the grid, connect triangles with each other
        GenerateVertsTriangles();

        //  Go through each point and change the y coord using the Diamond Square Algorithm
        DiamondSquare();
        if (useMedianFilter) {MedianFilter();}
        
        mesh.vertices = verts.GetGrid();
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        meshCollider.sharedMesh = mesh;
        randomTHeight =  0.5f * GetAvgHeight();
    }
    void GenerateVertsTriangles()
	{
        //  Creates all of the verticies, triangles
        //  Allows for us to change the x/z distance between points
        float dIncrement = mapScalar / (gridSize - 1); //  192 
        verts.SetScale(dIncrement);
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
                verts.SetVector(j, i, new Vector3(j, 0.0f, i) * dIncrement);
                uvs[(i * gridSize) + j] = new Vector2((float)j / (gridSize - 1), (float)i / (gridSize - 1));
			}
        }
        
        
	}
    void MedianFilter()
    {
        var window = new List<float>();
        float newHeight;

        int checkWindowOffsetX;
        int checkWindowOffsetY;
        
        int adjustedWindowWidthX;
        int adjustedWindowWidthY;

		HeightGrid copy = verts.Copy();

        for (int x = 0; x < gridSize; x++)
        {
            adjustedWindowWidthX = GetAdjustedWindowWidth(x);
            checkWindowOffsetX = GetWindowOffset(x);
            for (int y = 0; y < gridSize; y++)
            {
                adjustedWindowWidthY = GetAdjustedWindowWidth(y);
                checkWindowOffsetY = GetWindowOffset(y);
                for (int fx = 0; fx < adjustedWindowWidthX; fx++)
                {
                    for (int fy = 0; fy < adjustedWindowWidthY; fy++)
                    {
                        window.Add(copy.GetHeight(new Vector2(x + fx - checkWindowOffsetX, y + fy - checkWindowOffsetY)));
                    }
                }
                window.Sort();
                newHeight = window[adjustedWindowWidthX * adjustedWindowWidthY / 2];

                verts.SetHeight(new Vector2(x, y), newHeight);
                
                window.Clear();
            }
        }
    }
    int GetWindowOffset(int i)
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
        int windowOffset = (int)Mathf.Ceil((float)adjustedWindowWidthI / 2);
        return windowOffset;
    }
    int GetAdjustedWindowWidth(int i)
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
           verts.SetHeight(v, RandomInitialHeight());
        }
        highestCornerHeight = Mathf.Max(vs[0].y, vs[1].y, vs[2].y, vs[3].y);
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
        verts.SetHeight(mp, AverageHeight(mp, p1) + RandomHeight());

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
            verts.SetHeight(v, AverageHeight(v, mp) + RandomHeight());
        }
    }

    float AverageHeight(Vector2 mp, Vector2 corner)
	{
        float sum = 0;
        int count = 0;

        //  Get the position vector between the mid point and the corner point
        Vector2 dV = mp - corner;

        float[] angles = new[] {
            0f, Mathf.PI * 0.5f, Mathf.PI, Mathf.PI * 1.5f
        };
        foreach (float theta in angles)
        {
            //  Equation that rotates a vector around the origin, by a given angle
            Vector2 translation = RotateVector(dV, theta);
            Vector2 tempV = mp + translation;
            if (InBounds(tempV))
			{
                sum += verts.GetHeight(tempV);
                count++;
			}
		}
        if(count == 0)
		{
            return 0;
		}
        return sum / count;
	}

    Vector2 RotateVector(Vector2 v, float theta)
	{
        //  Rotates a vector around the origin, by a given angle
        return new Vector2(Mathf.Round((v.x * Mathf.Cos(theta)) - (v.y * Mathf.Sin(theta))), 
            Mathf.Round((v.x * Mathf.Sin(theta)) + (v.y * Mathf.Cos(theta))));
    }
    bool InBounds(Vector2 v)
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
        return Random.Range(-maxHeight*0.9f, maxHeight*0.9f);
	}

    void LowerHeight()
	{
        //  Lowers the max height by multiplying by the scalar, heightDecrement
        maxHeight *= heightDecrement;
	}
    float Rand(float range)
    {
        return Random.Range(-range, range);
    }
}
