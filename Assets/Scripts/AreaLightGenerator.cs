using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.IO;

public class AreaLightGenerator : MonoBehaviour
{
    public string placeName;
    GameObject place;

    // HA
    readonly float lightScaleFactor = 1.9f;
    readonly float lightIntensity = 10.0f;
    readonly Vector3 offset = new Vector3(0.0f, -0.01f, 0.0f);

    // GMA
    readonly float lightScaleFactor2 = 10.0f;
    readonly float lightIntensity2 = 20.0f;
    readonly Vector3 offset2 = new Vector3(0.0f, -0.3f, 0.0f);
    public readonly static string lightPrefix = "LightFixture";

    GameObject lightGroup;

    struct DiskLight
    {
        public Vector3 p;
        public float r;

        public override string ToString()
        {
            return p.ToString() + " " + r;
        }
    }

    GameObject createLightFromMesh(GameObject meshLight)
    {
        // 0. Disble original light mesh. We use fixture below instead.
        meshLight.SetActive(false);

        // 1. Create light fixture
        GameObject lightFixture = new GameObject();
        lightFixture.name = lightPrefix + "_" + meshLight.name;

        MeshRenderer meshRenderer = lightFixture.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Unlit/Color")); ///

        meshRenderer.sharedMaterial.color = new Color(1, 1, 1);
        
        // Do not render lights for gma
        if (meshLight.name.StartsWith("hqlight2"))
        {
            meshRenderer.enabled = false;
        }

        MeshFilter mf = lightFixture.AddComponent<MeshFilter>();
        mf.mesh = meshLight.GetComponent<MeshFilter>().mesh;

        // 2. Apply transform
        lightFixture.transform.position = meshLight.transform.position + offset;
        lightFixture.transform.rotation = meshLight.transform.rotation;

        // 3. Calculate transformed position of mesh
        Vector3 sumPos = Vector3.zero;
        foreach (var v in mf.mesh.vertices)
        {
            sumPos += v;
        }
        sumPos /= mf.mesh.vertices.Length;
        // Debug.Log("Center of mesh: " + sumPos);
        Vector3 meshPosition = meshLight.transform.TransformPoint(sumPos);
        // Debug.Log("Transformed: " + meshPosition);

        // 4. Calculate bound of mesh
        Bounds meshBound = new Bounds(meshPosition, Vector3.zero);
        foreach (var v in mf.mesh.vertices)
        {
            meshBound.Encapsulate(meshLight.transform.TransformPoint(v));
        }
        // Debug.Log("Mesh bounds: " + meshBound);

        // 5. Create light component at the position
        GameObject light = new GameObject("AreaLight");
        light.transform.position = meshPosition;
        light.transform.localRotation = Quaternion.Euler(90, 0, 0);
        light.transform.parent = lightFixture.transform;

        // 6. Configure light
        Light lightComp = light.AddComponent<Light>();
        var lightComponent = light.AddComponent<UniversalAdditionalLightData>();

        lightComp.type = LightType.Rectangle; //area light. rectangle shape
        lightComp.intensity = lightIntensity;


       // var lightComponent = light.AddComponent<UniversalAdditionalLightData>(); ///
       // lightComponent.SetLightTypeAndShape(UniversalLightTypeAndShape.RectangleArea); ///
       // lightComponent.SetIntensity(lightIntensity, LightUnit.Nits);
        Vector2 meshExtent = new Vector2(meshBound.extents.x, meshBound.extents.z);
        float maxExtent = Mathf.Max(meshBound.extents.x, meshBound.extents.z); //내가 추가한 코드
        if (meshLight.name.StartsWith("hqlight2"))
        {
           // lightComponent.SetAreaLightSize(meshExtent * lightScaleFactor2);
           lightComp.range = maxExtent * lightScaleFactor2;
        }
        else
        {
           // lightComponent.SetAreaLightSize(meshExtent * lightScaleFactor);
           lightComp.range = maxExtent * lightScaleFactor;
        }

        return lightFixture;
    }

    string ReadTxt(string filePath)
    {
        FileInfo fileInfo = new FileInfo(filePath);
        string value = "";

        if (fileInfo.Exists)
        {
            StreamReader reader = new StreamReader(filePath);
            value = reader.ReadToEnd();
            reader.Close();
        }
        else
        {
            Debug.LogError("No file:" + filePath);
            return null;
        }

        return value;
    }

    public void Init(string placeName = "ha.v0.34", string lightsFilename = null)
    {
        place = GameObject.Find(placeName);
        if (!place) // alternative name
            place = GameObject.Find(placeName + "(Clone)");

        if (place)
        {
            lightGroup = new GameObject("AreaLights");

            // Lights from text file
            if (lightsFilename != null && File.Exists(lightsFilename))
            {
                Debug.Log("Lights from file: " + lightsFilename);
                // Parse from text file
                string str = ReadTxt(lightsFilename);
                var points = str.Split('\n');
                List<DiskLight> diskLights = new List<DiskLight>();
                for (var i = 0; i < points.Length; ++i)
                {
                    if (string.IsNullOrWhiteSpace(points[i]))
                        continue;

                    var l = new DiskLight();
                    var arr = points[i].Split(' ');
                    l.p.x = float.Parse(arr[0]);
                    l.p.y = float.Parse(arr[1]);
                    l.p.z = float.Parse(arr[2]);
                    l.r = float.Parse(arr[3]);
                    l.p += offset2;
                    diskLights.Add(l);
                }
                foreach (DiskLight l in diskLights)
                {
                    // Make disk light
                    // 5. Create light component at the position
                    GameObject light = new GameObject("AreaLight");
                    light.transform.position = l.p;
                    light.transform.localRotation = Quaternion.Euler(90, 0, 0); // rotate rectangle
                    light.transform.parent = lightGroup.transform;

                    // 6. Configure light
                    Light lightComp = light.AddComponent<Light>();
                    var lightComponent = light.AddComponent<UniversalAdditionalLightData>();

                    lightComp.type = LightType.Rectangle; //area light. rectangle shape
                    lightComp.intensity = lightIntensity2;
                    lightComp.range = l.r * lightScaleFactor2;

                   // var lightComponent = light.AddComponent<UniversalAdditionalLightData>();
                   // lightComponent.SetLightTypeAndShape(UniversalLightTypeAndShape.RectangleArea);
                   // lightComponent.SetIntensity(lightIntensity2, LightUnit.Nits);
                  //  lightComponent.SetAreaLightSize(new Vector2(l.r, l.r) * lightScaleFactor2);
                }

                // Fit to rotated mesh
                lightGroup.transform.localRotation = Quaternion.Euler(0, 90, 0);
            }
            else
            {
                Debug.Log("Lights using mesh ");
                // Mesh lights
                for (var i = 0; i < place.transform.childCount; ++i)
                {
                    var child = place.transform.GetChild(i).gameObject;
                    // if (child.name[0] == 'g')
                    if (child.name.StartsWith("hqlight"))
                    {
                        GameObject l = createLightFromMesh(child);
                        l.transform.parent = lightGroup.transform;
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning(
                "[AreaLightGenerator.cs] Cannot find a place to generate lights: " + placeName
            );
        }
    }

    // Update is called once per frame
    void Update() { }
}
