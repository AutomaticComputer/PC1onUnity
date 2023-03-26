using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonScript : MonoBehaviour
{

    public TapeLibraryScript tapeLibrary;
    public string action;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        tapeLibrary.dialogCallback(action);
    }
}
