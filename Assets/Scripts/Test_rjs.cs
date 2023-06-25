using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Test_rjs : MonoBehaviour
{
    public float lightIntensity = 10.0f;
    readonly float lightScaleFactor2 = 20.0f;
    // Start is called before the first frame update
    void Start()
    {
        GameObject light = new GameObject("AreaLight");
        
        Light lightComp = light.AddComponent<Light>();

        // lightComp.color = Color.blue;

        light.transform.position = new Vector3(0, 5, 0);

        var lightComponent = light.AddComponent<UniversalAdditionalLightData>();

        lightComp.type = LightType.Rectangle;
        lightComp.intensity = lightIntensity;
        lightComp.range = 3 * lightScaleFactor2;
        // lightComponent.SetLightTypeAndShape(UniversalLightTypeAndShape.RectangleArea);
        // lightComponent.SetIntensity(lightIntensity, LightUnit.Nits);
        // lightComponent.lightType = LightType.Rectangle;
        // lightComponent.intensity = lightIntensity;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
