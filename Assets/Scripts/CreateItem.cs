using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateItem : MonoBehaviour
{
    public GameObject wallParent;
    public GameObject paintingParent;
    public GameObject paintingPrefab;
    public GameObject wallPrefab;

    public void OnPaintingButtonClicked()
    {
        var item = Instantiate(paintingPrefab, paintingParent.transform);
    }

    public void OnWallButtonClicked(GameObject ToggleButton)
    {
        var wall = Instantiate(wallPrefab, wallParent.transform);
        wall.GetComponent<WallPlacement>().ToggleButton = ToggleButton;
    }
}
