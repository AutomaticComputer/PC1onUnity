using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyColorScript : MonoBehaviour
{
    Color lightColor, darkColor;
    private bool isDark = false;
    // Start is called before the first frame update
    void Start()
    {
        setDark(isDark);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setColors(Color c1, Color c2)
    {
        lightColor = c1;
        darkColor = c2;
    }

    public void setDark(bool b)
    {
        isDark = b;
        if (b)
            GetComponent<Renderer>().material.color = darkColor;
        else
            GetComponent<Renderer>().material.color = lightColor;
    }
}
