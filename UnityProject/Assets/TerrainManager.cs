using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TerrainManager : MonoBehaviour
{
    int xBase,yBase;
    [Range(1,5000)]
    public float scale;

    void Start()
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GetTerrainData(terrain.terrainData);
        
    }

    TerrainData GetTerrainData(TerrainData terrainData )
    {
        xBase = terrainData.heightmapResolution - 1;
        yBase = terrainData.heightmapResolution - 1;

        terrainData.size = new Vector3(xBase, 1, yBase);
        Debug.Log(xBase + ", " + yBase);

        float[,] z = new float[xBase, yBase];

        for (int x = 0; x < xBase; x++)
        {
            for (int y = 0; y < yBase; y++)
            {
                z[x, y] = CalculateHeight(x,y); 
            }
        }
        terrainData.SetHeights(0, 0, z);
        return terrainData;
    }

    float CalculateHeight(float x,float y)
    {
        return Mathf.PerlinNoise(x/xBase *scale, y/yBase *scale);
    }

}
