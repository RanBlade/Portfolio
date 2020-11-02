using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceOverTrigger : MonoBehaviour
{
    [SerializeField]
    private string Name;
    [SerializeField]
    private string Message;
    [SerializeField]
    private AudioClip Clip;
    [SerializeField]
    private bool DoThisOnce = false;

    private bool doOnce = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(doOnce)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (DoThisOnce)
            {
                if (Name != "" && Message != "" && Clip != null && !doOnce)
                {
                    VoiceOverSystem.instance.VoiceOverEvent(Name, Message, Clip);
                    Debug.Log("This message has been played");
                }
                else
                {
                    Debug.LogWarning("VoiceOverTrigger has null fields and will not play");
                }
                doOnce = true;
            }
            else
            {
                if (Name != "" && Message != "" && Clip != null)
                {
                    VoiceOverSystem.instance.VoiceOverEvent(Name, Message, Clip);
                    Debug.Log("This message has been played");
                }
                else
                {
                    Debug.LogWarning("VoiceOverTrigger has null fields and will not play");
                }
            }
        }
        
    }
}
