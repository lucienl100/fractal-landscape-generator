**The University of Melbourne**
# COMP30019 – Graphics and Interaction

# Project-1 README

Remember that _"this document"_ should be `well written` and formatted **appropriately**. It should be easily readable within Github. Modify this file... 
this is just an example of different formating tools available for you. For help with the format you can find a guide [here](https://docs.github.com/en/github/writing-on-github).

## Table of contents
* [Team Members](#team-members)
* [General Info](#general-info)
* [Technologies](#technologies)
* [Diamond-Square implementation](#diamond-square-implementation)
* [Camera Motion](#camera-motion)
* [Vertex Shader](#vertex-shader)
* [Median Filter](#median-filter)

## Team Members

| Name | Task | State |
| :---         |     :---:      |          ---: |
| Nathan Rearick  | Diamond Square     |  ![95%](https://progress-bar.dev/95) |
| Nathan Rearick  | Camera Movement     |  ![95%](https://progress-bar.dev/95) |
| Lucien Lu    | Water and Water Shader      |  ![95%](https://progress-bar.dev/95) |
| Lucien Lu    | Scene Manager    |  ![95%](https://progress-bar.dev/95) |
| Lucien Lu    | Median Filter    |  ![95%](https://progress-bar.dev/95) |
| Timmy Truong    | Shaders     |  ![70%](https://progress-bar.dev/60) |
| Timmy Truong    | Sun     |  ![80%](https://progress-bar.dev/50) |

## General info
This is project - 1 ...
Project 1 is an implementation of the Diamond Square algorithm and custom HLSL shaders that demonstrates phong shading. This project is based on generating a fractal landscape using the DS algorithm. This implementation is made by, Lucien Lu, Nathan Rearick, Timmy Truong.
	
## Technologies
Project is created with:
* Unity 2019.4.3f1

## Diamond-Square implementation
For the Diamond Square algorithm, the corners of the grid is set to a random value in a specified range. Then the centre of these 4 points is set to the average of the heights of the corners, this is known as the diamond step. Then for each 'square', the center of the vertices is set to the average of the vertices (Note: For points on the edge, only 3 points are used), this is the square step. Due to the nature of needing to find a middle point between the 3/4 vertices, the grid must be 2^n+1 size.

This section of code shows the operation of applying Step() which implements the diamond and square step on each separate square of the grid with decreasing size.
```c#
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
A single method was used to find the average height of related points:
Get the displacement vector between the point of interest and another point which is the based midpoint in the case of a square step, then perform a rotation for 0, 90, 180, 270 degrees on the displacement vector, this will get the four/three points that enclose the point and check if each point is in the grid: at most there will be one missing. Then find the average of these points and assign it to the point of interest. 

Code:

```c#
    float AverageHeight(Vector2 pV, Vector2 mp)
	{
        float sum = 0;
        int count = 0;

        //  Get the displacement vector between the mid point and the corner point
        Vector2 dV = pV - mp;

        float[] angles = new[] {
            0f, Mathf.PI * 0.5f, Mathf.PI, Mathf.PI * 1.5f
        };
        foreach (float theta in angles)
        {
            Vector2 translation = rotateVector(dV, theta);
            Vector2 newV = pV + translation;
            if (inBounds(newV))
			{
                sum += verts.GetHeight(newV);
                count++;
			}
		}
        if(count == 0)
		{
            return 0;
		}
        return sum / count;
	}
    Vector2 rotateVector(Vector2 v, float theta)
    {
        return new Vector2(Mathf.Round((v.x * Mathf.Cos(theta)) - (v.y * Mathf.Sin(theta))), 
            Mathf.Round((v.x * Mathf.Sin(theta)) + (v.y * Mathf.Cos(theta))));
    }
 ```
 
```c#
public class meshGenerator : MonoBehaviour
{
    //This function run once when Unity is in Play
     void Start ()
    {
      GenerateMesh(); 
    }
    
    void GenerateMesh()
    {
      .
      .
      .
    }
}
```

## Camera Motion

You can use images/gif by adding them to a folder in your repo:

<p align="center">
  <img src="Gifs/Q1-1.gif"  width="300" >
</p>

To create a gif from a video you can follow this [link](https://ezgif.com/video-to-gif/ezgif-6-55f4b3b086d4.mov).


## Vertex Shader

You can use emojis :+1: but do not over use it, we are looking for professional work. If you would not add them in your job, do not use them here! :shipit:

**Now Get ready to complete all the tasks:**

- [x] Read the handout for Project-1 carefully
- [x] Modelling of fractal landscape
- [x] Camera motion 
- [x] Surface properties
- [ ] Project organisation and documentation

## Median Filter

We decided to add an optional Median Filter to the terrain generation. This will increase computation time and loading time by approximately 1 second but results in a much smoother surface.
<p align="left">
  <img src="Gifs/nomedianfilter"  width="300" >
</p>
<p align="right">
  <img src="Gifs/medianfilter"  width="300" >
</p>

```c#
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
```
