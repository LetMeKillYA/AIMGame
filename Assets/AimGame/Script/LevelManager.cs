using GoogleARCore;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    
    [SerializeField]
    protected bool       linearSpawn = true;
    [SerializeField]
    private Transform    levelSet;
    [SerializeField]
    private GameObject   targetObj;
    [SerializeField]
    private List<Target> targets;
    [SerializeField]
    private float        interval;
    [SerializeField]
    private int    noTargets = 10;
    private int    targetNo  = 0;
    public  Target currentTarget;
    private Target previousTarget;
    private float  targetTimer;
    private List<TargetBlock> blockList;
    // Use this for initialization
    void Start ()
    {
        ang      = 0;
        levelSet = transform;
        //SetAnchor();
	}

    void SetAnchor()
    {
        Pose   tempP     = new Pose(transform.position, transform.rotation);
        Anchor tempA     = Session.CreateAnchor(tempP);
        transform.parent = tempA.transform;
    }

    public void SetTargets()
    {
        targets = new List<Target>();

        CreateTargetsFromFile();

        /*for (int i = 0; i < noTargets; i++)
        {
            GameObject temp              = GameObject.Instantiate(targetObj);
            temp.transform.localScale    = RandomScale();
            temp.transform.parent        = transform;
            temp.transform.position      = Vector3.zero;
            temp.transform.localPosition = RandomCircle(GameControl.GetInstance().PlayerPos(), 2);

            targets.Add(temp.GetComponent<Target>());
        }*/
       
        GameControl.GetInstance().GetTargetList(targets);
        MenuManager.GetInstance().ResetUIColliders();
       
    }

  

    public static void WriteDataToFile(string jsonString)
    {
        int round = MenuManager.GetInstance().modularNo/4;
        string path = Application.persistentDataPath+"\\" +"level"+round+".txt";
        Debug.Log("AssetPath:" + path);
        File.WriteAllText(path, jsonString);
        #if UNITY_EDITOR
         UnityEditor.AssetDatabase.Refresh();
        #endif
    }

    void SaveTargets()
    {
        string outPut = "";
        for (int i = 0; i < targets.Count; i++)
        {
            outPut += targets[i].myBlock.SaveToString();
            outPut += "\n";
            
        }

        WriteDataToFile(outPut);
    }

    int id = 0;
    void CreateTargetsFromFile()
    {
        id = 0;
        int moduleNo = MenuManager.GetInstance().modularNo;
        int round = MenuManager.GetInstance().playerId + moduleNo / 5;
        string path = Application.persistentDataPath;

        if (moduleNo < 0)
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                path = path + "/" + "training.txt";
            else
                path = path + "\\" + "training.txt";
        }
        else
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                path = path + "/" + "level" + round % 4 + "-" + moduleNo % 4 + ".txt";
            else
                path = path + "\\" + "level" + round % 4 + "-" + moduleNo % 4 + ".txt";
        }
       

        Debug.Log("AssetPath:" + path);
        StreamReader reader = new StreamReader(path);
        blockList = new List<TargetBlock>();

        while (!reader.EndOfStream)
        {
            string jSon = reader.ReadLine();
            Debug.Log(jSon);
            GameObject  temp    = GameObject.Instantiate(targetObj);
            Target tTarget      = temp.GetComponent<Target>();
            TargetBlock tBlock  = tTarget.myBlock;

            tBlock = CreateFromJSON(jSon);
            blockList.Add(tBlock);


            temp.name = "TargetId" + id;
            targets.Add(temp.GetComponent<Target>());
            id++;
        }

        if (!linearSpawn)
        {
            //ShuffleList(blockList);
            //ShuffleList(targets);
        }
           

        for(int i =0;i< targets.Count;i++)
        {
            targets[i].transform.parent          = transform;
            targets[i].transform.localScale      = blockList[i].targetScale;
            targets[i].transform.localPosition   = blockList[i].targetPosition;
            targets[i].transform.localRotation   = Quaternion.Euler(blockList[i].targetRotation);
            targets[i].isValid                   = blockList[i].isTarget;
            targets[i].radius                    = blockList[i].targetRange;
            targets[i].blocker.SetActive(blockList[i].hasBlocker);
            
        }
        ShuffleList(targets);
        reader.Close();
    }

    public TargetBlock CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<TargetBlock>(jsonString);
    }

    public void Initiate()
    {
        SetTargets();
        //For Testing
        NextTarget();
        
        previousTarget = null;

        GameControl.GetInstance().ActivateTargetObects();
        
    }

    void OnEnable()
    {
        EventManager.OnTargetHit += NextTarget;

        Initiate();
    }

    private void OnDisable()
    {
        //SaveTargets();
        ang = 0;
        targetNo = 0;
        targets = new List<Target>();
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        EventManager.OnTargetHit -= NextTarget;

        
    }
    // Update is called once per frame
    void Update ()
    {
        targetTimer += Time.deltaTime; 
    }

    Vector3 RandomScale()
    {
        Vector3 randScale = Vector3.zero;
        float randOffset  = Random.value;
        float multiple = randOffset * 100;
        while (multiple <= 10 || multiple > 35)
        {
            randOffset = Random.value;
            multiple = randOffset * 100;
        }
        randScale = new Vector3(randOffset, randOffset, randOffset);
        return randScale;
    }

    float ang;
    Vector3 RandomCircle(Vector3 center, float radius)
    {
        float randOffset = Random.value * 2;
        ang += 360f / noTargets;// Random.value * 360;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        randOffset = randOffset > 1 ? (randOffset * 0.5f * -1f) : randOffset;
        pos.y = center.y + randOffset;
        return pos;
    }

    void NextTarget()
    {
        StartCoroutine(CallNextTarget());
    }
    private static System.Random rng = new System.Random();
    
    public void ShuffleList(List<Target> alpha)
    {
        for (int i = 0; i < alpha.Count; i++)
        {
            Target temp = alpha[i];
            int randomIndex = Random.Range(i, alpha.Count);
            alpha[i] = alpha[randomIndex];
            alpha[randomIndex] = temp;
        }
    }

    public void ShuffleList(List<TargetBlock> alpha)
    {
        for (int i = 0; i < alpha.Count; i++)
        {
            TargetBlock temp = alpha[i];
            int randomIndex = Random.Range(i, alpha.Count);
            alpha[i] = alpha[randomIndex];
            alpha[randomIndex] = temp;
        }
    }

    IEnumerator CallNextTarget()
    {
       
        yield return new WaitForSeconds(interval);

        previousTarget = currentTarget;
           

        if (targetNo < targets.Count)
        {
            currentTarget = targets[targetNo];
            currentTarget.TargetSpawn();
        }
        //string cName,int cRound,string tTaken,int err,string wSide,float d3d,float d2d
        int round = MenuManager.GetInstance().modularNo;
        if (targetNo > 1 && round > 0)
        {
            Vector3 fwd = previousTarget.transform.forward;
            Vector3 diff = currentTarget.transform.position - previousTarget.transform.position;
            string dir = AngleDir(fwd, diff, Vector3.up);
            string aimType = GameControl.GetInstance().aimAssistType.ToString();
           
            string error = GameControl.GetInstance().clickError.ToString();
            string d3d = ""+Vector3.Distance(previousTarget.transform.position, currentTarget.transform.position);
            string d2d = "" + Vector2.Distance(previousTarget.center, currentTarget.center);

            
            string supportTxt = " ";
            if (round % 4 == 1)
                supportTxt = "Table";
            else if (round % 4 == 2)
                supportTxt = "Stationary";
            else if (round % 4 == 3)
                supportTxt = "UI";
            else if (round % 4 == 0)
                supportTxt = "Moving";

            MenuManager.GetInstance().SetTimingData(aimType, supportTxt, targetTimer.ToString(), error, dir,d3d,d2d);
            GameControl.GetInstance().clickError = 0;
        }
            

        targetNo++;
        GameControl.GetInstance().CheckTimer();
        targetTimer = 0;
#if UNITY_EDITOR
        if (targetNo > targets.Count)
        {
            MenuManager.GetInstance().modularNo++;
            targetNo = 0;
            EventManager.CallSetComplete();
        }
#else
        if (targetNo > targets.Count)
        {
            MenuManager.GetInstance().modularNo++;
            targetNo = 0;
            EventManager.CallSetComplete();
        }
#endif
    }

    string AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0f)
        {
            return "L";
        }
        else if (dir < 0f)
        {
            return "R";
        }
        else
        {
            return "_";
        }
    }


    public void SetActive(bool inValue)
    {
        gameObject.SetActive(inValue);
       
    }

    public List<Target> GetTargetList()
    {
        return targets;
    }

    public Transform GetTargetTransform()
    {
        if (currentTarget != null)
            return currentTarget.transform;
        else
            return null;
    }

    public void ReduceTargetNo()
    {
        if(targetNo > 0)
            targetNo = 0;
    }
   
}
