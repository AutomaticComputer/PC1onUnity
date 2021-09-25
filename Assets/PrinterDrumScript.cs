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
    private const int charHeight = 24, charWidth = 12, horizontalMargin = 48, charsPerLine = 80;
    private int currentPosition, currentX;

    private int[] tape_to_font;
    private bool isRotated;
    private bool isUpper;

    private const int roTape = 0, lfTape = 8, crTape = 2, lsTape = 31, fsTape = 27;

    private const float typeTimeSlow = 0.15f, typeTimeFast = 0.015f;
    private float typeTime, timeLeft;

    private byte[] buffer;
    private int bufferLines, bufferCurrent;
    private const int bufferMax = 20000;


    private void clear()
    {
        for (int j = 0; j < charsPerRoll * charHeight; j++)
            for (int i = 0; i < charsPerLine * charWidth + horizontalMargin * 2; i++)
                texture.SetPixel(i, j, Color.white);

        currentPosition = 0;
        currentX = 0;
        isRotated = false;
        isUpper = true;

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
            63, 36, 63, 31, 16, 24, 30, 29, 
            63, 28, 34, 23, 25, 32, 19, 38, 
            21, 42, 20, 18, 35, 41, 22, 40, 
            17, 39, 26, 63, 37, 33, 27, 63, 
            16,  4, 11,  8, 63, 63, 14, 15, 
            10, 63,  3, 63,  7,  9, 63, 13, 
             2, 63, 63, 63, 63,  5, 63, 12, 
             0,  1, 63, 63,  6, 63, 63, 63
        };

        Renderer rend = GetComponent<Renderer>();
        texture = new Texture2D(charsPerLine * charWidth + horizontalMargin * 2, charsPerRoll * charHeight, 
            TextureFormat.RGB24, false);
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
        for (int i = 0; i < charWidth; i++)
        {
            texture.SetPixel(horizontalMargin + currentX * charWidth + i, 
                (charsPerRoll - (currentPosition % charsPerRoll)- 1) * charHeight,
                Color.white);
        }
    }

    private void setCursor()
    {
        for (int i = 0; i < charWidth; i++)
        {
            texture.SetPixel(horizontalMargin + currentX * charWidth + i, 
                (charsPerRoll - (currentPosition % charsPerRoll) - 1) * charHeight,
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
        if (b == lsTape)
        {
            isUpper = true;
            buffer[bufferCurrent] = lsTape;
            bufferCurrent++;
            return true;
        }
        if (b == fsTape)
        {
            isUpper = false;
            buffer[bufferCurrent] = fsTape;
            bufferCurrent++;
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
            for (int j = 0; j < 16 ; j++)
                for (int i = 0; i < charsPerLine * charWidth; i++)
                    texture.SetPixel(charsPerLine * charWidth + horizontalMargin * 2 +  i,
                        (charsPerRoll - (currentPosition % charsPerRoll) - 1) * charHeight - j,
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

        if (currentX < charsPerLine)
        {
            texture.SetPixels(horizontalMargin + currentX * charWidth, 
                (charsPerRoll - (currentPosition % charsPerRoll) - 1) * charHeight, charWidth, charHeight, 
                fontTexture.GetPixels(c * charWidth, isUpper ? charHeight : 0, charWidth, charHeight));
            currentX++;
            setCursor();
            if (bufferCurrent < bufferMax)
            {
                buffer[bufferCurrent] = (byte) c; 
                bufferCurrent++;
            }
        }

        if (currentX == charsPerLine)
        {
            clearCursor();
            currentX = 0;
            currentPosition++;
            isRotated = true;
            for (int j = 0; j < charHeight; j++)
                for (int i = 0; i < charsPerLine * charWidth; i++)
                    texture.SetPixel(horizontalMargin +  i,
                        (charsPerRoll - (currentPosition % charsPerRoll) - 1) * charHeight - j,
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
        bool isUpper_print = false;
        Texture2D tex;
        tex = new Texture2D(charWidth * charsPerLine + horizontalMargin * 2, bufferLines * charHeight + 128, 
                TextureFormat.RGB24, false);


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
            if (buffer[i] == lsTape)
            {
                isUpper_print = true;
                continue;
            }
            if (buffer[i] == fsTape)
            {
                isUpper_print = false;
                continue;
            }
            tex.SetPixels(horizontalMargin + x * charWidth, 
                (bufferLines - y - 1) * charHeight + 64, charWidth, charHeight, 
                fontTexture.GetPixels(buffer[i] * charWidth, isUpper_print ? charHeight : 0, charWidth, charHeight));
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
