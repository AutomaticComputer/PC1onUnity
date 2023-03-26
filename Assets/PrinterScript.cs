using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public class PrinterScript : MonoBehaviour
{
    [SerializeField]
    private PrinterDrumScript printerDrumScript;
    [SerializeField]
    private TeleprinterKeyboardScript teleprinterKeyboardScript;
    [SerializeField]
    private TapeScript tapeReadScript, tapePunchScript;
    [SerializeField]
    private OnOffButtonScript buttonReadScript, buttonTapeFastScript, buttonKeyScript, buttonPrintScript, buttonFastScript, buttonPunchScript;
    [SerializeField]
    private PushButtonScript pbSkipScript, pbReadScript, pbRew, pbCutScript;

    private byte keyCode, typeCode, punchCode;

    private float timeElapsed;

    const int maxKeyCode = 64, bsCode = 63;

    // Start is called before the first frame update
    void Start()
    {
        keyCode = 0xff;
        typeCode = 0xff;
        punchCode = 0xff;
        buttonReadScript.setOn(false);
        buttonFastScript.setOn(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (pbCutScript.isPushed())
        {
            printerDrumScript.printToFile();
            return;
        }

        printerDrumScript.setFast(buttonFastScript.isOn());
        tapeReadScript.setFast(buttonTapeFastScript.isOn());
        tapePunchScript.setFast(buttonTapeFastScript.isOn());

        byte b;
        b = teleprinterKeyboardScript.getCode();

        if (b < maxKeyCode)
        {
            keyCode = b;
            if (buttonKeyScript.isOn())
            {
                typeCode = b;
                if (buttonPunchScript.isOn())
                {
                    punchCode = b;
                }
            }
        }

        if (typeCode == 0xff && punchCode == 0xff && pbReadScript.isPushed())
        {
            b = tapeReadScript.read();
            if (b != 0xff)
            {
                typeCode = b;
                if (buttonPunchScript.isOn())
                    punchCode = b;
            }
        }

        if (typeCode == 0xff && punchCode == 0xff && pbSkipScript.isPushed())
        {
            b = tapeReadScript.read();
        }

        if (typeCode == 0xff && punchCode == 0xff && buttonReadScript.isOn())
        {
            b = tapeReadScript.read();
            if (b != 0xff)
            {
                typeCode = b;
                if (buttonPunchScript.isOn())
                    punchCode = b;
            }
            if (tapeReadScript.isAtEnd()) 
            {
                buttonReadScript.setOn(false);
            } 
        }

        if (typeCode != 0xff)
        {
            if (!buttonPrintScript.isOn() || printerDrumScript.type(typeCode))
                typeCode = 0xff;
        }

        if (punchCode != 0xff)
        {
//            if (punchCode == bsCode)
//                tapePunchScript.rewind();
            if (tapePunchScript.punch(punchCode))
                punchCode = 0xff;
        }

        if (pbRew.isPushed())
        {
            tapePunchScript.rewind();
        }
    }


    public byte read()
    {
        byte b = keyCode;
        keyCode = 0xff;
        return b;
    }

    public bool print(byte b) // true if ok
    {
        if (typeCode != 0xff)
            return false;
        bool ok = 
            (!buttonPrintScript.isOn() || printerDrumScript.type(b));
        if (ok && buttonPunchScript.isOn())
        {
            tapePunchScript.punchForced(b);
        }
        return ok;
    }

    public bool isRBusy()
    {
        return (typeCode == 0xff);
    }

    public byte readTape()
    {
        return tapeReadScript.read();
    }

    public void punchForced(byte b)
    {
        tapePunchScript.punchForced(b);
    }
}
