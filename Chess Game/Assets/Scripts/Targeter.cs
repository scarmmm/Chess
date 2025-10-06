using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : MonoBehaviour
{
    public Renderer renderer;
    private Material originalMaterial;
    public Material highlightMaterial;

    void Start()
    {
        renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            originalMaterial = renderer.sharedMaterial;
        }
    }

    private void OnMouseEnter()
    {
        if (!enabled) return;

        if (renderer != null && highlightMaterial != null)
        {
            // Replace the material entirely
            renderer.material = highlightMaterial;
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

