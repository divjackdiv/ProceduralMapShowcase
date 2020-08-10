using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TreeEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [Header("Setup Settings")]
    public List<Material> m_materialsAffected;
    public List<NoiseMaterialEditor> m_editorsToReseed;
    public Coordinates m_mapRes;
    public float m_seed;
    public bool m_automaticallyReseed = true;
    public bool m_generateHeightMap;

    [Header("Map Settings")] 
    public float m_scale = 1f;
    public Gradient m_heightColors;
    public List<PNoiseLayer> m_noiseLayers;

    [Header("Biome Settings")]
    public bool m_useBiomes;
    public List<Biome> m_biomes;


    [Header("Input Settings")]
    public bool m_enableInput = true; 
    public float m_zoomSpeed = 1f;
    public float m_movementSpeed = 1f;

    Texture2D m_map;
    Texture2D m_heightMap;
    Vector2 m_currentOffset;
    Vector2 m_scaleOffset;
    float m_originalScale;
    public Coordinates m_originalRes;
    // Start is called before the first frame update
    void Start()
    {
        Random.InitState((int) (Time.realtimeSinceStartup));
        m_originalScale = m_scale;
        m_originalRes = new Coordinates();
        m_originalRes.x = m_mapRes.x;
        m_originalRes.y = m_mapRes.y;
        GenerateMap(m_mapRes.x, m_mapRes.y, m_currentOffset, m_scaleOffset, m_scale, m_seed);
    }

    public Vector2 offsetTest;
    public float test;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (m_automaticallyReseed)
            {
                m_seed = Random.value * 1000; 
            }
            m_currentOffset = Vector2.zero;
            GenerateMap(m_mapRes.x, m_mapRes.y, m_currentOffset, m_scaleOffset, m_scale, m_seed);
        }
        if (m_enableInput)
        {
            Vector2 offStart = m_currentOffset;
            bool regen = false;
            if (Input.mouseScrollDelta.y != 0)
            {
                m_scale -= m_zoomSpeed * Input.mouseScrollDelta.y; 
                m_scaleOffset.x = Input.mousePosition.x / Screen.width;
                m_scaleOffset.y = Input.mousePosition.y / Screen.height;
                regen = true;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                m_currentOffset.x += m_movementSpeed;
                regen = true;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                m_currentOffset.x -= m_movementSpeed;
                regen = true;
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                m_currentOffset.y += m_movementSpeed;
                regen = true;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                m_currentOffset.y -= m_movementSpeed;
                regen = true;
            }
            if(regen)
                GenerateMap(m_mapRes.x, m_mapRes.y, m_currentOffset, m_scaleOffset, m_scale, m_seed);
        }
    }
    
    void GenerateMap(int width, int height, Vector2 offset, Vector2 scaleOffset,  float scale, float seed)
    {
        for (int i = 0; i < m_editorsToReseed.Count; i++)
        {
            m_editorsToReseed[i].SetSeed(m_seed);
            m_editorsToReseed[i].SetScale((scale/ m_originalScale) * ((m_originalRes.x * 1f)/m_mapRes.x));
            m_editorsToReseed[i].SetRes(m_mapRes.x, m_mapRes.y);
        }
        m_map = new Texture2D(width, height);
        m_map.filterMode = FilterMode.Point;

        float[,] terrainHeights = new float[width, height]; 
        for (int nl = 0; nl < m_noiseLayers.Count; nl++)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    float x = ((offset.x + i * 1f) / width) * scale;
                    float y = ((offset.y + j * 1f) / height) * scale;
                    float rand = m_noiseLayers[nl].GetNoise(x + seed, y + seed);
                    terrainHeights[i, j] += rand;
                }
            } 
        }
        if (m_generateHeightMap)
        {
            m_heightMap = new Texture2D(width, height);
            m_heightMap.filterMode = FilterMode.Point;
            Color[] heightMapCols = new Color[width * height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Color col = Color.white * terrainHeights[i, j];
                    heightMapCols[j * width + i] = col;
                }
            }
            m_heightMap.SetPixels(heightMapCols);
            m_heightMap.Apply();
        }

        Color[] terrainColors = new Color[width * height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Color col = m_heightColors.Evaluate(terrainHeights[i, j]);
                if (m_useBiomes)
                {
                    Biome biome = null;
                    for (int b = 0; b < m_biomes.Count; b++)
                    {
                        if (m_biomes[b].heightGradientIndex < m_heightColors.colorKeys.Length)
                        {
                            GradientColorKey heightColorKey = m_heightColors.colorKeys[m_biomes[b].heightGradientIndex];
                            if (heightColorKey.color == col)
                                biome = m_biomes[b];
                        }
                    }
                    if (biome != null)
                    {
                        float newHeight = 0;
                        for (int nl = 0; nl < biome.noiseLayers.Count; nl++)
                        {
                            float x = ((offset.x + i * 1f) / width) * scale;
                            float y = ((offset.y + j * 1f) / height) * scale; 

                            float rand = biome.noiseLayers[nl].GetNoise(x + seed, y + seed);
                            newHeight += rand;
                        }
                        col = biome.biomeColors.Evaluate(Mathf.Lerp(newHeight, terrainHeights[i, j], biome.originalHeightStrength));
                    }
                }
                terrainColors[j * width + i] = col;
            }
        }

        m_map.SetPixels(terrainColors);
        m_map.Apply();
        for (int i = 0; i < m_materialsAffected.Count; i++)
        {
            m_materialsAffected[i].SetTexture("_MainTex", m_map);
            m_materialsAffected[i].SetTexture("_HeightMap", m_heightMap);
        }
    }     
}

[Serializable]
public class Coordinates
{
    public int x;
    public int y;
}

[Serializable]
public class PNoiseLayer
{
    public float frequency = 1f;
    public float amplitude = 1f;
    public Vector2 offset;

    public float GetNoise(float x, float y)
    {
        float noise = Mathf.PerlinNoise(x* frequency + offset.x, y * frequency + offset.y) * amplitude;
        return noise;
    }
}

[Serializable]
public class Biome
{
    public string name;
    public int heightGradientIndex;
    public float originalHeightStrength;
    public Gradient biomeColors;
    public List<PNoiseLayer> noiseLayers; 
}