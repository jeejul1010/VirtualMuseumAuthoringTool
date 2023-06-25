using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;

public class SaveHandler : MonoBehaviour
{
    public GameObject savableWalls;
    public GameObject savablePaintings;
    public GameObject saveInputField;
    public TMP_Dropdown saveFileDropdown;
    public Button createButton;
    public GameObject paintingPrefab;
    public GameObject wallPrefab;
    public GameObject toggleButton;

    public void OnSaveButtonClick()
    {

        var saveFileName = saveInputField.GetComponent<TextMeshProUGUI>().text;
        if (saveFileName == null)
        {
            Debug.Log("Save Failed -- No file name");
            return;
        }

        string saveFolder = Application.dataPath + "/savedata";
        var saveFilePath = saveFolder + "/" + saveFileName + ".json";

        DirectoryInfo di = new DirectoryInfo(saveFolder);   
        if (di.Exists == false)   //If New Folder not exits  
        {
            di.Create();             //create Folder  
        }

        ExhibitionDataInfo data = new ExhibitionDataInfo();
        data.collectionjson = CreateExhibition.exhibitionTitle;
        data.hall = Hall.VisionHall; // ���߿� ���� �ɼ� ���� ����

        int nWalls = savableWalls.transform.childCount;
        for (int i =0; i<nWalls; i++)
        {
            data.walls.Add(new WallInfo(savableWalls.transform.GetChild(i)));
        }

        int nPaintings = savablePaintings.transform.childCount;
        for (int i = 0; i < nPaintings; i++)
        {
            data.paintings.Add(new PaintingInfo(savablePaintings.transform.GetChild(i)));
        }

        string json = JsonConvert.SerializeObject(data,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

        File.WriteAllText(saveFilePath, json);
        //ExhibitionDataInfo loadeddata = JsonConvert.DeserializeObject<ExhibitionDataInfo>(json);

    }

    public void OnLoadButtonClick()
    {
        if (saveFileDropdown.value > 0)
        {
            var saveFileName = SelectSaveFile.saveFileName;
            string saveFolder = Application.dataPath + "/savedata";
            var saveFilePath = saveFolder + "/" + saveFileName + ".json";
            string json = File.ReadAllText(saveFilePath);
            ExhibitionDataInfo loadeddata = JsonConvert.DeserializeObject<ExhibitionDataInfo>(json);


            SelectExhibitionObjects.exhibitonTitle = loadeddata.collectionjson;
            // halldropdown.value = loadeddata.hall;

            CreateExhibition.loadFromSaveFile = true;
            Button btn = createButton.GetComponent<Button>();
            btn.onClick.Invoke();

            //load walls
            int nWalls = loadeddata.walls.Count;
            for (int i =0; i<nWalls; i++)
            {
                var item = Instantiate(wallPrefab, loadeddata.walls[i].position, loadeddata.walls[i].rotation, savableWalls.transform);
                item.transform.localScale = loadeddata.walls[i].scale;
                item.GetComponent<WallPlacement>().ToggleButton = toggleButton;
                item.GetComponent<WallPlacement>().moving = false;
                item.layer = 8;

            }

            int nPaintings = loadeddata.paintings.Count;
            for (int i = 0; i < nPaintings; i++)
            {
                var item = Instantiate(paintingPrefab, loadeddata.paintings[i].position, loadeddata.paintings[i].rotation, savablePaintings.transform);
                item.transform.localScale = loadeddata.paintings[i].scale;
                item.GetComponent<ObjectPlacement>().moving = false;

            }

        }
        else
        {
            Debug.Log("You Should Choose A Save File!");
        }
    }
    class ExhibitionDataInfo
    {
        public string collectionjson;
        public Hall hall;
        public List<WallInfo> walls;
        public List<PaintingInfo> paintings;

        public ExhibitionDataInfo()
        {
            this.walls = new List<WallInfo>();
            this.paintings = new List<PaintingInfo>();
        }

    }
    class WallInfo
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public WallInfo()
        {

        }

        public WallInfo(Transform wall)
        {
            this.position = wall.position;
            this.rotation = wall.rotation;
            this.scale = wall.localScale;
        }
    }
    class PaintingInfo
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public int id; // ���߿� ������Ʈ

        public PaintingInfo()
        {

        }

        public PaintingInfo(Transform painting)
        {
            this.position = painting.position;
            this.rotation = painting.rotation;
            this.scale = painting.localScale;
        }
    }
    enum Hall
    {
        VisionHall,
    }

}
