using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaSwitch : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ChangeAlpha(gameObject.GetComponent<Material>(), 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeAlpha(Material mat, float alphaValue)
    {
        Color oldColor = mat.color;
        Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alphaValue);
        mat.SetColor("_color", newColor);
    }

}
