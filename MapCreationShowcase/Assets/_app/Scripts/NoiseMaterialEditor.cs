using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class NoiseMaterialEditor : MonoBehaviour
{
    public List<PNoiseLayerShader> m_noiseLayers;
    public Material m_noiseMaterial;
    public Gradient m_colorGradient;


    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        UpdateMat();
    }
    void OnValidate()
    {
        UpdateMat();
    }

    void UpdateMat()
    { 
        if (m_noiseMaterial != null)
        {
            float[] speedArr = new float[m_noiseLayers.Count];
            float[] frequencyArr = new float[m_noiseLayers.Count];
            float[] amplitudeArr = new float[m_noiseLayers.Count];

            for(int i = 0; i < m_noiseLayers.Count; i++)
            {
                speedArr[i] = m_noiseLayers[i].speed;
                frequencyArr[i] = m_noiseLayers[i].frequency;
                amplitudeArr[i] = m_noiseLayers[i].amplitude;
            }
            m_noiseMaterial.SetInt("_FilledArrays", m_noiseLayers.Count);
            m_noiseMaterial.SetFloatArray("_SpeedArray", speedArr);
            m_noiseMaterial.SetFloatArray("_FrequencyArray", frequencyArr);
            m_noiseMaterial.SetFloatArray("_AmplitudeArray", amplitudeArr);

            Color[] colorGradientArr = new Color[m_colorGradient.colorKeys.Length];
            float[] colorKeysArr = new float[m_colorGradient.colorKeys.Length];

            for (int i = 0; i < m_colorGradient.colorKeys.Length; i++)
            {
                colorGradientArr[i] = m_colorGradient.colorKeys[i].color;
                colorKeysArr[i] = m_colorGradient.colorKeys[i].time;
            }
            m_noiseMaterial.SetInt("_GradientKeysCount", m_colorGradient.colorKeys.Length);
            m_noiseMaterial.SetColorArray("_GradientColorArr", colorGradientArr);
            m_noiseMaterial.SetFloatArray("_GradientKeysArr", colorKeysArr);

            float[] alphaKeysTimeArr = new float[m_colorGradient.alphaKeys.Length];
            float[] alphaKeysArr = new float[m_colorGradient.alphaKeys.Length];
            for (int i = 0; i < m_colorGradient.alphaKeys.Length; i++)
            {
                alphaKeysArr[i] = m_colorGradient.alphaKeys[i].alpha; 
                alphaKeysTimeArr[i] = m_colorGradient.alphaKeys[i].time;
            }
            m_noiseMaterial.SetInt("_GradientAlphaKeysCount", m_colorGradient.alphaKeys.Length);
            m_noiseMaterial.SetFloatArray("_AlphaKeysArr", alphaKeysArr);
            m_noiseMaterial.SetFloatArray("_AlphaKeysTimeArr", alphaKeysTimeArr); 
        }
    }
    public void SetSeed(float seed)
    {
        m_noiseMaterial.SetFloat("_Seed", seed);
    } 
    public void SetScale(float scale)
    { 
        m_noiseMaterial.SetFloat("_Scale", scale); 
    }

    public void SetRes(int x, int y)
    {
        m_noiseMaterial.SetInt("_ResolutionX", x);
        m_noiseMaterial.SetInt("_ResolutionY", y);
    }
}


[Serializable]
public class PNoiseLayerShader
{
    public float speed = 0f;
    public float frequency = 1f;
    public float amplitude = 1f;
}