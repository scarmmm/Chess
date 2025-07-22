using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : MonoBehaviour
{
    public Renderer renderer;
    private Material originalMaterial;
    private static Material unlitGreenMaterial;

    void Start()
    {
        renderer = GetComponent<Renderer>();

        // Cache the material reference itself, not an instance clone
        if (renderer != null)
        {
            originalMaterial = renderer.sharedMaterial;
        }

        // Create a shared unlit green material
        if (unlitGreenMaterial == null)
        {
            unlitGreenMaterial = new Material(Shader.Find("Unlit/Color"));
            unlitGreenMaterial.color = Color.green * .5f;
        }
    }

    private void OnMouseEnter()
    {
        if (!enabled) return;

        if (renderer != null)
        {
            // Replace the material entirely
            renderer.material = unlitGreenMaterial;
        }
    }

    private void OnMouseExit()
    {
        if (renderer != null && originalMaterial != null)
        {
            renderer.material = originalMaterial;
        }
    }
}

