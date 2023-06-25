using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEditor;
using System.Linq;
using System;
using UnityEngine.UI;

public class CreateExhibition : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public Text btnText;

    public static ExhibitionObjects exhibitionObjectsList;
    public static string exhibitionTitle;
    public static int exhibitionObjectCount;

    public static List<PaintingImg> collectionsDB = new List<PaintingImg>();
    //public PaintingImg testPaint;
    bool temp = true;
    bool loadComplete = false;
    public static bool loadFromSaveFile = false;


    public GameObject artworkPrefab;
    public GameObject ViewPanel;
    public GameObject scrollViewContent;

    public GameObject detailView;
    public GameObject detailViewContent;


    private void Start()
    {
        ViewPanel.SetActive(false);
        detailView.SetActive(false);

    }

    public void ButtonOnClick()
    {
        if (temp)
        {
            if (dropdown.value > 0 || loadFromSaveFile)
            {
                temp = false;
                btnText.text = "Loading...";

                CompleteExhibitionInformation();
            }
            else
            {
                Debug.Log("You Should Choose A Exhibiton!");
            }
        }
    }

    public void DrawObjectDetail(int objIndex)
    {
        detailView.SetActive(false);
        detailView.SetActive(true);

        Debug.Log(objIndex);
        var detailObject = collectionsDB[objIndex].jsonInfo;

        detailViewContent.transform.GetChild(0+1).GetComponent<Image>().sprite = collectionsDB[objIndex].resizedImg;
        detailViewContent.transform.GetChild(1 + 1).GetComponent<TextMeshProUGUI>().text = detailObject.title;
        detailViewContent.transform.GetChild(2 + 1).GetComponent<TextMeshProUGUI>().text = detailObject.productionDate.objectDate;

        var aggregateArtists = string.Join(", ", detailObject.artists.Where(s => !String.IsNullOrEmpty(s)));
        detailViewContent.transform.GetChild(3 + 1).GetComponent<TextMeshProUGUI>().text = aggregateArtists;

        detailViewContent.transform.GetChild(4 + 1).GetComponent<TextMeshProUGUI>().text = detailObject.description;

        List<string> productionPlaceList = new List<string>();
        productionPlaceList.Add(detailObject.productionPlace.country);
        productionPlaceList.Add(detailObject.productionPlace.county);
        productionPlaceList.Add(detailObject.productionPlace.state);
        productionPlaceList.Add(detailObject.productionPlace.city);

        var aggregateMarterials = string.Join(", ", detailObject.materials.Where(s => !String.IsNullOrEmpty(s)));
        var aggregateTechniques = string.Join(", ", detailObject.techniques.Where(s => !String.IsNullOrEmpty(s)));
        var aggregateSubjects = string.Join(", ", detailObject.subjects.Where(s => !String.IsNullOrEmpty(s)));
        var aggregateProductionPlace = string.Join(", ", productionPlaceList.Where(s => !String.IsNullOrEmpty(s)));
        var aggregateSchoolStyle = string.Join(", ", detailObject.schoolStyle.Where(s => !String.IsNullOrEmpty(s)));


        detailViewContent.transform.GetChild(6 + 1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = detailObject.id;
        detailViewContent.transform.GetChild(7 + 1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = detailObject.title;
        detailViewContent.transform.GetChild(8 + 1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = detailObject.type;
        detailViewContent.transform.GetChild(9 + 1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = detailObject.dimensions;
        detailViewContent.transform.GetChild(10 + 1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = aggregateMarterials;
        detailViewContent.transform.GetChild(11 + 1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = aggregateTechniques;
        detailViewContent.transform.GetChild(12 + 1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = aggregateSubjects;
        detailViewContent.transform.GetChild(13 + 1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = aggregateArtists;
        detailViewContent.transform.GetChild(14 + 1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = detailObject.productionDate.objectDate;
        detailViewContent.transform.GetChild(15 + 1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = aggregateProductionPlace;
        detailViewContent.transform.GetChild(16 + 1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = aggregateSchoolStyle;
       
    }
    public void DrawObjectList()
    {
        ViewPanel.SetActive(true);

        for (int i = 0; i < collectionsDB.Count; i++)
        {
            GameObject exhibitInList = Instantiate(artworkPrefab);
            exhibitInList.transform.SetParent(scrollViewContent.transform);

            exhibitInList.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = collectionsDB[i].resizedImg;

            var exhibitionObject = collectionsDB[i].jsonInfo;

            exhibitInList.transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = exhibitionObject.title;

            var aggregateArtists = string.Join(", ", exhibitionObject.artists.Where(s => !String.IsNullOrEmpty(s)));
            exhibitInList.transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = aggregateArtists;

            exhibitInList.transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = exhibitionObject.productionDate.objectDate;

            // image and title button
            int _i = i;
            exhibitInList.transform.GetChild(0).transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => DrawObjectDetail(_i));

        }
    }


    void Update()
    {
        //Debug.Log("collectionsDB.Count: " + collectionsDB.Count);
        if (loadComplete == false)
        {
            if(exhibitionObjectCount != 0 && collectionsDB.Count == exhibitionObjectCount)
            {
                loadComplete = true;
                btnText.text = "Collection Loaded";
                DrawObjectList();
            }
        }
            
    }

    public void CompleteExhibitionInformation() // 이미지, 프리팹을 미리 저장하지 않고 공통 artwork 프리팹을 인스턴스화 할때 이미지를 불러올것
    {
        //JSON 폴더 폴더 패스
        var JsonFolderPath = Application.streamingAssetsPath + "/DataJSON";

        //드롭다운에서 전시명 가져오기 
        exhibitionTitle = SelectExhibitionObjects.exhibitonTitle;
        var saveFilePath = JsonFolderPath + "/" + exhibitionTitle + ".json";

        //JSON 파일 읽어서 c#으로 디시리얼라이즈 하기
        string fileContents = File.ReadAllText(saveFilePath);
        exhibitionObjectsList = Newtonsoft.Json.JsonConvert.DeserializeObject<ExhibitionObjects>(fileContents);

        //전시물 개수
        exhibitionObjectCount = exhibitionObjectsList.exhibitionObjects.Count;

        //전시물 이미지 폴더 경로
        var imageFolderPath = Application.streamingAssetsPath + "/Sprite/Artworks/" + exhibitionTitle;
        DirectoryInfo di = new DirectoryInfo(imageFolderPath);  //Create Directoryinfo value by imageFolderPath  
        if (di.Exists == false)   //If New Folder not exits  
        {
            di.Create();             //create Folder  
        }

        for (int i=0; i<exhibitionObjectCount; i++)
        {
            PaintingImg m_paint = new PaintingImg();
            m_paint.jsonInfo = exhibitionObjectsList.exhibitionObjects[i];

            var spriteName = m_paint.jsonInfo.id;
            var spritePath = imageFolderPath + "/" + spriteName + ".png";
            var resizedSritePath = imageFolderPath + "/" + spriteName + "_resize.png";

            if (File.Exists(resizedSritePath) == true)
            {
                byte[] bytes = System.IO.File.ReadAllBytes(resizedSritePath);
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(bytes);
                m_paint.resizedImg = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                collectionsDB.Add(m_paint);

            }
            else
            {
                if (File.Exists(spritePath) == false)
                {
                    StartCoroutine(GetTextureRequest(m_paint.jsonInfo.primaryImage, (response) =>
                    {
                        m_paint.downloadedImg = response;

                        byte[] textureBytes = m_paint.downloadedImg.texture.EncodeToPNG();
                        File.WriteAllBytes(spritePath, textureBytes);

                        collectionsDB.Add(m_paint);
                        var resizedTexture = ScaleTexture(m_paint.downloadedImg.texture, Mathf.FloorToInt(m_paint.downloadedImg.texture.width * 336.0f / m_paint.downloadedImg.texture.height + 0.5f), 336);

                        byte[] restextureBytes = resizedTexture.EncodeToPNG();
                        File.WriteAllBytes(resizedSritePath, restextureBytes);
                        m_paint.resizedImg = Sprite.Create(resizedTexture, new Rect(0, 0, resizedTexture.width, resizedTexture.height), new Vector2(0.5f, 0.5f));

                    }));
                }
                else
                {
                    byte[] bytes = System.IO.File.ReadAllBytes(spritePath);
                    Texture2D texture = new Texture2D(1, 1);
                    texture.LoadImage(bytes);
                    m_paint.downloadedImg = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                    collectionsDB.Add(m_paint);
                    var resizedTexture = ScaleTexture(m_paint.downloadedImg.texture, Mathf.FloorToInt(m_paint.downloadedImg.texture.width * 336.0f / m_paint.downloadedImg.texture.height + 0.5f), 336);

                    byte[] textureBytes = resizedTexture.EncodeToPNG();
                    File.WriteAllBytes(resizedSritePath, textureBytes);
                    m_paint.resizedImg = Sprite.Create(resizedTexture, new Rect(0, 0, resizedTexture.width, resizedTexture.height), new Vector2(0.5f, 0.5f));
                }

            }


        }

    }

    Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }

    //전시물 이미지 파일 url에서 로드하기 
    IEnumerator GetTextureRequest(string url, System.Action<Sprite> callback)
    {
        using (var www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                if (www.isDone)
                {
                    var texture = DownloadHandlerTexture.GetContent(www);
                    var rect = new Rect(0, 0, texture.width, texture.height);
                    var sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
                    callback(sprite);
                }
            }
        }
    }


}