using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using System.Linq;

public static class P
{
#pragma warning disable CS0660
#pragma warning disable CS0661
    [StructLayout(LayoutKind.Sequential), System.Serializable]
    public struct HQCamera
    {
        public float fovy { get; set; }

        public float aspect { get; set; }

        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] position { get; set; }

        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] forward { get; set; }

        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] up { get; set; }

        public static bool operator ==(HQCamera c1, HQCamera c2)
        {
            return c1.fovy == c2.fovy
                && c1.aspect == c2.aspect
                && c1.position == c2.position
                && Enumerable.SequenceEqual(c1.forward, c2.forward)
                && Enumerable.SequenceEqual(c1.up, c2.up);
        }
        public static bool operator !=(HQCamera c1, HQCamera c2)
        {
            return c1.fovy != c2.fovy
                || c1.aspect != c2.aspect
                || c1.position != c2.position
                || !Enumerable.SequenceEqual(c1.forward, c2.forward)
                || !Enumerable.SequenceEqual(c1.up, c2.up);
        }
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct HQVertexAttribute
    {
        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public string name { get; set; }

        [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public string format { get; set; }

        public int dimension { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct HQMesh
    {
        public int vertexCount { get; set; }

        public IntPtr vertexBuffer { get; set; }

        public int attributeCount { get; set; }

        // Max 8 attributes (position, normal, color, uv1~4, tangent)
        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public HQVertexAttribute[] vertexAttrs { get; set; }

        public int indexCount { get; set; }

        public IntPtr indexBuffer { get; set; }

        [field: MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public float[] transform;

        public IntPtr texture { get; set; }

        public bool isLight { get; set; }
    }

    [StructLayout(LayoutKind.Sequential), System.Serializable]
    public struct HQParams
    {
        public HQCamera cam { get; set; } // Hide from inspector
        public int pixelSamples;
        public int shadowSamples;
        public int maxDepth;
        public bool accumulate;
        public UInt32 flag { get; set; } // Hide from inspector

        public static bool operator ==(HQParams c1, HQParams c2)
        {
            return c1.cam == c2.cam //
                && c1.pixelSamples == c2.pixelSamples //
                && c1.shadowSamples == c2.shadowSamples //
                && c1.maxDepth == c2.maxDepth //
                && c1.accumulate == c2.accumulate //
                && c1.flag == c2.flag;
        }
        public static bool operator !=(HQParams c1, HQParams c2)
        {
            return c1.cam != c2.cam //
                || c1.pixelSamples != c2.pixelSamples //
                || c1.shadowSamples != c2.shadowSamples //
                || c1.maxDepth != c2.maxDepth //
                || c1.accumulate != c2.accumulate //
                || c1.flag != c2.flag;
        }
    }

    [DllImport("hq-renderer", EntryPoint = "Init")]
    public static extern void Init(IntPtr texture, int numObjects, HQMesh[] meshArr);

    [DllImport("hq-renderer", EntryPoint = "Render")]
    public static extern float Render();

    [DllImport("hq-renderer", EntryPoint = "SetParams")]
    public static extern void SetParams(HQParams param);

    [DllImport("hq-renderer", EntryPoint = "Release")]
    public static extern void Release();
}

public class OptixPlugin : MonoBehaviour
{
    Texture m_texture;
    static public RenderTexture m_renderTexture;
    Renderer m_Renderer;
    Camera m_mainCamera;
    [SerializeField]
    private Text _fpsNum;
    private int m_captureCounter = 0;
    private bool m_capture = true;

    public bool takeScreenshotWhenInit = false;
    public P.HQParams m_params = new P.HQParams();
    private P.HQParams m_prevParams;
    bool isInitialized = false;

    [System.Serializable]
    public class ControlFlag
    {
        public bool TraceShadow;
        public bool TraceSecondary;
        public bool ShadingNormal;
        public bool GeometricNormal;

        public ControlFlag()
        {
            TraceShadow = false;
            TraceSecondary = true;
            ShadingNormal = false;
            GeometricNormal = false;
        }

        public UInt32 toNumber()
        {
            UInt32 a = 0;
            if (TraceShadow)
                a = a | ((UInt32)1 << 0);
            if (TraceSecondary)
                a = a | ((UInt32)1 << 1);
            if (ShadingNormal)
                a = a | ((UInt32)1 << 2);
            if (GeometricNormal)
                a = a | ((UInt32)1 << 3);
            return a;
        }
    };
    public ControlFlag flagControl = new ControlFlag();

    P.HQMesh ConvertUnityMeshToNativeMesh(MeshFilter mf)
    {
        P.HQMesh mesh = new P.HQMesh();

        // 0. Check if any MeshFilter has more than one vertexBuffer
        Debug.Assert(mf.mesh.vertexBufferCount == 1);

        // 1. Vertex buffer
        mesh.vertexCount = mf.mesh.vertexCount;
        mesh.vertexBuffer = mf.mesh.GetNativeVertexBufferPtr(0);

        // 2. Attributes
        mesh.attributeCount = mf.mesh.vertexAttributeCount + 1;
        mesh.vertexAttrs = new P.HQVertexAttribute[mesh.attributeCount];
        for (var i = 0; i < mf.mesh.vertexAttributeCount; ++i)
        {
            // Debug.Log("attr: " + mf.mesh.GetVertexAttribute(i));
            mesh.vertexAttrs[i].name = mf.mesh.GetVertexAttribute(i).attribute.ToString();
            mesh.vertexAttrs[i].format = mf.mesh.GetVertexAttribute(i).format.ToString();
            mesh.vertexAttrs[i].dimension = mf.mesh.GetVertexAttribute(i).dimension;
        }
        mesh.vertexAttrs[mesh.vertexAttrs.Length - 1].name = "Index";
        mesh.vertexAttrs[mesh.vertexAttrs.Length - 1].format = mf.mesh.indexFormat.ToString();
        mesh.vertexAttrs[mesh.vertexAttrs.Length - 1].dimension = 3;

        // 3. Index buffer
        mesh.indexCount = 0;
        for (var i = 0; i < mf.mesh.subMeshCount; ++i)
        {
            mesh.indexCount += (int)mf.mesh.GetIndexCount(i); // Sum total index count of all submesh
        }
        mesh.indexBuffer = mf.mesh.GetNativeIndexBufferPtr();

        // 4. Transform
        // Debug.Log(mf.gameObject.transform.localToWorldMatrix.ToString());
        var mat = mf.gameObject.transform.localToWorldMatrix;
        int cnt = 0;
        mesh.transform = new float[12];
        for (var column = 0; column < 4; ++column)
        {
            for (var row = 0; row < 3; ++row)
            {
                mesh.transform[cnt++] = mat[row, column];
            }
        }

        // 5. Texture
        Renderer renderer = mf.gameObject.GetComponent<Renderer>();
        Material material = renderer.material;
        if (material.HasProperty("_MainTex") && material.mainTexture)
        {
            // Debug.Log("mainTexture: " + material.mainTexture.graphicsFormat.ToString());
            mesh.texture = material.mainTexture.GetNativeTexturePtr();
            // foreach (var n in material.GetTexturePropertyNames())
            // {
            //     if (material.GetTexture(n))
            //         Debug.Log("[Property]" + n + ": " + material.GetTexture(n).name);
            // }
            // Debug.Log(material.mainTexture.name + ": " + mesh.texture);
        }
        else
        {
            // Debug.Log("Doesnt have texture");
            mesh.texture = IntPtr.Zero;
        }

        // 6. Check whether it's light
        if (mf.name.StartsWith("hqlight"))
        {
            mesh.isLight = true;
        }
        else
        {
            mesh.isLight = false;
        }

        return mesh;
    }

    P.HQCamera GetCameraInfo()
    {
        P.HQCamera cam = new P.HQCamera();
        cam.fovy = m_mainCamera.fieldOfView;
        cam.aspect = m_mainCamera.aspect;
        cam.position = new float[3];
        cam.forward = new float[3];
        cam.up = new float[3];
        for (var i = 0; i < 3; ++i)
        {
            cam.position[i] = m_mainCamera.transform.position[i];
            cam.forward[i] = m_mainCamera.transform.forward[i];
            cam.up[i] = m_mainCamera.transform.up[i];
        }
        return cam;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void Initialize()
    {
        m_captureCounter = 0;
        m_capture = true;
        if (isInitialized)
        {
            Debug.Log("Optix already initialized.");
            return;
        }
        else
            Debug.Log("Initialize Optix.");


        /////////////////////////////////////////////////
        // Set output texture for OptiX. Output frame buffuer size, pixel format and etc. are determined by this texture
        // m_texture = Resources.Load<Texture2D>("banana");
        m_texture = new Texture2D(Screen.width, Screen.height, DefaultFormat.LDR, TextureCreationFlags.None);

        // m_renderTexture = Resources.Load<RenderTexture>("RenderTexture");
        m_renderTexture = new RenderTexture(Screen.width, Screen.height, 0, GraphicsFormat.R8G8B8A8_UNorm, 0);

        /////////////////////////////////////////////////
        // Get camera info
        m_mainCamera = Camera.main;

        /////////////////////////////////////////////////
        // [ Load all objects and convert ] ////////////
        object[] objs = GameObject.FindObjectsOfType(typeof(MeshFilter));
        // Exclude player object
        var filtered = objs.Cast<MeshFilter>().Where(val => val.name != "Player");
        filtered = filtered.Where(val => !val.name.StartsWith(AreaLightGenerator.lightPrefix));
        var meshArr = filtered.ToArray();
        var meshList = new List<P.HQMesh>();

        foreach (var mf in meshArr)
        {
            Debug.Log("Object name: " + mf.name);
            // Renderer renderer = mf.gameObject.GetComponent<Renderer>();
            // Material material = renderer.material;
            // Graphics.CopyTexture(material.mainTexture, 0, 0, 0, 0, 512, 512, m_renderTexture, 0, 0, 0, 0);

            var mesh = ConvertUnityMeshToNativeMesh(mf);
            meshList.Add(mesh);
        }

        /////////////////////////////////////////////////
        // Init optix
        P.Init(m_texture.GetNativeTexturePtr(), meshList.Count, meshList.ToArray());
        isInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isInitialized)
        {
            if (PlayerController.isModified || m_prevParams != m_params || m_params.flag != flagControl.toNumber())
            {
                Debug.Log("Params (camera, flags, etc.) are changed");
                PlayerController.isModified = false;
                m_params.cam = GetCameraInfo();
                m_params.flag = flagControl.toNumber();

                P.SetParams(m_params);
                m_prevParams = m_params;
            }

            // Render
            float time = P.Render(); // time in milliseconds
            int fps = (int)Math.Round(1 / time);
            fps = Math.Min(fps, 99);
            _fpsNum.text = fps.ToString("D2");

            // Write the result to the texture
            Graphics.CopyTexture(m_texture, m_renderTexture);

            if (takeScreenshotWhenInit)
                if (m_capture && m_captureCounter++ == 1)
                {
                    string folderPath = "Assets/Screenshots/"; // the path of your project folder
                    if (!System.IO.Directory.Exists(folderPath)) // if this path does not exist yet
                        System.IO.Directory.CreateDirectory(folderPath);  // it will get created
                    var screenshotName =
                                            "Screenshot_" +
                                            System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + // puts the current time right into the screenshot name
                                            ".png"; // put youre favorite data format here
                    ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, screenshotName), 1); // takes the sceenshot, the "2" is for the scaled resolution, you can put this to 600 but it will take really long to scale the image up
                    Debug.Log(folderPath + screenshotName); // You get instant feedback in the console
                    m_capture = false;
                }
        }
    }

    void OnDestroy()
    {
        // Debug.Log("Called OnDestroy");
        // P.Release();
    }
}
