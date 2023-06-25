using UnityEngine;
using System.IO;
using TMPro;
using System.Collections.Generic;

public class SelectSaveFile : MonoBehaviour
{
    [Header("Dropdown")]
    public TMP_Dropdown dropdown;
    private string placeHolder = "Select save file";
    public List<string> fileNameList;
    public static string saveFileName;

    // Start is called before the first frame update
    void Start()
    {
        SetDropdownOptions();
    }

    // Update is called once per frame
    void Update()
    {
        SelectButton();
    }

    private void SetDropdownOptions()
    {
        string saveFolder = Application.dataPath + "/savedata";

        DirectoryInfo dir = new DirectoryInfo(saveFolder);
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
            saveFileName = fileNameList[dropdown.value - 1];
        }
    }

    public void DeleteButton()
    {
        if (dropdown.value >0)
        {
            saveFileName = fileNameList[dropdown.value - 1];
            string saveFolder = Application.dataPath;
            string saveFilePath = saveFolder + "/savedata/" + saveFileName + ".json";
            File.Delete(saveFilePath);

            fileNameList.Remove(saveFileName);
            dropdown.options.RemoveAt(dropdown.value);

            dropdown.value = 0;
        }
    }
}
