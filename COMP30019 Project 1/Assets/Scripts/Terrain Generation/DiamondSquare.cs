using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;

public class DiamondSquare : MonoBehaviour
{
    [Range(2, 100)] public int nValue = 2;
    [Range(0.0f, 1000.0f)] public float baseVariance = 1.0f;
    private float variance = 1.0f;
    private float[][] grid;
    private int gridSize;
    //This function run once when Unity is in Play
    void Start()
    {
        Debug.Log("hello");
        GenerateMesh();
    }
    void GenerateMesh()
	{
        GenerateEmptyGrid();
        DiamondSquareAlgorithm();
        RemoveDebug();

	}

    void DiamondSquareAlgorithm()
	{
        /*
         * Uses the diamond square algorithm
         * Sets the 4 corners of the grid to a random value
         * Each time the code iterates through the i variable, there will be 2^i squares that the program will have to iterate through
         * Each square is passed throught the Step method, which will then set the designated height values
         */
        //  Set the corners
        SetHeight(new Vector2(0, 0), RandomHeight());
        SetHeight(new Vector2(gridSize - 1, 0), RandomHeight());
        SetHeight(new Vector2(gridSize - 1, gridSize - 1), RandomHeight());
        SetHeight(new Vector2(0, gridSize - 1), RandomHeight());

        variance = baseVariance;
        float maxV = gridSize - 1;
        for (int i = 0; i < nValue; i++)
        {
            int sqrtSquares = (int)Math.Pow(2, i);
            float divided = maxV / sqrtSquares;
            //  Run for each row of squares
            for (int j = 0; j < sqrtSquares; j++)
            {
                float y = j * divided;
                //  Run for each column of squares
                for (int k = 0; k < sqrtSquares; k++)
                {
                    float x = k * divided;
                    Vector2 p1 = new Vector2(x, y);
                    Vector2 p3 = new Vector2(x, y) + Vector2.one * divided;
                    Step(p1, p3);
                }
            }
            LowerVariance();
        }
    }

    void GenerateTriangles()
	{

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
        SetHeight(mp, GenerateAverage(mp, p1) + RandomHeight());

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
        Vector2 p5 = new Vector2(mp.x, p1.y);
        Vector2 p6 = new Vector2(p3.x, mp.y);
        Vector2 p7 = new Vector2(mp.x, p3.y);
        Vector2 p8 = new Vector2(p1.x, mp.y);

        Vector2[] diamondCorners = new[]
        {
         new Vector2(mp.x, p1.y),
         new Vector2(p3.x, mp.y),
         new Vector2(mp.x, p3.y),
         new Vector2(p1.x, mp.y)
        };
        
        foreach (Vector2 v in diamondCorners)
		{
            SetHeight(v, GenerateAverage(mp, v) + RandomHeight());
		}
    }

    float GenerateAverage(Vector2 corner, Vector2 mp)
	{
        //  Generates the average height from at most, four corners
        Vector2 dVec = mp - corner;
        Vector2[] points = new[]
        {
            mp + new Vector2(1, 1) * dVec,
            mp + new Vector2(-1, 1) * dVec,
            mp + new Vector2(-1, -1) * dVec,
            mp + new Vector2(1, -1) * dVec
        };

        int count = 0;
        float sum = 0;
        foreach (Vector2 v in points)
		{
            //  Some points may not have 4 corners
			if (InGrid(v))
			{
                sum += GetHeight(v);
                count++;
			}
		}
        //  Don't want to potentially divide by 0
		if (count == 0)
		{
            return 0;
		}
        return sum / count;
	}

    float GetHeight(Vector2 v)
	{
        //  Returns the height of a point
        return grid[(int)v.x][(int)v.y];
	}

    void SetHeight(Vector2 v, float h)
	{
        //  Sets the height of a point
        grid[(int)v.x][(int)v.y] = h;
	}

    bool InGrid(Vector2 v)
	{
        //  Checks if a point is in the grid
        if(v.x >= gridSize || v.x < 0 || v.y >= gridSize || v.y < 0)
		{
            return false;
		}
        return true;
	}

    float RandomHeight()
	{
        //  Returns a random height based off of the variance
        return Random.Range(-1.0f, 1.0f) * variance;
	}
    void LowerVariance()
	{
        //  Lowers the variance
        variance = variance * 0.50f;
	}
    void GenerateEmptyGrid()
	{
        //  Generates an empty grid
        gridSize = (int)Math.Pow(2.0, nValue) + 1;
        grid = new float[gridSize][];
        for(int i = 0; i < gridSize; i++)
		{
            grid[i] = new float[gridSize];
		}
	}
    
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
            DebugGrid();
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
            GenerateMesh();
            DebugGrid();
		}
	}

    void DebugGrid()
	{
        string message = "";
        //  Print the array
        for (int i = 0; i < gridSize; i++)
        {
            if (i != 0)
            {
                message += "\n";
            }
            for (int j = 0; j < gridSize; j++)
            {
                GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                s.transform.position = new UnityEngine.Vector3(i, grid[i][j], j);
                s.transform.SetParent(this.transform);
                message += grid[i][j] + " ";
            }
        }
        Debug.Log(message);
    }

    void RemoveDebug()
	{
        //  Remove all children sphere for debug mode
        foreach (Transform child in this.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
