using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MenuController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_Object;

    
    public void changeButton()
    {
        m_Object.text = "All Loaded!";
    }
}
