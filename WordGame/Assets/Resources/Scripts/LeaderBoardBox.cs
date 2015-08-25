using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using UtilityScripts;

public class LeaderBoardBox : MonoBehaviour
{

    public Text leaderBoardTextRef;
    private gameState gameStateRef;
    // Use this for initialization
    void Start()
    {
        gameStateRef = GetComponentInParent<UIRefrences>().gameStateRefUI;
    }

    // Update is called once per frame
    void Update()
    {
        leaderBoardTextRef.text = gameStateRef.GetFormattedLeaderBoard();
    }
}
