using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCanScript : MonoBehaviour
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
    private void OnMouseDown()
    {
        tapeLibrary.startDelete(gameObject.transform.position + new Vector3(-0.08f, 0.23f, 0));
    }
}
