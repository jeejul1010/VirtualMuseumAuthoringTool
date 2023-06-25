using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePainting : MonoBehaviour
{
    public GameObject paintingPrefab;

    public void OnButtonClicked()
    {
        Instantiate(paintingPrefab);
    }
}
