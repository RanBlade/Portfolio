using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EHR_Snappable : MonoBehaviour
{
    public string Snapname;
    public GameObject SnapObj;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
       
    }

    public void SetObjActive()
    {
        SnapObj.SetActive(true);
    }
}
