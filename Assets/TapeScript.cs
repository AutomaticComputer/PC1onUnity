using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class TapeScript : MonoBehaviour
{
    // Start is called before the first frame update
    private Texture2D punchedTexture, texture;

    private const int charsPerRoll = 160; 
    private byte[] data;
    private int currentPosition, length; 
    private const int maxLength = 100000;
    private Vector3 savedEulerAngles;

    private bool isDrawn;
//    private const float readTime = 0.003333f, punchTime = 0.016667f;

// 15 chars/s
    private const float readTime = 0.066667f, punchTime = 0.066667f; 
    private float readTimeLeft, punchTimeLeft;

    public int charsBeforeCurrent; // how many characters to show before the current position. 
    void Start()
    {
        savedEulerAngles = gameObject.transform.localEulerAngles;

        Renderer rend = GetComponent<Renderer>();
        punchedTexture = new Texture2D(56, 520, TextureFormat.RGB24, false);
        texture = new Texture2D(56, charsPerRoll * 8, TextureFormat.RGB24, false);

        rend.material.mainTexture = texture;

        for (int j = 0; j < 64; j++)
            for (int i = 0; i < 7; i++)
            {
                bool b = ((i < 3 && (j & (1 << i)) != 0) || (i > 3 && (j & (1 << (i-1))) != 0));
                for (int x = 0; x < 8; x++)
                    for (int y = 0; y < 8; y++)
                    {
                        if ((i == 3 && (2 * x - 7) * (2 * x - 7) + (2 * y - 7) * (2 * y - 7) <= 9) ||
                            (b && (2 * x - 7) * (2 * x - 7) + (2 * y - 7) * (2 * y - 7) <= 28))
                            punchedTexture.SetPixel(i * 8 + x, j * 8 + y, Color.black);
                        else
                            punchedTexture.SetPixel(i * 8 + x, j * 8 + y, Color.white);
                    }
            }
        for (int i = 0; i < 7; i++)
            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                    punchedTexture.SetPixel(i * 8 + x, 64 * 8 + y, Color.white);

        clear();

        readTimeLeft = 0.0f;
        punchTimeLeft = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDrawn)
        {
            texture.Apply();
            isDrawn = false;
        }
        gameObject.transform.localEulerAngles = savedEulerAngles + 
            new Vector3((currentPosition % charsPerRoll) * (360.0f / charsPerRoll), 0, 0);

        if (readTimeLeft < readTime)
            readTimeLeft += Time.deltaTime;
        if (punchTimeLeft < punchTime)
            punchTimeLeft += Time.deltaTime;
    }

    private void drawAll()
    {
        for (int i = 0; i < charsPerRoll; i++)
        {
            byte b;
            if (i < charsPerRoll - charsBeforeCurrent && i < length)
                b = data[i];
            else
                b = 64;

            texture.SetPixels(0, i * 8, 56, 8, punchedTexture.GetPixels(0, b * 8, 56, 8));
        }
        texture.Apply();
    }

    public void clear()
    {
        data = new byte[maxLength];
        length = 0;

        currentPosition = 0;

        drawAll();

        isDrawn = false;
    }

    public bool punch0(byte b, bool wait) // true if ok
    {
        if (wait) 
        {
          if (punchTimeLeft < punchTime)
            return false;
            punchTimeLeft -= punchTime;
        }
  
        if (currentPosition >= maxLength - 1)
            return false;

        data[currentPosition] |= b;
        texture.SetPixels(0, (currentPosition % charsPerRoll) * 8, 56, 8, 
            punchedTexture.GetPixels(0, data[currentPosition] * 8, 56, 8));
        if (currentPosition >= charsBeforeCurrent)
        {
            int p = currentPosition + charsPerRoll - charsBeforeCurrent;
            int c = 64;
            if (p < length)
                c = data[p];
            texture.SetPixels(0, (p % charsPerRoll) * 8, 56, 8, punchedTexture.GetPixels(0, c * 8, 56, 8));
        }
        isDrawn = true;

        if (currentPosition == length)
            length++;
        currentPosition++;

        return true;
    }

    public bool punch(byte b) // true if ok
    {
        return punch0(b, true);
    }
    public void punchForced(byte b) 
    {
        punch0(b, false);
    }

    public void rewind()
    {
        if (currentPosition == 0)
            return;

        currentPosition--;
        byte b = 0;
        int p = (currentPosition + charsPerRoll - charsBeforeCurrent) % charsPerRoll;
        if (currentPosition >= charsBeforeCurrent)
            b = data[currentPosition - charsBeforeCurrent];
        else
            b = 64;
        texture.SetPixels(0, p * 8, 56, 8, punchedTexture.GetPixels(0, b * 8, 56, 8));
        isDrawn = true;
    }

    public byte read()
    {
        byte b;

        if (readTimeLeft < readTime)
            return 0xff;
        readTimeLeft -= readTime;

        if (currentPosition >= length)
            return 0xff;

        b = data[currentPosition];
        if (currentPosition >= charsBeforeCurrent)
        {
            int p = currentPosition + charsPerRoll - charsBeforeCurrent;
            byte c = 64;
            if (p >= 0 && p < length)
                c = data[p];
            texture.SetPixels(0, (p % charsPerRoll) * 8, 56, 8, punchedTexture.GetPixels(0, c * 8, 56, 8));
        }
        isDrawn = true;

        b = data[currentPosition];
//        if (currentPosition == length)
//            length++;
        currentPosition++;

        return b;
    }


    public void skip()
    {
        if (currentPosition >= length)
            return;

        if (currentPosition >= charsBeforeCurrent)
        {
            int p = currentPosition + charsPerRoll - charsBeforeCurrent;
            byte c = 64;
            if (p >= 0 && p < length)
                c = data[p];
            texture.SetPixels(0, (p % charsPerRoll) * 8, 56, 8, punchedTexture.GetPixels(0, c * 8, 56, 8));
        }
        isDrawn = true;

        currentPosition++;
    }


    public void readString(string s)
    {
        string[] values = Regex.Split(s, @"\D+");

        length = 0;

        for (int i = 0; i < values.Length; i++)
        {
            if (!string.IsNullOrEmpty(values[i]))
            {
                data[length] = byte.Parse(values[i]);
                length++;
            }
        }

        for (int i = length; i < maxLength; i++)
            data[i] = 0;
        currentPosition = 0;

        drawAll();

        isDrawn = false;
    }

    public byte[] getData()
    {
        byte[] value = new byte[length];
        for (int i = 0; i < length; i++)
            value[i] = data[i];
        return value;
    }

    public bool isRBusy()
    {
        return (currentPosition >= length);
    }
}
