using System;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Vector2 offset;
    [SerializeField] private List<ParallaxLayer> parallaxLayers;

    [Serializable]
    private struct ParallaxLayer
    {
        public GameObject gameObject;
        public Vector2 scrollFactor;
    }

    private void Update()
    {
        foreach (ParallaxLayer layer in parallaxLayers)
        {
            layer.gameObject.transform.position = (Vector2.one - layer.scrollFactor) * mainCamera.transform.position + offset;
        }
    }
}
