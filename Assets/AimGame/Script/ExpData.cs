using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ExpData : MonoBehaviour
{
    public UserDetails uData;
    public List<ExpDetails> eDataList;

    private string filePath;
    // Use this for initialization
    void Start ()
    {
        uData     = new UserDetails();
        eDataList = new List<ExpDetails>();

    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void SetPlayerData(string pName,string pGender,string pHand)
    {
        uData.playerName   = pName;
        uData.playerGender = pGender;
        uData.playerHand   = pHand;
    }

    //string cName,int cRound,string tTaken,int err,string wSide,float d3d,float d2d
    public void AddTimingToList(string technique,string round,string values,string error,string side,string dist3D,string dist2D)
    {
      
        ExpDetails temp    = new ExpDetails(uData.playerName,technique, round,values,error,side,dist3D,dist2D);
        eDataList.Add(temp);
    }

    public void Purge(string technique, string round)
    {
        List<ExpDetails> temp = new List<ExpDetails>(eDataList);
        foreach (ExpDetails d in temp)
        {
            if (d.conditionName == technique && d.conditionRound == round)
            {
                eDataList.Remove(d);
            }
        }
    }

    public void ConvertToJasonAndSave()
    {
        string jsonString = "playerName\tconditionName\tconditionRound\ttimetaken\terrors\twhichSide\tdistance3D\tdistance2D  \n";
        
        ListContainer container = new ListContainer(eDataList);

        // TODO: Wrap this in try/catch to handle serialization exceptions
        //jsonString += JsonUtility.ToJson(uData);
        //jsonString += JsonUtility.ToJson(container);
        foreach (ExpDetails d in eDataList)
        {
            //jsonString += JsonUtility.ToJson(d);
            jsonString += d.playerName+"\t";
            jsonString += d.conditionName + "\t";
            jsonString += d.conditionRound + "\t";
            jsonString += d.timetaken + "\t";
            jsonString += d.errors + "\t";
            jsonString += d.whichSide + "\t";
            jsonString += d.distance3D + "\t";
            jsonString += d.distance2D + "\t";
            jsonString += "\n";
        }

        filePath = Application.persistentDataPath + "/Data";

        try
        {
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

        }
        catch (IOException ex)
        {
            Debug.Log(ex.Message);
        }

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            filePath = filePath + "/" + uData.playerName + ".txt";
        else
            filePath = filePath + "/" + uData.playerName + ".txt";

        Debug.Log("*************"+filePath);
        //filePath    = Application.persistentDataPath + "/"+ uData.playerName+".json";

        File.WriteAllText(filePath, jsonString);
    }

    
}

[System.Serializable]
public class UserDetails
{
    public string playerName;
    public string playerGender;
    public string playerHand;
}

[System.Serializable]
public class ExpDetails
{
    public string playerName;
    public string conditionName;
    public string conditionRound;
    public string timetaken;
    public string errors;
    public string whichSide;
    public string distance3D;
    public string distance2D;

    public ExpDetails(string pName, string cName, string cRound,string tTaken, string err,string wSide, string d3d, string d2d)
    {
        playerName     = pName;
        conditionName  = cName;
        conditionRound = cRound;
        timetaken      = tTaken;
        errors         = err;
        whichSide      = wSide;
        distance3D     = d3d;
        distance2D     = d2d;
    }
}

public struct ListContainer
{
    public List<ExpDetails> dataList;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="ExpDetails">Data list value</param>
    public ListContainer(List<ExpDetails> _dataList)
    {
        dataList = _dataList;
    }
}