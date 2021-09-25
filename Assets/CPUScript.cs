using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CPUScript : MonoBehaviour
{
    private bool playSound = false;
 
    AudioSource audioSource;
    private float sampleRate = 48000f;
    private Queue<Tuple<uint, bool>> audioPulses;
    private float currentAudioLevel = 0.0f;
    private uint elapsedCycles;
    private float elapsedTimeAudio;

    private PanelScript panelScript; 
    public PrinterScript printerScript;

    public UInt64 regA, regR, regM, regX, currentOrder;
    public UInt16 regK;
    public UInt32[] mainStore;
    bool isInitialLoad;
    private const int mainStoreSize = 512; // in short words
    private float timeLeft;
    private float cycleTime;
    private float[] cycleTimes;
    private byte opCode;
    private bool isLongWord;
    private UInt16 operand;
    private bool isStopped, isFreeRun; 
    
    private int initialLoadCounter, initialLoadChars;
    private UInt32[] initialLoadBuffer;
    const byte initialLoadEndChar = 0x3f;
    const UInt64 bit_18 = 1L << 18, bit_34 = 1L << 34, bit_35 = 1L << 35, bit_36 = 1L << 36;

// Start is called before the first frame update
void Start()
    {
        audioPulses = new Queue<Tuple<uint, bool>>();

        // The code for generating sound is based on 
        // https://forum.unity.com/threads/generating-a-simple-sinewave.471529/
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0; 
        audioSource.Stop(); 
        
        mainStore = new UInt32[mainStoreSize];
        initialLoadBuffer = new UInt32[6];

        cycleTimes = new float[]
        {
            0.0001f, 0.00005f, 0.000025f, 0.0000125f
        };
        cycleTime = cycleTimes[0];

        isInitialLoad = false; 
        isStopped = true;
        isFreeRun = false;

        regA = 0;
        regM = 0;
        regR = 0;
        regX = 0;
        regK = 0;

        timeLeft = 0.0f;

    }


    private bool runPressed, manualPressed, kbWaitPressed, flagContinue;
    // flagContinue: either Continue is pressed or primary input ended


// Update is called once per frame
    void Update()
    {
        if(playSound && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        if(!playSound && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        if (audioPulses.Count == 0)
        {
            elapsedCycles = 0;
            elapsedTimeAudio = -0.1f;
        }

        if (panelScript == null)
            panelScript = gameObject.GetComponent<PanelScript>();

    // get value from panel

        if (!isInitialLoad) {
            if (panelScript.initialLoadPushed()) 
            {
                isStopped = true;
                isInitialLoad = true;
                initialLoadCounter = -1;
                initialLoadChars = 0;
                timeLeft = 0.0f;
            }
        }  

        bool clearStartPushed = panelScript.clearStartPushed(),
            initialStartPushed = panelScript.initialStartPushed(),
            restartPushed = panelScript.restartPushed(),
            freeRunPushed = panelScript.freeRunPushed(),
            stopPushed = panelScript.stopPushed(), 
            soundPushed = panelScript.soundPushed();

        if (soundPushed)
        {
            if (playSound) 
            {
                playSound = false;
            }
            else
            {
                elapsedCycles = 0;
                elapsedTimeAudio = -0.1f;
                audioPulses.Clear();
                playSound = true;                
            }
        }

        if (!isInitialLoad) {
            if (freeRunPushed) 
            {
                isFreeRun = true;
            }
            if (stopPushed) 
            {
                isFreeRun = false;
            }

/*             if (!isFreeRun)
            {
                isStopped = true; // after "one shot"
            }
 */
            if (clearStartPushed || initialStartPushed || restartPushed) {
                isStopped = false;
                timeLeft = 0;
            }
            if (clearStartPushed) {
                regA = 0;
                regM = 0;
                regR = 0;
                regK = 0;
                regX = readShortWord(regK);
            }
            if (initialStartPushed) {
                regK++;
                regX = readShortWord(regK);
            }
        }

//        cycleTime = cycleTimes[speedWheelScript.getValue()];

        if (isInitialLoad)
        {
            timeLeft += Time.deltaTime;
            while (true) 
            {
                byte a = printerScript.readTape();
                if (a == 0xff) {
                    break;
                }
                if (timeLeft < 0)
                    break;
                addCycles(4);
                if (initialLoadCounter < 0) 
                {
                    if (a != 0 && a != initialLoadEndChar) 
                    {
                        initialLoadCounter = 0;
                        initialLoadChars = 0;
                    }
                    continue;
                } 
                if (initialLoadChars == 6) {
                    if (a == initialLoadEndChar) {
                        isInitialLoad = false;
                        break;
                    }
                }
                else
                {
                    initialLoadBuffer[initialLoadChars] = a;
                    if (initialLoadChars == 5) 
                    {
                        mainStore[initialLoadCounter & (mainStoreSize - 1)] = 
                            (initialLoadBuffer[0] << 12) 
                            | (initialLoadBuffer[1] << 6) 
                            | initialLoadBuffer[2];
                        mainStore[(initialLoadCounter + 1)  & (mainStoreSize - 1)] = 
                            (initialLoadBuffer[3] << 12)
                            | (initialLoadBuffer[4] << 6) 
                            | initialLoadBuffer[5] ;
                        regA = (regA ^ 
                            (((UInt64) mainStore[initialLoadCounter & (mainStoreSize - 1)] << 18) | 
                              (UInt64) mainStore[(initialLoadCounter + 1)  & (mainStoreSize - 1)]));
                        initialLoadCounter += 2;
                    }
                }
                initialLoadChars = (initialLoadChars + 1) % 7;
            }
        } 
        else
        {
            if (!isStopped) 
            {
                timeLeft += Time.deltaTime;
                while(timeLeft > 0) {
                    currentOrder = regX;
                    regK++;
                    regX = readShortWord(regK);
                    processOneOrder();
                    if (!isFreeRun || isStopped)
                    {
                        isStopped = true;
                        break; // "one shot"
                    }
                }
            }
        }

        updateDisplay();
    }

    private int lastDisplayed;

    private void updateDisplay()
    {

        // update panelScript
        panelScript.setRegisterIndicator(0, regM);
        panelScript.setRegisterIndicator(1, regA);
        panelScript.setRegisterIndicator(2, regR);
        panelScript.setFreeRunIndicator(isFreeRun);
        panelScript.setSoundIndicator(playSound);

        panelScript.setOrIndicator(regX);
        panelScript.setSccIndicator(regK);
    }

    private void addCycles(uint i)
    {
        timeLeft -= i * cycleTime;
        elapsedCycles += i;
    }

    private UInt64 readShortWord(UInt64 addr)
    {
        UInt64 w = (UInt64) mainStore[addr  & (mainStoreSize - 1)];
        return w;
    }

    private UInt64 readLongWord(UInt64 addr)
    {
        UInt64 a = ((addr+1) / 2)*2; // OK?
        UInt64 w = 
            ((((UInt64) mainStore[a  & (mainStoreSize - 1)]) << 18) | ((UInt64) mainStore[(a+1)  & (mainStoreSize - 1)]));
        return w;
    }

    private void loadShortWord(UInt64 addr)
    {
        regM = (readShortWord(addr) << 18);
    }

    private void loadLongWord(UInt64 addr)
    {
        regM = readLongWord(addr); 
    }

    private void loadWord(bool isLongWord, UInt64 addr)
    {
        if (isLongWord)
        {
            loadLongWord(addr);
        }
        else
        {
            loadShortWord(addr);
        }
    }

    private void storeShortWord(UInt64 addr, UInt64 data)
    {
        mainStore[addr  & (mainStoreSize - 1)] = (UInt32) data;
        regX = readShortWord(regK); // might have been rewritten
    }

    private void storeLongWord(UInt64 addr, UInt64 data)
    {
        UInt64 a = ((addr+1)/2)*2;
        mainStore[a  & (mainStoreSize - 1)] = (UInt32) (data >> 18);
        mainStore[(a + 1)  & (mainStoreSize - 1)] = (UInt32) (data & (bit_18 - 1));

        regX = readShortWord(regK); // might have been rewritten
    }

    public void processOneOrder()
    {
        opCode = (byte) (currentOrder >> 12);
        isLongWord = ((currentOrder & 0x800) != 0);
        operand = (UInt16) (currentOrder & 0x7ff);

        switch (opCode)
        {
            case 1: // t
                if (isLongWord) 
                {
                    storeLongWord(operand, regA);
                } 
                else
                {
                    storeShortWord(operand, regA >> 18);
                }
                addCycles(8);
                break;
            case 3: // o
                {
                    byte b = (byte) (regA >> 30);

                    if (!printerScript.print(b)) 
                    {
                        regK = operand;
                        regX = readShortWord(regK);
                    }
                    addCycles(4);
                }
                break;
            case 6: // n
                loadWord(isLongWord, operand);
                regA = (((1L << 36) - regM) & 0xfffffffffL);
                addCycles(4);
                break;
            case 9: // l
                {
                    UInt16 i; 
                    if (operand < 1024) 
                    {
                        i = operand;
                        if (i > 128) 
                        {
                            i = 128;
                        }
                        if (isLongWord) 
                        {
                            for(int j = 0; j < i; j++) 
                            {
                                regA = ((regA << 1) & (bit_36 - 1));
                                regA |= ((regR & bit_34) >> 34);
                                regR = regR - (regR & bit_34);
                                regR = (regR << 1) - (regR & bit_35);
                            }
                        } 
                        else
                        {
                            regA = ((regA << i) & (bit_36 - 1));
                        }
                    }
                    else
                    {
                        i = (UInt16) (2048 - operand);
                        if (i > 128) 
                        {
                            i = 128;
                        }
                        if (isLongWord) 
                        {
                            for(int j = 0; j < i; j++) 
                            {
                                regR = (regR >> 1) + (regR & bit_35);
                                regR |= ((regA & 1) << 34);
                                regA = (regA >> 1);
                            }
                        } 
                        else
                        {
                            regA = (regA >> i);
                        }
                    }
                    addCycles((UInt16) (6 + i));
                }
                break;
            case 10: // r
                {
                    UInt16 i; 
                    if (operand < 1024) 
                    {
                        i = operand;
                        if (i > 128) 
                        {
                            i = 128;
                        }
                        if (isLongWord) 
                        {
                            bool b = ((regA & bit_35) != 0);
                            for(int j = 0; j < i; j++) 
                            {
                                regR = (regR >> 1) + (regR & bit_35);
                                regR |= ((regA & 1) << 34);
                                regA = (regA >> 1);
                                if (b)
                                {
                                    regA |= bit_35;
                                }
                            }
                        } 
                        else
                        {
                            bool b = ((regA & bit_35) != 0);
                            for(int j = 0; j < i; j++) 
                            {
                                regA = (regA >> 1);
                                if (b)
                                {
                                    regA |= bit_35;
                                }
                            }
                        }
                    }
                    else
                    {
                        i = (UInt16) (2048 - operand);
                        if (i > 128) 
                        {
                            i = 128;
                        }
                        if (isLongWord) 
                        {
                            for(int j = 0; j < i; j++) 
                            {
                                regA = ((regA << 1) & (bit_36 - 1));
                                regA |= ((regR & bit_34) >> 34);
                                regR = regR - (regR & bit_34);
                                regR = (regR << 1) - (regR & bit_35);
                            }
                        } 
                        else
                        {
                            regA = ((regA << i) & (bit_36 - 1));
                        }
                    }
                    addCycles((UInt16) (6 + i));
                }
                break;
            case 12: // i
                if (isLongWord) 
                {
                    Debug.Log("jl: Unused");
                } 
                else
                {
                    byte b = printerScript.readTape();

                    if (b == 0xff) 
                    {
                        regK = operand;
                        regX = readShortWord(regK);
                        addCycles(10);
                    }
                    else
                    {
                        regA = ((UInt64) b) << 30;
                        addCycles(4);
                    }
                }
                break;
            case 13: // p
                loadWord(isLongWord, operand);
                regA = regM;
                addCycles(4);
                break;
            case 14: // c
                loadWord(isLongWord, operand);
                regA = (regA & regM);
                addCycles(4);
                break;
            case 15: // v
                loadWord(isLongWord, operand);
                {
                    UInt64 signOfM, signOfA;
                    regR = regA;
                    regA = 0;
                    signOfM = ((regM & bit_35) << 1);
                    signOfA = 0;
                    for(int i=0; i<35; i++)
                    {
                        if ((regR & 1) != 0) {
                            regA = (((regA | signOfA) +(regM | signOfM)) 
                                    & 0x1fffffffffL);
                        }
                        else
                        {
                            regA = (regA | signOfA);
                        }
                        regR = (regR >> 1);
                        regR |= ((regA & 1) << 35);
                        signOfA = (regA & bit_36);
                        regA = (regA >> 1);
                    }
                    if ((regR & 1) != 0) {
                        regA = ((regA - regM) & 0xfffffffffL);
                    }
                    regR = (regR >> 1);
                }
                if (isLongWord)
                {
                    addCycles(44);
                }
                else
                {
                    addCycles(26);
                }
                break;
            case 17: // z
                if (isLongWord) 
                {
                    if (regA == 0) 
                    {
                        regK = operand;
                        regX = readShortWord(regK);
                    }
                } 
                else
                {
                    if ((regA & (0x7ffL << 18)) == 0) 
                    {
                        regK = operand;
                        regX = readShortWord(regK);
                    }
                }
                addCycles(11);
                break;
            case 18: // d
                loadWord(isLongWord, operand);
                {
                    bool aWasNegative, mIsNegative; 
                    bool overflow = false;
                    regR = (regR - (regR & bit_35)) << 1;

                    aWasNegative = ((regA & bit_35) != 0);
                    mIsNegative = ((regM & bit_35) != 0);
                    for(int i=0; i<36; i++)
                    {
                        if (aWasNegative == mIsNegative) {
                            regA = ((regA - regM) & 0xfffffffffL);
                            regR |= 1;
                        }
                        else
                        {
                            regA = ((regA + regM) & 0xfffffffffL);
                        }
                        aWasNegative = ((regA & bit_35) != 0);

                        regA = (regA << 1) & 0xfffffffffL;
                        regA |= ((regR & bit_35) >> 35);
                        regR = (regR - (regR & bit_35)) << 1;
                    }
                    overflow = (((regA & 1) != 0) == ((regR & bit_35) != 0));
                    regA = (regA >> 1);
                    if (aWasNegative)
                    {
                      regA = regA | bit_35;
                    }
                    regR |= 1;

// Debug.Log(Convert.ToString((long)regA, 2) + "," + Convert.ToString((long)regR, 2));
                    if (aWasNegative)
                    {
                        if (mIsNegative) {
                            regA = ((regA - regM) & 0xfffffffffL);
                            regR = ((regR + 1) & 0xfffffffffL);                            
                        }
                        else
                        {
                            regA = ((regA + regM) & 0xfffffffffL);
                            regR = ((regR - 1) & 0xfffffffffL);
                        }
                    }
                    UInt64 tmp = regA;
                    regA = regR;
                    regR = tmp;
                    if (overflow)
                    {
                        isStopped = true;
                        Debug.Log("Overflow");
                    }
                }
                addCycles(161);
                break;
            case 19: // b
                loadWord(isLongWord, operand);
                regA = (regA ^ regM);
                addCycles(4);
                break;
            case 20: // s
                loadWord(isLongWord, operand);
                regA = ((regA - regM) & 0xfffffffffL);
                addCycles(4);
                break;
            case 21: // y
                if (operand == 31 && playSound)
                {
                    if (isLongWord) 
                    {
                        audioPulses.Enqueue(new Tuple<uint, bool>(elapsedCycles, false));
                    }
                    else
                    {
                        audioPulses.Enqueue(new Tuple<uint, bool>(elapsedCycles, true));
                    }
                }
                addCycles(4);
                break;
            case 23: // x
                if (isLongWord) 
                {
                    Debug.Log("xl: Unused");
                } 
                else
                {
                    UInt64 w = readShortWord(operand);
                    w = (w & 0x3f800L) | ((regA >> 18) & 0x7ffL);
                    storeShortWord(operand, w);
                }
                addCycles(8);
                break;
            case 24: // a
                loadWord(isLongWord, operand);
                regA = ((regA + regM) & 0xfffffffffL);
                addCycles(4);
                break;
            case 25: // w
                loadWord(isLongWord, operand);
                if (isLongWord)
                {
                    UInt64 tmp = regR;
                    UInt64 signOfM, signOfA;
                    regR = regA;
                    regA = tmp;
                    signOfM = ((regM & bit_35) << 1);
                    signOfA = 0;
                    for(int i=0; i<35; i++)
                    {
                        if ((regR & 1) != 0) {
                            regA = (((regA | signOfA) +(regM | signOfM)) 
                                    & 0x1fffffffffL);
                        }
                        else
                        {
                            regA = (regA | signOfA);
                        }
                        regR = (regR >> 1);
                        regR |= ((regA & 1) << 35);
                        signOfA = (regA & bit_36);
                        regA = (regA >> 1);
                    }
                    if ((regR & 1) != 0) {
                        regA = ((regA - regM) & 0xfffffffffL);
                    }
                    regR = (regR >> 1);
                    addCycles(44);
                }
                else
                {
                    UInt64 tmp = regR;
                    UInt64 signOfM, signOfA;
                    regR = regA;
                    regA = tmp;
                    signOfM = ((regM & bit_35) << 1);
                    signOfA = 0;
                    for(int i=0; i<17; i++)
                    {
                        if ((regR & bit_18) != 0) {
                            regA = (((regA | signOfA) +(regM | signOfM))
                                    & 0x1fffffffffL);
                        }
                        regR = (regR >> 1);
                        regR |= ((regA & 1) << 35);
                        signOfA = (regA & bit_36);
                        regA = (regA >> 1);
                    }
                    if ((regR & bit_18) != 0) {
                        regA = ((regA - regM) & 0xfffffffffL);
                    }
                    regR = ((regR >> 1) & (bit_35 - bit_18));
                    addCycles(26);
                }
                break;
            case 26: // j
                if (isLongWord) 
                {
                    regK = operand;
                    regX = readShortWord(regK);
                } 
                else
                {
                    Debug.Log("Unused: j");
                }
                addCycles(4);
                break;
            case 29: // q
                loadWord(isLongWord, operand);
                regA = regR;
                regR = regM;
                addCycles(4);
                break;
            case 30: // k
                if (isLongWord) 
                {
                    if ((regA & bit_35) == 0) 
                    {
                        regK = operand;
                        regX = readShortWord(regK);
                    }
                } 
                else
                {
                    if ((regA & bit_35) != 0) 
                    {
                        regK = operand;
                        regX = readShortWord(regK);
                    }
                }
                addCycles(8);
                break;
            default:
                Debug.Log("UnImplemented, K=" + regK);
                Debug.Log("op: " + opCode + ", long: " + isLongWord + 
                ", operand: " + operand); 
                isStopped = true;
                break;
        }
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        for(int i = 0; i < data.Length; i+= channels)
        {
            if (audioPulses.Count > 0)
            {
                Tuple<uint, bool> pulse = audioPulses.Peek();
                if (elapsedTimeAudio > pulse.Item1 * 0.0001f)
                {
                    audioPulses.Dequeue();
                    if (pulse.Item2) 
                    {
                        currentAudioLevel = 0.5f;
                    }
                    else
                    {
                        currentAudioLevel = 0.0f;
                    }
                }          
            }
            data[i] = currentAudioLevel;
           
            if(channels == 2)
            {
                data[i+1] = data[i];
            }
           elapsedTimeAudio += 1.0f/sampleRate;
        }
    }
}
