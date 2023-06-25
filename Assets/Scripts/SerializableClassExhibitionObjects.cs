using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class PaintingImg
{
    public ExhibitionObject jsonInfo;
    public Sprite downloadedImg;
    public Sprite resizedImg;
}


[Serializable]
public class ExhibitionObjects
{
    public List<ExhibitionObject> exhibitionObjects { get; set; }
}

[System.Serializable]
public class ExhibitionObject
{
    public string id { get; set; }
    public string title { get; set; }
    public string type { get; set; }
    public string description { get; set; }
    public List<ColorInfo> colors { get; set; }
    public List<string> materials { get; set; }
    public List<string> techniques { get; set; }
    public List<string> subjects { get; set; }
    public List<string> artists { get; set; }
    public DateInfo productionDate { get; set; }
    public PlaceInfo productionPlace { get; set; }
    public List<string> schoolStyle { get; set; }
    public string dimensions { get; set; }
    public string primaryImage { get; set; }
}

[Serializable]
public class ColorInfo
{
    public string originalHex { get; set; }
    public string normalizedHex { get; set; }
}

[Serializable]
public class DateInfo
{
    public string objectDate { get; set; }
    public int beginDate { get; set; }
    public int endDate { get; set; }
}

[Serializable]
public class PlaceInfo
{
    public string country { get; set; }
    public string county { get; set; }
    public string state { get; set; }
    public string city { get; set; }
}

public class SerializableClassExhibitionObjects : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
