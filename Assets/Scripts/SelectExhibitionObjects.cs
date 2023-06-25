using UnityEngine;
using System.IO;
using TMPro;
using System.Collections.Generic;

public class SelectExhibitionObjects : MonoBehaviour
{
    [Header("Dropdown")]
    public TMP_Dropdown dropdown;
    private string placeHolder = "Choose json file";
    public List<string> fileNameList;
    public static string exhibitonTitle;

    // Start is called before the first frame update
    void Start()
    {
        SetDropdownOptions();
        GetComponent<Debugger>().init();
    }

    // Update is called once per frame
    void Update()
    {
        SelectButton();
    }

    private void SetDropdownOptions()
    {
        string AssetsFolderPath = Application.streamingAssetsPath;
        string JsonlFolder = AssetsFolderPath + "/DataJSON";

        DirectoryInfo dir = new DirectoryInfo(JsonlFolder);
        FileInfo[] info = dir.GetFiles("*.json");

        dropdown.options.Clear();

        dropdown.options.Insert(0, new TMP_Dropdown.OptionData(placeHolder));
        dropdown.captionText.text = placeHolder;

        fileNameList = new List<string>();

        foreach (var item in info)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            var fileName = Path.GetFileNameWithoutExtension(item.ToString());
            fileNameList.Add(fileName);
            option.text = (fileName.Length > 40) ? fileName.Substring(0, 40) + ".." : fileName;
            dropdown.options.Add(option);
        }
    }

    public void SelectButton()
    {
        if (dropdown.value > 0)
        {
            exhibitonTitle = fileNameList[dropdown.value - 1];
        }
    }
}
