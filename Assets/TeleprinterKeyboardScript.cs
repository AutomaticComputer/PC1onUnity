using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleprinterKeyboardScript : MonoBehaviour
{
    public GameObject keyPrefab, wideKeyPrefab;

    [SerializeField]
    private Texture2D teleprinterFont;
//    private const int charWidth = 12, charHeight = 24;
    private const int charWidth = 16, charHeight = 32;

    private GameObject[] keyboard;

    const float KeyW = 0.02f, KeyH = 0.02f, KeyY = 0.0f, // KeyY = 0.0125f,
        KeyDownY = 0.0075f, CollideZ = 0.0f, // CollideZ = 0.015f,
        KeyboardStartX = -0.14f, KeyboardStartZ = 0.05f;
    const int blankCode = 64;

    private char[] keyCharShift0, keyCharShift1;
    private int[] keyX, keyZ, keyWide, keyFont, keyShift0, keyShift1;
    private float[] keyD;

    private int keyPressed;
    private float keyTimer;
    private Vector3 savedPosition;

    private bool isShifted;
    private byte c;
    // Start is called before the first frame update
    void Start()
    {
        keyX = new int[] 
        {
        1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 
        1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 
        0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 
        0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 
        5 
        };
        keyZ = new int[]
        {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 
        4
        };
        keyD = new float[]
        {
            0.0f, 0.5f, 0.75f, 1.25f, 2.0f
        };
        keyWide = new int[]
        {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
        1
        };
        keyFont = new int[]
        {
        63,  1,  2,  3,  4,  5,  6,  7,  8,  9,  0, 43, 
        33, 39, 21, 34, 36, 41, 37, 25, 31, 32, 10, 44, 
        46, 17, 35, 20, 22, 23, 24, 26, 27, 28, 13, 11, 45, 
        48, 42, 40, 19, 38, 18, 30, 29, 14, 15, 12, 47, 
        16
        };
        keyShift0 = new int[]
        {
         0, 57, 48, 42, 33, 53, 60, 44, 35, 45, 56, 63,
        29, 25, 16, 10,  1, 21, 28, 12,  3, 13, 40,  2,
        32, 24, 20, 18, 22, 11,  5, 26, 30,  9, 47, 34,  8,
        27, 17, 23, 14, 15, 19,  6,  7, 38, 39, 55, 31,
        4 
        };
        keyShift1 = new int[]
        {
         0, 57, 48, 42, 33, 53, 60, 44, 35, 45, 56, 63,
        29, 25, 16, 10,  1, 21, 28, 12,  3, 13, 40,  2,
        32, 24, 20, 18, 22, 11,  5, 26, 30,  9, 47, 34,  8,
        27, 17, 23, 14, 15, 19,  6,  7, 38, 39, 55, 31,
        4 
        };
        keyCharShift0 = new char[]
        {
        '|', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '\b', 
        'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', '+', '\r', 
        '[', 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', ':', '-', '\t', 
        'F', 'z', 'x', 'c', 'v', 'b', 'n', 'm', ',', '.', '=', 'L', 
        ' '}; 
        keyCharShift1 = new char[]
        {
        '|', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '\b', 
        'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', '+', '\r', 
        '[', 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', ':', '-', '\t', 
        'F', 'z', 'x', 'c', 'v', 'b', 'n', 'm', ',', '.', '=', 'L', 
        ' '}; 

        keyboard = new GameObject[keyX.Length];

        for (int i = 0; i < keyboard.Length; i++)
        {
            if (keyWide[i] == 0)
            {
                keyboard[i] = Instantiate(keyPrefab,
                     transform.position + transform.rotation *
                     new Vector3(KeyboardStartX + keyX[i] * KeyW + keyD[keyZ[i]] * KeyW, 
                     KeyY, KeyboardStartZ - keyZ[i] * KeyH),
                     keyPrefab.transform.rotation * transform.rotation);
            }
            else
            {
                keyboard[i] = Instantiate(wideKeyPrefab,
                     transform.position + transform.rotation *
                     new Vector3(KeyboardStartX + keyX[i] * KeyW + keyD[keyZ[i]] * KeyW,
                     KeyY, KeyboardStartZ - keyZ[i] * KeyH),
                     keyPrefab.transform.rotation * transform.rotation);
            }
            /*
            Texture2D texture = new Texture2D(charWidth * 3, charHeight * 2);
            for (int x = 0; x < charWidth * 3; x++)
                for (int y = 0; y < charHeight * 2; y++)
                    texture.SetPixel(x, y, Color.white);
            texture.SetPixels(charWidth, charHeight/2, charWidth, charHeight,
                teleprinterFont.GetPixels(keyFont[i] * charWidth, charHeight, charWidth, charHeight));
            */
            Texture2D texture = new Texture2D(charWidth * 2, charHeight);
            for (int x = 0; x < charWidth * 2; x++)
                for (int y = 0; y < charHeight; y++)
                    texture.SetPixel(x, y, Color.white);
            texture.SetPixels(charWidth/2, 0, charWidth, charHeight,
                teleprinterFont.GetPixels(keyFont[i] * charWidth, charHeight, charWidth, charHeight));
            keyboard[i].GetComponent<Renderer>().material.mainTexture = texture;
            keyboard[i].GetComponent<KeyColorScript>().setColors(Color.white, Color.gray);
            texture.Apply();
        }

        isShifted = false;
        keyPressed = -1;
        keyTimer = 0.0f;
        setColor();
        c = 0xff;
    }


    // Update is called once per frame
    void Update()
    {
        if (keyPressed >= 0) 
        {
            keyTimer -= Time.deltaTime;
            if (keyTimer <= 0.0f) 
            {
                keyboard[keyPressed].transform.position = savedPosition;
                keyPressed = -1;
            }
        }

        if (c != 0xff)
            return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            c = 8; // LF : it seems that we can't read Tab with Input.inputString
            return;
        }

        string s = Input.inputString;
        if (s.Length > 0)
        {
            char ch = s[0];
            for (int k = 0; k < keyboard.Length; k++)
            {
                if (!isShifted && keyCharShift0[k] == ch) {
                    pressedKeyDown(k);
                    c = (byte)keyShift0[k];
                }
                if (isShifted && keyCharShift1[k] == ch) {
                    pressedKeyDown(k);
                    c = (byte)keyShift1[k];
                }
            }
        }
        if (c == blankCode) // blank
            c = 0;
    }

    private void pressedKeyDown(int k)
    {
        if (keyPressed >= 0) 
        {
            keyboard[keyPressed].transform.position = savedPosition;
        }
        keyTimer = 0.25f;
        keyPressed = k;
        savedPosition = keyboard[k].transform.position;
        keyboard[k].transform.position += transform.rotation * new Vector3(0, - KeyDownY, 0);
    }

    private void setColor()
    {
        for(int i=0; i<keyboard.Length; i++)
        {
            bool b = ((!isShifted && keyShift0[i] >= 0) || (isShifted && keyShift1[i] >= 0));
            keyboard[i].GetComponent<KeyColorScript>().setDark(!b);
        }
    }

    public byte getCode()
    {
        byte b = c;
        c = 0xff;
        return b;
    }
    private void OnMouseDown()
    {
        int i, j;
        int k;
        Vector3 dir = Quaternion.Inverse(transform.rotation) * 
                Camera.main.ScreenPointToRay(Input.mousePosition).direction;
        Vector3 q = Quaternion.Inverse(transform.rotation) * 
                (Camera.main.transform.position - transform.position);
        float px = q.x + dir.x * (CollideZ - q.y) / dir.y; 
        float pz = q.z + dir.z * (CollideZ - q.y) / dir.y;

        j = (int)Math.Round((KeyboardStartZ - pz) / KeyH);
        i = (int)Math.Round((px - KeyboardStartX - KeyW * 0.5 * j) / KeyW);
        for (k=0; k < keyboard.Length; k++)
        {
            if (keyX[k] - i <= keyWide[k]*3 && i - keyX[k] <= keyWide[k]*3 && keyZ[k] == j)
            {
                if (!isShifted && keyShift0[k] >= 0)
                {
                    pressedKeyDown(k);
                    c = (byte) keyShift0[k];
                }
                if (isShifted && keyShift1[k] >= 0)
                {
                    pressedKeyDown(k);
                    c = (byte) keyShift1[k];
                }
            }
        }

/*         if (c == 0) // Shift0
        {
            if (isShifted)
            {
                isShifted = false;
                setColor();
            }
            else
                c = 0xff;
        }
        if (c == 27) // Shift1
        {
            if (!isShifted)
            {
                isShifted = true;
                setColor();
            }
            else
                c = 0xff;
        }
 */
        if (c == blankCode) // blank
        {
            c = 0;
        }
    }
}
