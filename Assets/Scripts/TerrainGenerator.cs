using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainGenerator : MonoBehaviour
{
    public int width = 200;
    public int height = 160;

    public float terrainScale = 0.025f;
    public float temperatureScale = 0.015f;
    public float humidityScale = 0.025f;
    public float landThreshold = 0.05f;

    public int seed = 0;

    public Tilemap groundTilemap;
    public Tilemap treeTilemap;

    public TileBase water;
    public TileBase polar;
    public TileBase tundra;
    public TileBase taiga;
    public TileBase steps;
    public TileBase vpm;
    public TileBase vplm;
    public TileBase desert;
    public TileBase savana;
    public TileBase rainforest;

    public TileBase polarTrees;
    public TileBase tundraTrees;
    public TileBase taigaTrees;
    public TileBase steppeTrees;
    public TileBase forestTrees;
    public TileBase wetForestTrees;
    public TileBase desertTrees;
    public TileBase savannaTrees;
    public TileBase rainforestTrees;

    float[,] landmass;
    float[,] temperature;
    float[,] humidity;

    void Start()
    {
        Generate();
    }

    void Generate()
    {
        landmass = new float[width, height];
        temperature = new float[width, height];
        humidity = new float[width, height];

        float2 offset = new float2(seed, seed);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                landmass[x, y] = noise.snoise(
                    new float2(x * terrainScale, y * terrainScale) + offset
                );

                float tempNoise = noise.snoise(
                    new float2(x * temperatureScale, y * temperatureScale) + offset
                );

                temperature[x, y] = math.clamp(
                    0.65f + (tempNoise * 0.35f),
                    0f, 1f
                );

                float humidityNoise = noise.snoise(
                    new float2(x * humidityScale, y * humidityScale) + offset
                );

                humidity[x, y] = (humidityNoise + 1f) * 0.5f;
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                if (landmass[x, y] < landThreshold)
                {
                    groundTilemap.SetTile(pos, water);
                    continue;
                }

                TileBase ground;
                TileBase trees;

                if (temperature[x, y] < 0.33f)
                {
                    if (humidity[x, y] < 0.33f)
                    {
                        ground = polar;
                        trees = polarTrees;
                    }
                    else if (humidity[x, y] < 0.66f)
                    {
                        ground = tundra;
                        trees = tundraTrees;
                    }
                    else
                    {
                        ground = taiga;
                        trees = taigaTrees;
                    }
                }
                else if (temperature[x, y] < 0.66f)
                {
                    if (humidity[x, y] < 0.33f)
                    {
                        ground = steps;
                        trees = steppeTrees;
                    }
                    else if (humidity[x, y] < 0.66f)
                    {
                        ground = vpm;
                        trees = forestTrees;
                    }
                    else
                    {
                        ground = vplm;
                        trees = wetForestTrees;
                    }
                }
                else
                {
                    if (humidity[x, y] < 0.33f)
                    {
                        ground = desert;
                        trees = desertTrees;
                    }
                    else if (humidity[x, y] < 0.66f)
                    {
                        ground = savana;
                        trees = savannaTrees;
                    }
                    else
                    {
                        ground = rainforest;
                        trees = rainforestTrees;
                    }
                }

                groundTilemap.SetTile(pos, ground);

                float treeNoise = noise.snoise(
                    new float2(x * 0.31f, y * 0.31f) + offset
                );

                if (treeNoise > 0.25f && trees != null)
                {
                    treeTilemap.SetTile(pos, trees);
                }
            }
        }
    }
}