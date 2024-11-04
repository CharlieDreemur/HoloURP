using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;

public class MaterialSetter : MonoBehaviour
{
    [SerializeField]
    public int eyesIndex;
    [SerializeField]
    private Material eyesMaterial;
    [SerializeField]
    public List<int> clotheseMaterialIndex;
    [SerializeField]
    private List<Material> clotheseMaterials;
    [SerializeField]
    private Color evilClothesColor;
    [SerializeField]
    private Color evilEyesColor;
    [SerializeField]
    private Color eyesEmissionColor;
    [SerializeField]
    private float eyesEmissionIntensity;
    [SerializeField]
    private Color originalColor;
    [SerializeField]
    private SkinnedMeshRenderer faceMeshRenderer;
    [SerializeField]
    private SkinnedMeshRenderer clothesMeshRenderer;
    [SerializeField]
    private float colorChangeDuration = 1.0f;
    public void Awake()
    {

        if (faceMeshRenderer != null && eyesIndex >= 0 && eyesIndex < faceMeshRenderer.materials.Length)
        {
            eyesMaterial = faceMeshRenderer.materials[eyesIndex];
            originalColor = eyesMaterial.GetColor("_BaseColor");
        }

        if (clothesMeshRenderer != null)
        {
            clotheseMaterials = new List<Material>();
            foreach (int index in clotheseMaterialIndex)
            {
                if (index >= 0 && index < clothesMeshRenderer.materials.Length)
                {
                    clotheseMaterials.Add(clothesMeshRenderer.materials[index]);
                }
            }
        }

        else
        {
            Debug.LogWarning("Material index is out of range or MeshRenderer not found.");
        }
    }

    [ContextMenu("BecomePurple")]
    public void Change2EvilColor()
    {
        // Animate the eyes base color
        eyesMaterial.DOColor(evilEyesColor, "_BaseColor", colorChangeDuration);
        float instnsity = Mathf.Pow(2.0f, eyesEmissionIntensity);
        Color emissonColor = new Color(eyesEmissionColor.r * instnsity, eyesEmissionColor.g * instnsity, eyesEmissionColor.b * instnsity);
        eyesMaterial.SetColor("_EmissionCol", emissonColor);

        // Animate the clothes materials
        for (int i = 0; i < clotheseMaterials.Count; i++)
        {
            clotheseMaterials[i].DOColor(evilClothesColor, "_BaseColor", colorChangeDuration);
        }
    }
}
