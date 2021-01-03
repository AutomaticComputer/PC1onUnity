using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MiniTapeScript : MonoBehaviour
{
    private string data;
    private string fileName;

    private TapeLibraryScript tapeLibraryScript;
    private Texture2D texture;
    private Texture2D font;
    // Start is called before the first frame update
    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        texture = new Texture2D(360, 16, TextureFormat.RGB24, false);
        rend.material.mainTexture = texture;

        byte[] bytes = Encoding.ASCII.GetBytes(fileName);
        for (int i = 0; i < bytes.Length; i++)
        {
            if (bytes[i] >= 32 && bytes[i] < 128)
            {
                texture.SetPixels(i * 6, 4, 6, 8, font.GetPixels(((bytes[i] - 32) % 64) * 6, (1 - (bytes[i] - 32) / 64) * 8, 6, 8));
            }
        }
        texture.Apply();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setFont(Texture2D f)
    {
        font = f;
    }

    public void setTapeLibraryScript(TapeLibraryScript t)
    {
        tapeLibraryScript = t;
    }

    public void setName(string s)
    {
        fileName = s;
    }

    public void setData(string s)
    {
        data = s;
    }

    public string getData()
    {
        return data;
    }

    private void OnMouseDown()
    {
        tapeLibraryScript.selected(this);
    }
}
