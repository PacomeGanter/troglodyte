using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TerrainManager : MonoBehaviour
{
    int xBase,yBase;
    [Range(0, 0.2f)]
    public float DigForce;
    public int baseSize = 256;
    [Range(1, 50)]
    public int terrainMaxHeight;
    [Range(1, 32)]
    public int[] radius;
    Terrain terrain;
    float basePixelError = 1f;
    float cameraY = 0;
    public Material terrainMaterial;

    Vector3 HandPos = Vector3.zero;
    void Awake()
    {
        terrain = GetComponent<Terrain>();
        terrain.terrainData = new TerrainData();
        terrain.terrainData.heightmapResolution = baseSize;
        terrain.terrainData.SetDetailResolution(baseSize,32);
        terrain.terrainData.baseMapResolution = baseSize;
        terrain.terrainData.alphamapResolution = baseSize;
        Vector3 terrainSize = new Vector3(baseSize, terrainMaxHeight, baseSize);
        terrain.terrainData.size = terrainSize;
        terrainMaterial.SetFloat(Shader.PropertyToID("_TerrainHeight"), terrainMaxHeight);
        Transform camera = transform.GetChild(0);
        camera.position = new Vector3(baseSize * 0.5f, baseSize - baseSize*0.1f, baseSize * 0.5f);
        GetTerrainData(terrain.terrainData);
    }


    private void Update()
    {
        Vector2 mappedValues = new Vector2(0, 0);

        if (HandPos == Vector3.zero)
        {
            Vector2 mouse = Input.mousePosition;
            mappedValues.x = Map(Screen.width, 0, 0, xBase, mouse.x);
            mappedValues.y = Map(0, Screen.height, 0, yBase, mouse.y);

            mappedValues.x = Mathf.Clamp(mappedValues.x, 0, xBase - 1);
            mappedValues.y = Mathf.Clamp(mappedValues.y, 0, yBase - 1);
        }
        else
        {
            mappedValues = new Vector2(HandPos.x, HandPos.y);
            HandPos = Vector3.zero;
        }

        // check for all buttons, 
        // if they are clicked we set pixel error to 5 and start painting
        // if they are released we set pixel error to 1
        for (int i = 0; i < 3; i++)
        {
            // if pressed
            if (Input.GetMouseButton(i))
            {
                terrain.heightmapPixelError = basePixelError * 4;
                Paint(mappedValues, radius[i]);
                Debug.Log(mappedValues);
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
            StartCoroutine(SaveImage());
        }

    }

    void GetTerrainData(TerrainData terrainData )
    {
        xBase = terrainData.heightmapResolution;
        yBase = terrainData.heightmapResolution;

        terrainData.size = new Vector3(xBase, terrainMaxHeight, yBase);
        Debug.Log(xBase + ", " + yBase);

        float[,] z = new float[xBase, yBase];

        for (int x = 0; x < xBase; x++)
        {
            for (int y = 0; y < yBase; y++)
            {
                z[x, y] = 1;
            }
        }
        terrainData.SetHeights(0, 0, z);
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
            heights[(int)points[i].x, (int)points[i].y] -= DigForce;
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

    IEnumerator SaveImage()
    {
        yield return new WaitForEndOfFrame();
        terrain.heightmapPixelError = 1;
        Camera Cam = GetComponentInChildren<Camera>();
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture newRT = new RenderTexture(Screen.width,Screen.height, 16);
        Cam.targetTexture = newRT;

        Cam.Render();

        Texture2D Image = new Texture2D(Cam.targetTexture.width, Cam.targetTexture.height); 
        Image.ReadPixels(new Rect(0, 0, Cam.targetTexture.width, Cam.targetTexture.height), 0, 0);
        Image.Apply();
        Cam.targetTexture = currentRT;

        var Bytes = Image.EncodeToPNG();
        Destroy(Image);

        System.IO.File.WriteAllBytes(Application.dataPath + "/Pics/" + "Pic_" +
                                     System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + 
                                     ".png", 
                                     Bytes);
        terrain.heightmapPixelError = basePixelError;
        //CleanDisplay();
    }


    public void ReceiveHandPosition(in Vector3 pos)
    {
        HandPos = pos;
        Debug.Log(HandPos);
    }
}



