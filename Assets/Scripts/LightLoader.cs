using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LightLoader : MonoBehaviour
{

    public GameObject hallPrefab;

    void Awake() {
        LoadLight();
    }

    public void LoadLight()
    {
        GameObject hall = Instantiate(hallPrefab);
        hall.layer = 8;
        int nChildObj = hall.transform.childCount;

        GameObject areaLightGenerator = new GameObject("AreaLightGenerator");
        AreaLightGenerator alg = areaLightGenerator.AddComponent<AreaLightGenerator>();
        // string path = AssetDatabase.GetAssetPath(hallPrefab);
        string lightFilename = "";
        alg.Init(hallPrefab.name, lightFilename);

        for (int i = 0; i < nChildObj; i++)
        {
            GameObject mChild = hall.transform.GetChild(i).gameObject;
            mChild.layer = 8;
            if (mChild.GetComponent<MeshCollider>() == null)
            {
                Mesh m_mesh = mChild.GetComponent<MeshFilter>().mesh;

                if (m_mesh != null)
                {

                    MeshCollider mMeshCollider = mChild.AddComponent<MeshCollider>();

                    //Debug.Log(string.Format("child {0} AddComponent mesh collider", mChild.name));

                    mMeshCollider.sharedMesh = m_mesh;

                    //Debug.Log(string.Format("child {0} sharedMesh set", mChild.name));
                }

            }
        }
    }
}
