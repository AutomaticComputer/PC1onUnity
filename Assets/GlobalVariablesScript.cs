using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariablesScript : MonoBehaviour
{
    private bool _keyIsDisabled = false;
    [SerializeField]
    private UploadScript uploadScript;
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        uploadScript.gameObject.SetActive(true);            
#endif        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setKeyDisabled(bool b)
    {
        _keyIsDisabled = b;
    }

    public bool keyIsDisabled()
    {
        return _keyIsDisabled;
    }
}
