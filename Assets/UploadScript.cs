using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UploadScript : MonoBehaviour
{
    [SerializeField]
     private TapeLibraryScript tapeLibrary;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    private void OnMouseDown()
    {
        tapeLibrary.startDownload(gameObject.transform.position + new Vector3(-0.25f, 0.15f, 0));
    }
#endif
}
