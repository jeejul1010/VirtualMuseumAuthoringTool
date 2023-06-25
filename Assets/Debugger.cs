using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Debugger : MonoBehaviour
{
    public TMP_Dropdown dataDropdown;
    public Button createButton;

    // Start is called before the first frame update
    public void init()
    {
        dataDropdown.value = 1;
        dataDropdown.GetComponent<SelectExhibitionObjects>().SelectButton();

        createButton.onClick.Invoke();
    }
}
