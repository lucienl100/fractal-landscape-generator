using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class HeightGrid
{
	//	Grid class for height map
	private Vector3[] grid;
	private int size;
	private float scale = 1.0f;
	public HeightGrid(int size)
	{
		this.grid = new Vector3[size * size];
		this.size = size;
	}
	public int GetGridSize()
	{
		return size;
	}
	public float GetAvgHeight()
    {
        float sum = 0;
        for (int i = 0; i < grid.Length; i+=4)
		{
			sum += grid[i].y;
        }
        float avg = sum / grid.Length * 4;
        return avg;
    }

	public float GetHeight(Vector2 v)
	{
		return grid[(int)v.y * size + (int)v.x].y;
	}

	public void SetHeight(Vector2 v, float h)
	{
		grid[(int)v.y * size + (int)v.x].y = h;
	}

	public void SetVector(int x, int z, Vector3 v)
	{
		grid[z * size + x] = new Vector3(v.x, v.y, v.z);
	}

	public void SetScale(float scale)
	{
		this.scale = scale;
	}
	public Vector3[] GetGrid()
	{
		return grid;
	}

	public void SetGrid(Vector3[] grid)
	{
		for (int i = 0; i < grid.Length; i++)
		{
			this.grid[i] = grid[i];
		}
	}

	public HeightGrid Copy()
	{
		HeightGrid temp = new HeightGrid(size);
		temp.SetGrid(this.grid);
		return temp;
	}

	public override string ToString()
	{
		String output = "";
		for(int y = 0; y<size; y++)
		{
			for(int x = 0; x < size; x++)
			{
				output += this.GetHeight(new Vector2(x, y)) + " ";
			}
			output += "\n";
		}

		return output;
	}

}