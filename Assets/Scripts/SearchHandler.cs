using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class SearchHandler : MonoBehaviour
{
    public GameObject scrollViewContent;
    public TMP_InputField currentInputField;
    public GameObject searchInputField;
    public GameObject activeWordZone;
    public GameObject searchWordPrefab;
    List<string> searchWordList = new List<string>();
    
    public Sprite spriteFBOn;
    public Sprite spriteFBOff;
    public GameObject keywordScrollView;
    public GameObject keywordScrollViewContent;
    public GameObject keywordPrefab;
    GameObject activeFilterButton;

    private void Start()
    {
        keywordScrollView.SetActive(false);
    }

    
    public void FilterButtonOnClick(GameObject buttonObject)
    { 
        //if the same filter button is active
        if (activeFilterButton == buttonObject)
        {
            activeFilterButton.GetComponent<Image>().sprite = spriteFBOff;
            activeFilterButton = null;
            keywordScrollView.SetActive(false);
            foreach (Transform child in keywordScrollViewContent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
        else 
        {
            //if other filter button is active
            if (activeFilterButton != null)
            {
                activeFilterButton.GetComponent<Image>().sprite = spriteFBOff;
                keywordScrollView.SetActive(false);
                foreach (Transform child in keywordScrollViewContent.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
            activeFilterButton = buttonObject;
            activeFilterButton.GetComponent<Image>().sprite = spriteFBOn;
            keywordScrollView.SetActive(true);

            //전시물 데이터 불러오기 
            var collectionsDB = CreateExhibition.collectionsDB;
            var numofObjects = collectionsDB.Count;

            var allKeywords = new List<string>();

            //전시물들이 가진 모든 서브젝트를 키워드 리스트에 저장
            if (activeFilterButton.name == "Subject")
            {
                for (int i = 0; i < numofObjects; ++i)
                {
                    var exhibitionObject = collectionsDB[i].jsonInfo;

                    var subjects = exhibitionObject.subjects;

                    for (int j = 0; j < subjects.Count; j++)
                    {
                        var subject = subjects[j];
                        allKeywords.Add(subject);
                    }
                }
            }
            else if (activeFilterButton.name == "Material")
            {
                for (int i = 0; i < numofObjects; ++i)
                {
                    var exhibitionObject = collectionsDB[i].jsonInfo;

                    var subjects = exhibitionObject.materials;

                    for (int j = 0; j < subjects.Count; j++)
                    {
                        var subject = subjects[j];
                        allKeywords.Add(subject);
                    }
                }
            }
            else if (activeFilterButton.name == "Technique")
            {
                for (int i = 0; i < numofObjects; ++i)
                {
                    var exhibitionObject = collectionsDB[i].jsonInfo;

                    var subjects = exhibitionObject.techniques;

                    for (int j = 0; j < subjects.Count; j++)
                    {
                        var subject = subjects[j];
                        allKeywords.Add(subject);
                    }
                }
            }
            else if (activeFilterButton.name == "Artist")
            {
                for (int i = 0; i < numofObjects; ++i)
                {
                    var exhibitionObject = collectionsDB[i].jsonInfo;

                    var subjects = exhibitionObject.artists;

                    for (int j = 0; j < subjects.Count; j++)
                    {
                        var subject = subjects[j];
                        allKeywords.Add(subject);
                    }
                }
            }
            else if (activeFilterButton.name == "School Style")
            {
                for (int i = 0; i < numofObjects; ++i)
                {
                    var exhibitionObject = collectionsDB[i].jsonInfo;

                    var subjects = exhibitionObject.schoolStyle;

                    for (int j = 0; j < subjects.Count; j++)
                    {
                        var subject = subjects[j];
                        allKeywords.Add(subject);
                    }
                }
            }

            //중복되는 키워드 삭제
            var newAllKeywords = allKeywords.Distinct().ToList();

            //키워드 버튼 생성 
            for (int k = 0; k < newAllKeywords.Count; k++)
            {
                var keywordButton = Instantiate(keywordPrefab);

                keywordButton.name = k.ToString();
                keywordButton.transform.SetParent(keywordScrollViewContent.transform);

                var keyword = keywordButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                keyword.text = newAllKeywords[k];
                var keywordLayoutElements = keywordButton.transform.GetComponentInChildren<LayoutElement>();
                keywordLayoutElements.preferredWidth = keyword.preferredWidth + 20;

                keywordButton.GetComponent<Button>().onClick.AddListener(() => KeywordButtonOnClicked(keywordButton));

            }

        }


    }

    public void DestroyAndRenew(GameObject myparent, string removeWord)
    {
        searchWordList.Remove(removeWord);
        Destroy(myparent);
        RenewList();
    }
    public void RenewList()
    {
        // renew list
        var collectionsDB = CreateExhibition.collectionsDB;

        int nTotal = scrollViewContent.transform.childCount;

        int nWord = searchWordList.Count;

        for (int i = 0; i < nTotal; i++)
        {
            GameObject exhibitInList = scrollViewContent.transform.GetChild(i).gameObject;

            //exhibitInList.transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = searchInputField.GetComponent<TextMeshProUGUI>().text;
            if (nWord == 0)
            {
                exhibitInList.SetActive(true);
            }
            else
            {
                String allInfo = EveryText(collectionsDB[i].jsonInfo);
                //Debug.Log(allInfo);
                exhibitInList.SetActive(true);

                for (int j = 0; j < nWord; j++)
                {
                    string word = searchWordList[j];
                    if (!allInfo.Contains(word.ToLower()))
                    {
                        exhibitInList.SetActive(false);
                    }
                }

            }
        }

    }

    public void OnEnterButtonClicked()
    {
        // ActiveWord zone에 search word 추가
        String currentinput = searchInputField.GetComponent<TextMeshProUGUI>().text;

        currentinput = currentinput.Remove(currentinput.Length - 1);

        if (!String.IsNullOrEmpty(currentinput))
        {
            GameObject searchWord = Instantiate(searchWordPrefab);
            searchWord.transform.SetParent(activeWordZone.transform);
            searchWord.transform.GetChild(0).GetComponent<Text>().text = currentinput;

            var searchWordLayoutElements = searchWord.transform.GetComponent<LayoutElement>();
            searchWordLayoutElements.preferredWidth = searchWord.transform.GetChild(0).GetComponent<Text>().preferredWidth + 20;

            searchWord.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => DestroyAndRenew(searchWord,currentinput));
            searchWordList.Add(currentinput);

            // clear the input field
            currentInputField.Select();
            currentInputField.text = "";


        }

        RenewList();
    }

    public void KeywordButtonOnClicked(GameObject buttonObject)
    {
        // ActiveWord zone에 search word 추가
        String currentinput = buttonObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;

        Debug.Log(currentinput);

        if (!String.IsNullOrEmpty(currentinput))
        {
            GameObject searchWord = Instantiate(searchWordPrefab);
            searchWord.transform.SetParent(activeWordZone.transform);
            searchWord.transform.GetChild(0).GetComponent<Text>().text = currentinput;

            var searchWordLayoutElements = searchWord.transform.GetComponent<LayoutElement>();
            searchWordLayoutElements.preferredWidth = searchWord.transform.GetChild(0).GetComponent<Text>().preferredWidth + 20;

            searchWord.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => DestroyAndRenew(searchWord, currentinput));
            searchWordList.Add(currentinput);


        }

        RenewList();
    }

    String EveryText(ExhibitionObject exhibitionObject)
    {
        List<string> strlist = new List<string>();

        List<string> productionPlaceList = new List<string>();
        productionPlaceList.Add(exhibitionObject.productionPlace.country);
        productionPlaceList.Add(exhibitionObject.productionPlace.county);
        productionPlaceList.Add(exhibitionObject.productionPlace.state);
        productionPlaceList.Add(exhibitionObject.productionPlace.city);

        var aggregateMarterials = string.Join(", ", exhibitionObject.materials.Where(s => !String.IsNullOrEmpty(s)));
        var aggregateTechniques = string.Join(", ", exhibitionObject.techniques.Where(s => !String.IsNullOrEmpty(s)));
        var aggregateSubjects = string.Join(", ", exhibitionObject.subjects.Where(s => !String.IsNullOrEmpty(s)));
        var aggregateProductionPlace = string.Join(", ", productionPlaceList.Where(s => !String.IsNullOrEmpty(s)));
        var aggregateSchoolStyle = string.Join(", ", exhibitionObject.schoolStyle.Where(s => !String.IsNullOrEmpty(s)));
        var aggregateArtists = string.Join(", ", exhibitionObject.artists.Where(s => !String.IsNullOrEmpty(s)));

        strlist.Add(exhibitionObject.id);
        strlist.Add(exhibitionObject.title);
        strlist.Add(exhibitionObject.type);
        strlist.Add(exhibitionObject.dimensions);
        strlist.Add(aggregateMarterials);
        strlist.Add(aggregateTechniques);
        strlist.Add(aggregateSubjects);
        strlist.Add(aggregateArtists);
        strlist.Add(exhibitionObject.productionDate.objectDate);
        strlist.Add(aggregateProductionPlace);
        strlist.Add(aggregateSchoolStyle);

        var outstr = string.Join(", ", strlist.Where(s => !String.IsNullOrEmpty(s)));

        return outstr.ToLower();

    }
}
