using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TerrainManager : MonoBehaviour
{
    int xBase,yBase;
    [Range(1,5000)]
    public float scale;
    [Range(0, 0.2f)]
    public float Deepness;
    [Range(1, 10)]
    public float terrainMaxHeight;
    [Range(1, 32)]
    public int radius;
    Terrain terrain;
    void Start()
    {
        terrain = GetComponent<Terrain>();
        terrain.terrainData = GetTerrainData(terrain.terrainData);
        Vector3 terrainSize = terrain.terrainData.size;
        terrainSize.y = terrainMaxHeight;
        terrain.terrainData.size = terrainSize;
    }


    private void Update()
    {
        Vector2 mouse = Input.mousePosition;

        Vector2 mappedMouse = new Vector2(0, 0);

        mappedMouse.x = Map(Screen.width, 0, 0, xBase, mouse.x);
        mappedMouse.y = Map(0, Screen.height, 0, yBase, mouse.y);

        mappedMouse.x = Mathf.Clamp(mappedMouse.x, 0, xBase-1);
        mappedMouse.y = Mathf.Clamp(mappedMouse.y, 0, yBase-1);

        // this only happens when we press mouse left button
        if (Input.GetMouseButton(0))
        {
            Debug.Log(mappedMouse);

            float[,] heights = terrain.terrainData.GetHeights(0, 0, xBase, yBase);

            List<Vector2> points = new List<Vector2>();

            // here we are doing a nested for loop because we need to access all the points
            // to measure the distance of each one to the origing (which is the current mouse pos)
            for (int x = 0; x < xBase - 1; x++)
            {
                for (int y = 0;y < yBase -1; y++)
                {
                    Vector2 currentPoint = new Vector2(x, y);

                    if (Vector2.Distance(mappedMouse, currentPoint) < radius)
                    {
                        points.Add(currentPoint);
                    }
                }
            }

            for (int i = 0; i < points.Count; i++)
            {
                heights[(int)points[i].x, (int)points[i].y] -= Deepness;
            }
            
            terrain.terrainData.SetHeights(0,0,heights);
        }

    }

    TerrainData GetTerrainData(TerrainData terrainData )
    {
        xBase = terrainData.heightmapResolution;
        yBase = terrainData.heightmapResolution;

        terrainData.size = new Vector3(xBase, 1, yBase);
        Debug.Log(xBase + ", " + yBase);

        float[,] z = new float[xBase, yBase];

        for (int x = 0; x < xBase; x++)
        {
            for (int y = 0; y < yBase; y++)
            {
                z[x, y] = 1; //CalculateHeight(x,y); 
            }
        }
        terrainData.SetHeights(0, 0, z);
        return terrainData;
    }

    float CalculateHeight(float x,float y)
    {
        return Mathf.PerlinNoise(x/xBase * scale, y/yBase *scale);
    }

    public float Map(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
    {

        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

        return (NewValue);
    }

}
