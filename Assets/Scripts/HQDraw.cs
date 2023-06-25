using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HQDraw : MonoBehaviour
{
    RenderTexture tex;
    public OptixPlugin plugin;

    // Start is called before the first frame update
    void Start()
    {
    }

    void OnEnable()
    {
        plugin.Initialize();

        tex = OptixPlugin.m_renderTexture;

        Material mat = new Material(Shader.Find("UI/Default"));
        mat.mainTexture = tex;

        var imageComp = GetComponent<Image>();
        imageComp.material = mat;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
