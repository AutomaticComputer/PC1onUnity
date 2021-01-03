using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorScript : MonoBehaviour
{
    [SerializeField]
    private Material onMaterial, offMaterial, specialMaterial;
    private bool lastState, wasSpecial;
    // Start is called before the first frame update
    void Start()
    {
        lastState = false;
        wasSpecial = false;
        gameObject.GetComponent<Renderer>().material = offMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setState(bool b)
    {
        if (b != lastState || wasSpecial)
        {
            wasSpecial = false;
            lastState = b;
            if (b)
                gameObject.GetComponent<Renderer>().material = onMaterial;
            else
                gameObject.GetComponent<Renderer>().material = offMaterial;
        }
    }

    public void setSpecial()
    {
        wasSpecial = true;
        gameObject.GetComponent<Renderer>().material = specialMaterial;
    }
}
