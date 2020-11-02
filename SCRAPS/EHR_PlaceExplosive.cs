using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHR_PlaceExplosive : MonoBehaviour
{
    //Do not need other scripts to refrence these objects. But we want to set
    //them in the inspector
    [Header("Object Refrences")]
    [SerializeField]
    private GameObject device;
    [SerializeField]
    private GameObject deviceAlpha;
    [SerializeField]
    GameObject explosionSystem;
    [SerializeField]
    private GameObject InteractiveObject;
    [SerializeField]
    private GameObject object_FX;
    [SerializeField]
    private SCRAPS_Objective objectiveRef;

    [Header("Explosion Values")]
    [SerializeField]
    private float ExplosionTimer;
    [SerializeField]
    private float lift;
    [SerializeField]
    private float radius;
    [SerializeField]
    private float force;

    [Header("Sfx")]
    [SerializeField]
    private AudioClip explosionSFX;
    [SerializeField]
    private AudioSource as_explosion;
    [SerializeField]
    AudioClip moveClip;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void PlaceDevice()
    {
        if (CheckForDeviceComponents())
        {
            deviceAlpha.SetActive(false);
            device.SetActive(true);
            Invoke("Explode", ExplosionTimer);
            VoiceOverSystem.instance.VoiceOverEvent("Scrapper", "This might be a big explosion," +
                " I better get back" , moveClip);
            
        }
        else if(SCRAPS_Inventory.instance.GetKeyItemAmount("Timer") > 0)
        {
            SCRAPS_MessageSystem.instance.NewMessage("Scrapper",
                "It seems I still need TNT. I should look around",
                SCRAPS_MessageSystem.msgType.standard);
        }
        else if (SCRAPS_Inventory.instance.GetKeyItemAmount("TNT") > 0)
        {
            SCRAPS_MessageSystem.instance.NewMessage("Scrapper",
                "It seems I still need a timer. I should look around",
                SCRAPS_MessageSystem.msgType.standard);
        }
        else 
        {
            SCRAPS_MessageSystem.instance.NewMessage("Scrapper",
                "It seems I still need a timer and TNT. I should look around",
                SCRAPS_MessageSystem.msgType.standard);
        }
    }

    private bool CheckForDeviceComponents()
    {
        if(SCRAPS_Inventory.instance.GetKeyItemAmount("Timer") > 0
           // && SCRAPS_Inventory.instance.CanConsumeKeyItem("Timer", 1)
            && SCRAPS_Inventory.instance.GetKeyItemAmount("TNT") > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Explode()
    {
        int i = 0;
        if (object_FX != null)
        {
            object_FX.SetActive(false);
        }
        device.SetActive(false);

        Collider[] hits = Physics.OverlapSphere(transform.position, radius);

        List<Rigidbody> rbs = new List<Rigidbody>();

        foreach(Collider c in hits)
        {
            Health hp = c.GetComponent<Health>();
            if(hp)
            {
                hp.ApplyDamage(hp.maxHealth);
                //return;
            }
            Rigidbody rb = c.GetComponent<Rigidbody>();
            if (rb)
            {
                i++;
                //Debug.Log("found a rigidbody: " + i + " name: " + rb.name);
                rbs.Add(rb);
            }
        }

        foreach( Rigidbody rb in rbs)
        {
            rb.AddExplosionForce(force, transform.position, radius, lift, ForceMode.Impulse );
        }


        explosionSystem.SetActive(true);
        as_explosion.clip = explosionSFX;
        as_explosion.Play();

        if (gameObject.GetComponent<EHR_ActivateQuest>() != null)
        {
            Debug.Log("Activating Quest if there is a Quest");
            gameObject.GetComponent<EHR_ActivateQuest>().activate = true;
        }
        if(objectiveRef != null)
        {
            objectiveRef.UpdateObjective(1);
            if(objectiveRef.isComplete)
            {
                objectiveRef.gameObject.SetActive(false);
            }
        }

        Invoke("Recycle", 3);
    } 

    private void Recycle()
    {
        Destroy(InteractiveObject);        
    }
   
}
