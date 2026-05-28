using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GetMainLightDirection : MonoBehaviour
{

    [SerializeField] private Material skyboxMaterial;
    private static readonly int MainLightDirection = Shader.PropertyToID("_MainLightDirection");

    private void Update()
    {
        skyboxMaterial.SetVector(MainLightDirection, transform.forward);
    }
}