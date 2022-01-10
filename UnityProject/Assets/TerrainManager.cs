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
    public int[] radius;
    Terrain terrain;
    float basePixelError = 1f;

    void Start()
    {
        terrain = GetComponent<Terrain>();
        terrain.terrainData = GetTerrainData(terrain.terrainData);
        Vector3 terrainSize = terrain.terrainData.size;
        terrainSize.y = terrainMaxHeight;
        terrain.terrainData.size = terrainSize;
        terrain.heightmapPixelError = basePixelError;
    }


    private void Update()
    {
        Vector2 mouse = Input.mousePosition;

        Vector2 mappedMouse = new Vector2(0, 0);

        mappedMouse.x = Map(Screen.width, 0, 0, xBase, mouse.x);
        mappedMouse.y = Map(0, Screen.height, 0, yBase, mouse.y);

        mappedMouse.x = Mathf.Clamp(mappedMouse.x, 0, xBase-1);
        mappedMouse.y = Mathf.Clamp(mappedMouse.y, 0, yBase-1);

        // check for all buttons, 
        // if they are clicked we set pixel error to 5 and start painting
        // if they are released we set pixel error to 1
        for (int i = 0; i < 3; i++)
        {
            // if pressed
            if (Input.GetMouseButton(i))
            {
                terrain.heightmapPixelError = basePixelError * 2;
                Paint(mappedMouse, radius[i]);
            }
            // if released
            if (Input.GetMouseButtonUp(i))
            {
                terrain.heightmapPixelError = basePixelError;
            }

        }

        // this will save the image to app/screenshots and clean the terrain
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveImage();
            CleanDisplay();
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

    private void Paint(Vector2 mappedMouse, int radius)
    {
        float[,] heights = terrain.terrainData.GetHeights(0, 0, xBase, yBase);

        List<Vector2> points = new List<Vector2>();

        // here we are doing a nested for loop because we need to access all the points
        // to measure the distance of each one to the origing (which is the current mouse pos)
        for (int x = 0; x < xBase - 1; x++)
        {
            for (int y = 0; y < yBase - 1; y++)
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

        terrain.terrainData.SetHeights(0, 0, heights);
    }


    private void CleanDisplay()
    {
        float[,] z = new float[xBase, yBase];

        for (int x = 0; x < xBase; x++)
        {
            for (int y = 0; y < yBase; y++)
            {
                z[x, y] = 1; //CalculateHeight(x,y); 
            }
        }
        terrain.terrainData.SetHeights(0, 0, z);
    }

    private void SaveImage()
    {
        Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture();
        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);

        System.IO.File.WriteAllBytes(string.Format("{0}/{1}{2}.png",
                              Application.dataPath,
                              "pic_",
                              System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")), bytes);
    }
}



