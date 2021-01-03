using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffButtonScript : MonoBehaviour
{
    private bool _isOn = true;
    private Color offColor, onColor;
    // Start is called before the first frame update
    void Start()
    {
        offColor = new Color(0.4f, 0.4f, 0.0f);
        onColor = new Color(0.7f, 1.0f, 0.0f);
        //        gameObject.GetComponent<Renderer>().material.color = onColor;
        setOn(_isOn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        setOn(!_isOn);
    }
    public bool isOn()
    {
        return _isOn;
    }

    public void setOn(bool b)
    {
        if (b)
        {
            gameObject.GetComponent<Renderer>().material.color = onColor;
        }
        else
        {
            gameObject.GetComponent<Renderer>().material.color = offColor;
        }
        _isOn = b;
    }
}
