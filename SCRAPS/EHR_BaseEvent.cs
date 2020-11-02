using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class EHR_BaseEvent  : MonoBehaviour
{
    public bool completed = false;
    
    abstract public void RunEvent();
}
