using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PrinterDrumScript : MonoBehaviour
{
    Texture2D texture;
    [SerializeField]
    private Texture2D fontTexture;

    private Vector3 savedEulerAngles;
    private const int charsPerRoll = 240;
    private const int pixelsPerChar = 8;
    private int currentPosition, currentX;

    private int[] tape_to_font;
    private bool isRotated;

    private const int roTape = 0, lfTape = 8, crTape = 2;

    private const float typeTimeSlow = 0.15f, typeTimeFast = 0.015f;
    private float typeTime, timeLeft;

    private byte[] buffer;
    private int bufferLines, bufferCurrent;
    private const int bufferMax = 20000;


    private void clear()
    {
        for (int j = 0; j < 1920; j++)
            for (int i = 0; i < 560; i++)
                texture.SetPixel(i, j, Color.white);

        currentPosition = 0;
        currentX = 0;
        isRotated = false;

        clearCursor();
        setCursor();

        texture.Apply();
        isRotated = true;

        bufferCurrent = 0;
        bufferLines = 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        string filePath = Application.persistentDataPath + @"/Printouts/";
        if (!Directory.Exists(filePath)) 
            Directory.CreateDirectory(filePath);

        savedEulerAngles = gameObject.transform.localEulerAngles;

        tape_to_font = new int[]
        {
            32, 52, 30, 47, 14, 40, 46, 45, 
            13, 44, 50, 39, 41, 48, 35, 54, 
            37, 58, 36, 34, 51, 57, 38, 56, 
            33, 55, 42, 16, 53, 49, 43, 27, 
            15,  4, 11,  8, 17, 18, 31, 12, 
            10, 19,  3, 20,  7,  9, 21, 25, 
             2, 22, 23, 24, 28,  5, 29, 26, 
             0,  1, 59, 60,  6, 61, 62, 63
        };

        Renderer rend = GetComponent<Renderer>();
        texture = new Texture2D(560, charsPerRoll * pixelsPerChar, TextureFormat.RGB24, false);
        rend.material.mainTexture = texture;

        buffer = new byte[bufferMax];

        clear();

        timeLeft = 0.0f;
        setFast(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (isRotated)
            gameObject.transform.localEulerAngles = savedEulerAngles +
                new Vector3((currentPosition % charsPerRoll) * (360.0f / charsPerRoll), 0, 0);
        if (timeLeft < typeTime)
            timeLeft += Time.deltaTime;
    }

    private void clearCursor()
    {
        for (int i = 0; i < 6; i++)
        {
            texture.SetPixel(40 + currentX * 6 + i, 
                (charsPerRoll - (currentPosition % charsPerRoll)- 1) * pixelsPerChar,
                Color.white);
        }
    }

    private void setCursor()
    {
        for (int i = 0; i < 6; i++)
        {
            texture.SetPixel(40 + currentX * 6 + i, 
                (charsPerRoll - (currentPosition % charsPerRoll) - 1) * pixelsPerChar,
                Color.gray);
        }
    }

    public bool type(byte b)
    {
        if (timeLeft < typeTime)
            return false;
        timeLeft -= typeTime;

        if (b == roTape)
        {
            return true;
        }
        if (b == crTape)
        {
            clearCursor();
            currentX = 0;
            setCursor();
            texture.Apply();
            if(bufferCurrent < bufferMax)
            {
                buffer[bufferCurrent] = 0xfe; // CR
                   bufferCurrent++;
            }
            return true;
        }
        if (b == lfTape)
        {
            clearCursor();
            currentPosition++;
            isRotated = true;
            setCursor();
            for (int j = 0; j < 8 ; j++)
                for (int i = 0; i < 6 * 80; i++)
                    texture.SetPixel(40 +  i,
                        (charsPerRoll - (currentPosition % charsPerRoll) - 1) * pixelsPerChar - j,
                        Color.white);
            texture.Apply();
            if (bufferCurrent < bufferMax)
            {
                buffer[bufferCurrent] = 0xff; // LF
                bufferCurrent++;
                bufferLines++;
            }
            return true;
        }
        int c = 0;
        c = tape_to_font[b];

        if (currentX < 80)
        {
            texture.SetPixels(40 + currentX * 6, 
                (charsPerRoll - (currentPosition % charsPerRoll) - 1) * pixelsPerChar, 6, 8, 
                fontTexture.GetPixels(c * 6, 0, 6, 8));
            currentX++;
            setCursor();
            if (bufferCurrent < bufferMax)
            {
                buffer[bufferCurrent] = (byte) c; 
                bufferCurrent++;
            }
        }

        if (currentX == 80)
        {
            clearCursor();
            currentX = 0;
            currentPosition++;
            isRotated = true;
            for (int j = 0; j < 8 ; j++)
                for (int i = 0; i < 6 * 80; i++)
                    texture.SetPixel(40 +  i,
                        (charsPerRoll - (currentPosition % charsPerRoll) - 1) * pixelsPerChar - j,
                        Color.white);
            setCursor();
        }
        texture.Apply();

        return true;
    }

    public bool isReady()
    {
        return (timeLeft >= typeTime);
    }

    public void printToFile()
    {
#if UNITY_WEBGL
#else
        Texture2D tex;
        tex = new Texture2D(560, bufferLines * pixelsPerChar + 64, TextureFormat.RGB24, false);


        for (int j = 0; j < tex.height; j++)
            for (int i = 0; i < tex.width; i++)
                tex.SetPixel(i, j, Color.white);

        int x = 0, y = 0;

        for (int i = 0; i < bufferCurrent; i++)
        {
            if (buffer[i] == 0xfe)
            {
                x = 0;
                continue;
            }
            if (buffer[i] == 0xff)
            {
                y ++;
                continue;
            }
            tex.SetPixels(40 + x * 6, (bufferLines - y - 1) * pixelsPerChar + 32, 6, 8, 
                fontTexture.GetPixels(buffer[i] * 6, 0, 6, 8));
            x++;
        }

        tex.Apply();
        byte[] bytes = tex.EncodeToPNG();

        Object.Destroy(tex);

        string fileNameBase, fileName;

        fileNameBase = Application.persistentDataPath + @"/Printouts/" + System.DateTime.Now.ToString("yyyyMMddHHmmss");

        for(int i=0; ; i++) {
            if (i == 0)
                fileName = fileNameBase + ".png";
            else
                fileName = fileNameBase + i + ".png";
            if (!System.IO.File.Exists(fileName))
                break;
        }

        File.WriteAllBytes(fileName, bytes);
#endif

        clear();
    }

    public void setFast(bool b)
    {
        if (b)
            typeTime = typeTimeFast;
        else
            typeTime = typeTimeSlow;
    }
}
