using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceOverSystem : MonoBehaviour
{
    private static VoiceOverSystem _instance;
    private AudioSource voiceSystemAudio;
    private bool TriggerRun = false;
    public static VoiceOverSystem instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<VoiceOverSystem>();
                
            }
            return _instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        voiceSystemAudio = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(TriggerRun && !voiceSystemAudio.isPlaying)
        {
            voiceSystemAudio.clip = null;
            TriggerRun = false;
        }
    }

    //Voice Over Message
    public void VoiceOverEvent(string name, string message, AudioClip clip)
    {
        SCRAPS_MessageSystem.instance.NewMessage(name, message, SCRAPS_MessageSystem.msgType.standard);
        voiceSystemAudio.clip = clip;
        voiceSystemAudio.Play();
        TriggerRun = true;
        Debug.Log("VoiceOverTrigger Called from VoiceOverSystem");
    }
    //Voice over Event
    
}
