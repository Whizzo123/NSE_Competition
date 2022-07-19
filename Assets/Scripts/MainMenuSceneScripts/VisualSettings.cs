using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualSettings : MonoBehaviour
{
    [SerializeField] [Tooltip("Render distance for obstacles and gems and particle effects and players.")] [Range(0, 1)] public float renderDistance;
    [SerializeField] [Tooltip("The quality level of textures.")] public int qualityLevel;

    public void RenderDistanceChange(float distance)
    {
        renderDistance = distance;
    }    
    public void QualityLevel(int quality)
    {
        qualityLevel = quality;
        UpdateVisualSettings();
    }


    public void UpdateVisualSettings()
    {
        QualitySettings.lodBias = (1 + (10 * renderDistance));
        QualitySettings.SetQualityLevel(qualityLevel, true);
    }
}
