using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TapeSaveScript : MonoBehaviour
{
    [SerializeField]
    private TapeScript tapeScript;
    [SerializeField]
    private TapeLibraryScript tapeLibraryScript;
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
        StringWriter sw = new StringWriter();
        byte[] d = tapeScript.getData();

        for(int i = 0; i < d.Length; i++)
        {
            sw.Write(d[i]);
            sw.Write(' ');
            if (i % 10 == 9)
                sw.WriteLine();
        }
        sw.WriteLine();
        sw.Close();

        tapeLibraryScript.save(sw.ToString());

        tapeScript.clear();
    }
}
