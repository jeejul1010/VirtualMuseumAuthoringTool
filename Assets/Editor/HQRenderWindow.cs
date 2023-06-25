using UnityEditor;
using UnityEngine;

public class HQRender : EditorWindow
{
    static int width = 512;
    static int height = 512;

    [MenuItem("Window/High-quality Rendering (HQRender)")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        var window = GetWindow(typeof(HQRender));
        window.position = new Rect(0, 0, width, height);
    }

    Texture tex;

    void OnEnable()
    {
        tex = Resources.Load<RenderTexture>("RenderTexture");
        if (tex)
        {
            Debug.Log("[HQRenderWindow] RenderTexture loaded");
        }
    }

    void Update()
    {
        Repaint();
    }

    void OnGUI()
    {
        tex = OptixPlugin.m_renderTexture;
        if (tex)
        {
            EditorGUI.DrawPreviewTexture(new Rect(0, 0, width, height), tex);
        }
    }
}