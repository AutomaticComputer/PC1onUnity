using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleprinterKeyboardScript : MonoBehaviour
{
    public GameObject keyPrefab, wideKeyPrefab;

    [SerializeField]
    private Texture2D teleprinterFont;

    private GameObject[] keyboard;

    const float KeyW = 0.015f, KeyH = 0.015f, KeyY = 0.0125f,
        KeyDownY = 0.0075f, CollideZ = 0.015f,
        KeyboardStartX = -0.12f, KeyboardStartZ = 0.04f;
    const int blankCode = 64;

    private char[] keyCharShift0, keyCharShift1;
    private int[] keyX, keyZ, keyWide, keyFont, keyShift0, keyShift1;

    private bool isShifted;
    private byte c;
    // Start is called before the first frame update
    void Start()
    {
        keyX = new int[] 
            {
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 14, 16, 
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 15,
                -1, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 
                1, 2, 3, 4, 5, 6, 7, 8, 9, 
                4
            };
        keyZ = new int[]
        {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 
        3, 3, 3, 3, 3, 3, 3, 3, 3,
        4
        };
        keyWide = new int[]
        {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
        0, 0, 0, 0, 0, 0, 0, 0, 0, 
        1
        };
        keyFont = new int[]
        {
        1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 11, 26, 63, 32, 
        49, 55, 37, 50, 52, 57, 53, 41, 47, 48, 15,
        13, 33, 51, 36, 38, 39, 40, 42, 43, 44, 10, 25, 30, 
        58, 56, 35, 54, 34, 46, 45, 31, 12, 
        14
        };
        keyShift0 = new int[]
        {
        57, 48, 42, 33, 53, 60, 44, 35, 45, 56, 34, 55, 63, blankCode, 
        29, 25, 16, 10, 1, 21, 28, 12, 3, 13, 32,
        8, 24, 20, 18, 22, 11, 5, 26, 30, 9, 40, 47, 2, 
        17, 23, 14, 15, 19, 6, 7, 38, 39, 
        4
        };
        keyShift1 = new int[]
        {
        57, 48, 42, 33, 53, 60, 44, 35, 45, 56, 34, 55, 63, blankCode, 
        29, 25, 16, 10, 1, 21, 28, 12, 3, 13, 32,
        8, 24, 20, 18, 22, 11, 5, 26, 30, 9, 40, 47, 2, 
        17, 23, 14, 15, 19, 6, 7, 38, 39, 
        4
        };
        keyCharShift0 = new char[]
        {
            '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-', '=', '\b', '|', 
            'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', '[',
            '\t', 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', '+', ':', '\r',
            'z', 'x', 'c', 'v', 'b', 'n', 'm', ',', '.', 
            ' '
        }; 
        keyCharShift1 = new char[]
        {
            '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-', '=', '\b', '|', 
            'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', '[',
            '\t', 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', '+', ':', '\r',
            'z', 'x', 'c', 'v', 'b', 'n', 'm', ',', '.', 
            ' '
        }; 

        keyboard = new GameObject[keyX.Length];

        for (int i = 0; i < keyboard.Length; i++)
        {
            if (keyWide[i] == 0)
            {
                keyboard[i] = Instantiate(keyPrefab,
                     transform.position + transform.rotation *
                     new Vector3(KeyboardStartX + keyX[i] * KeyW + keyZ[i] * (KeyW * 0.5f), 
                     KeyY, KeyboardStartZ - keyZ[i] * KeyH),
                     keyPrefab.transform.rotation * transform.rotation);
            }
            else
            {
                keyboard[i] = Instantiate(wideKeyPrefab,
                     transform.position + transform.rotation *
                     new Vector3(KeyboardStartX + keyX[i] * KeyW + keyZ[i] * (KeyW * 0.5f),
                     KeyY, KeyboardStartZ - keyZ[i] * KeyH),
                     keyPrefab.transform.rotation * transform.rotation);
            }
            Texture2D texture = new Texture2D(12, 16);
            for (int x = 0; x < 12; x++)
                for (int y = 0; y < 16; y++)
                    texture.SetPixel(x, y, Color.white);
            texture.SetPixels(3, 4, 6, 8,
                teleprinterFont.GetPixels(keyFont[i] * 6, 0, 6, 8));
            keyboard[i].GetComponent<Renderer>().material.mainTexture = texture;
            keyboard[i].GetComponent<KeyColorScript>().setColors(Color.white, Color.gray);
            texture.Apply();
        }

        isShifted = false;
        setColor();
        c = 0xff;
    }


    // Update is called once per frame
    void Update()
    {
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
                if (!isShifted && keyCharShift0[k] == ch)
                    c = (byte)keyShift0[k];
                if (isShifted && keyCharShift1[k] == ch)
                    c = (byte)keyShift1[k];
            }
        }
        if (c == blankCode) // blank
            c = 0;
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
            if (keyX[k] == i && keyZ[k] == j)
            {
                if (!isShifted && keyShift0[k] >= 0)
                {
                    c = (byte) keyShift0[k];
                }
                if (isShifted && keyShift1[k] >= 0)
                {
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
