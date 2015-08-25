using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WordBox : MonoBehaviour
{

    public Text hiddenTextRef;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        hiddenTextRef.text = GetComponentInParent<UIRefrences>().gameStateRefUI.GetHiddenWord();
    }
}
