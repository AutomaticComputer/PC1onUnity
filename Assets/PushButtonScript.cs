using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushButtonScript : MonoBehaviour
{
    public bool _isPushed;
    // Start is called before the first frame update
    void Start()
    {
        _isPushed = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        _isPushed = true;
    }
    public bool isPushed()
    {
        bool b = _isPushed;
        _isPushed = false;
        return b;
    }
}
