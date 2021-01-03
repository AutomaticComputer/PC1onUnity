using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapeSkipScript : MonoBehaviour
{
    [SerializeField]
    private TapeScript tapeScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        tapeScript.skip();
    }

}
