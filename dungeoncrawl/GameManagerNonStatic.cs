using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerNonStatic : MonoBehaviour {


    public Quest TailsmanQuest;
    public Quest AltarQuest;
    public static GameManagerNonStatic instance;
    public GameObject PlayerPrefab;
    public GameObject UIPrefab;

    public GameObject TownStartSpawn;
    public GameObject ExitDungeonSpawn;

    public bool initalLoad = true;
    public bool cameraSetup = false;

    public int WinLose = -1;

    public bool PlayAgain = false;
    public bool GameOverHandled = false;

    public bool SceneChange = false;

    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
            PlayAgain = false;

        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Debug.Log("GameManager Loading");

        DontDestroyOnLoad(this);

        LoadPlayer();
        LoadUI();

        TailsmanQuest = new Quest
        {
            Name = "Retrieve the Talisman of Yok",
            Description = "Venture into the catacombs and collect the Talisman of Yok. Once collected bring the tailsman back to Ubbr to be infused",
            ID = 1607
        };

        AltarQuest = new Quest
        {
            Name = "Destroy the Altar of Yok",
            Description = "Now that you have the talisman. You must take it to the Altar in the snowy Mountains to the East. Once you find the altar you must kill the altar guardian. After the guardian has been defeated place the talisman on the altar and the power will overwhelm and destroy it.",
            ID = 1337
        };
        //GameObject.Find("CameraController").GetComponent<CameraControllerISOFollow>().Setup();
        initalLoad = false;
        SetupCamera();
    }

    // Update is called once per frame
    void Update() {
        if (SceneChange && SceneManager.GetActiveScene().name == "DungeonOne")
        {
            GameObject spawn = GameObject.Find("PlayerSpawn");
            Debug.Log("Found Spawn: " + spawn.transform.name);
            SceneChange = false;
        }
        if (cameraSetup == false)
        {
            SetupCamera();
        }
        CheckForWinLose();
   
        if(SceneManager.GetActiveScene().name == "GameOverWin" || SceneManager.GetActiveScene().name == "GameOverLose")
        {
            Debug.Log("Destroying GameManager");
            DestroyGameObjects();
        }
    }

    private void FixedUpdate()
    {
        
    }

    public Quest GiveQuest()
    {
        if (TailsmanQuest.QuestCompleted == false)
        {
            return TailsmanQuest;
        }
        else if (AltarQuest.QuestCompleted == false && TailsmanQuest.QuestCompleted == true)
        {
            return AltarQuest;
        }
        else
        {
            Debug.Log("No quests available");
            return null;
        }
    }
    void LoadPlayer()
    {
        Instantiate(PlayerPrefab, TownStartSpawn.transform.position, TownStartSpawn.transform.rotation);

    }
    void LoadUI()
    {
        Instantiate(UIPrefab, Vector3.zero, Quaternion.identity);
        GameObject.Find("UICanvas(Clone)").name = "UICanvas";

    }
    void SetupCamera()
    {
        if (GameObject.Find("Player"))
        {
            Debug.Log("Player Found");
            if (GameObject.Find("CameraController"))
            {
                Debug.Log("Found CameraController for level");
                GameObject playerRef = GameObject.Find("Player").gameObject;
                if (playerRef != null)
                {
                    Debug.Log("Player GameObject NOT NULL");
                    GameObject followObjRef = GameObject.Find("CameraController").gameObject;

                    followObjRef.GetComponent<CameraControllerISOFollow>().FollowObject = playerRef;
                    followObjRef.GetComponent<CameraControllerISOFollow>().StopFollowing();
                    followObjRef.GetComponent<CameraControllerISOFollow>().Setup();
                    followObjRef.GetComponent<CameraControllerISOFollow>().StartFollowing();
                    cameraSetup = true;
                }

                //cameraSetup = true;
            }
        }
    }

    void CheckForWinLose()
    {
        GameObject CheckPlayerDead = GameObject.Find("Player");
        if (AltarQuest.QuestCompleted == true && TailsmanQuest.QuestCompleted == true)
        {
            WinLose = 1;
            SceneManager.LoadScene("GameOverWin");

        }

        else if (!CheckPlayerDead.GetComponent<PlayerController>().ReportDeath())
        {
            WinLose = 2;
            SceneManager.LoadScene("GameOverLose");

        }
    }
    void DestroyGameObjects()
    {       
            Destroy(GameObject.Find("Player"));
            Destroy(GameObject.Find("UICanvas"));
            Destroy(gameObject);
        
    }
}
