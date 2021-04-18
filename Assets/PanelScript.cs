using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelScript : MonoBehaviour
{
    public GameObject indicatorPrefab;

    [SerializeField]
    private PushButtonScript pbClearStartScript, pbInitialStartScript, pbRestartScript, 
        pbInitialLoadScript, pbFreeRunScript, pbStopScript, pbSoundScript;

    private GameObject[,] registerIndicators;
    private GameObject[] orIndicators, sccIndicators;
    private GameObject freeRunIndicator, soundIndicator;
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_WEBGL
#else
        pbSoundScript.gameObject.SetActive(true);
#endif
        registerIndicators = new GameObject[3, 36];
        for(int i=0; i<3; i++) 
        {
            for(int j=0; j<36; j++)
            {
                registerIndicators[i, j] = Instantiate(indicatorPrefab, 
                    transform.position + 
                    new Vector3(-0.16f + (j +(j/9)) * 0.01f, 0.05f- i * 0.02f, -0.05f), 
                    Quaternion.identity);
                registerIndicators[i, j].GetComponent<IndicatorScript>().setState(false);
            }
        }
        orIndicators = new GameObject[18];
        for(int j=0; j<18; j++)
        {
            orIndicators[j] = Instantiate(indicatorPrefab, 
                transform.position + 
                new Vector3(-0.16f + (j +(j/6)) * 0.01f, -0.02f, -0.05f),
                Quaternion.identity);
            orIndicators[j].GetComponent<IndicatorScript>().setState(false);
        }
        sccIndicators = new GameObject[11];
        for(int j=0; j<11; j++)
        {
            sccIndicators[j] = Instantiate(indicatorPrefab, 
                transform.position + 
                new Vector3(-0.16f + j * 0.01f, -0.04f, -0.05f),
                Quaternion.identity);
            sccIndicators[j].GetComponent<IndicatorScript>().setState(false);
        }
        freeRunIndicator = Instantiate(indicatorPrefab, 
                transform.position + 
                new Vector3(0.188f, -0.06f, -0.05f),
                Quaternion.identity);
        freeRunIndicator.GetComponent<IndicatorScript>().setState(false);
#if UNITY_WEBGL
#else
        soundIndicator = Instantiate(indicatorPrefab, 
                transform.position + 
                new Vector3(0.1f, -0.06f, -0.05f),
                Quaternion.identity);
        soundIndicator.GetComponent<IndicatorScript>().setState(false);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool clearStartPushed() 
    {
        return pbClearStartScript.isPushed();
    }

    public bool initialStartPushed() 
    {
        return pbInitialStartScript.isPushed();
    }

    public bool restartPushed() 
    {
        return pbRestartScript.isPushed();
    }

    public bool initialLoadPushed() 
    {
        return pbInitialLoadScript.isPushed();
    }

    public bool stopPushed() 
    {
        return pbStopScript.isPushed();
    }

    public bool freeRunPushed() 
    {
        return pbFreeRunScript.isPushed();
    }

    public bool soundPushed() 
    {
        return pbSoundScript.isPushed();
    }

    public void setRegisterIndicator(int i, UInt64 d)
    {
        for(int j = 0; j < 36; j++) 
        {
            registerIndicators[i, j].GetComponent<IndicatorScript>().setState(((d >> (35-j)) & 1) != 0);
        }
    }

    public void setOrIndicator(UInt64 d)
    {
        for(int j = 0; j < 18; j++) 
        {
            orIndicators[j].GetComponent<IndicatorScript>().setState(((d >> (17-j)) & 1) != 0);
        }
    }

    public void setSccIndicator(UInt16 d)
    {
        for(int j = 0; j < 11; j++) 
        {
            sccIndicators[j].GetComponent<IndicatorScript>().setState(((d >> (10-j)) & 1) != 0);
        }
    }

    public void setFreeRunIndicator(bool b)
    {
        freeRunIndicator.GetComponent<IndicatorScript>().setState(b);
    }

    public void setSoundIndicator(bool b)
    {
#if UNITY_WEBGL
#else
        soundIndicator.GetComponent<IndicatorScript>().setState(b);
#endif
    }
}
