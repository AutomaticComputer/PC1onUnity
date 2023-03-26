using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

public class TapeLibraryScript : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void TapeUpdownInit();
    [DllImport("__Internal")]
    private static extern void AddTapeDownload(string name, string data);
#endif

    public GameObject miniTapePrefab;
    public GameObject renamePanelPrefab;
    public GameObject deletePanelPrefab;
    public GameObject errorPanelPrefab;
    public GameObject canvas;

    private GameObject[] tapes;

    private TapeScript readTapeScript;
    private Vector3 minitapePos;
    private GameObject dialogPanel;
    [SerializeField]
    private Texture2D font;
    [SerializeField]
    private List<TextAsset> textAssets;

    private string currentFileName;
    [SerializeField]
    private GlobalVariablesScript globalVariablesScript;

    private enum Mode {
        Idle, Save, Load, Delete, Rename
#if UNITY_WEBGL && !UNITY_EDITOR
        , Download
#endif
    }
    private Mode currentMode;

    private byte[] ascii_to_tape = new byte[]
        {
            // | for 0, ^ for 27, _ for 31, [ for 32, # for 63
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 0, 0, 2, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            4, 0, 0, 63, 0, 0, 0, 0, 0, 0, 0, 40, 38, 34, 39, 0, 
            56, 57, 48, 42, 33, 53, 60, 44, 35, 45, 47, 0, 0, 55, 0, 0, 
            0, 24, 19, 14, 18, 16, 22, 11, 5, 12, 26, 30, 9, 7, 6, 3, 
            13, 29, 10, 20, 1, 28, 15, 25, 23, 21, 17, 32, 0, 0, 27, 31, 
            0, 24, 19, 14, 18, 16, 22, 11, 5, 12, 26, 30, 9, 7, 6, 3, 
            13, 29, 10, 20, 1, 28, 15, 25, 23, 21, 17, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 
        };
#if UNITY_WEBGL
    private class TapePair 
    { 
        public TapePair(string n, string c) 
        {
            name = n;
            content = c;
        }
        public string name;
        public string content;
    };
    private List<TapePair> tapeList;

    private string uploadName;

    IEnumerator LoadTape(string url) {
        WWW www = new WWW (url);
        yield return www;
        save(www.text, uploadName);
    }

    void UploadName(string name) {
        uploadName = name;
    }
    void Upload(string url) {
        StartCoroutine(LoadTape(url));
    }
#endif

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        TapeUpdownInit(); 

        tapeList = new List<TapePair>();
        foreach(TextAsset ta in textAssets) {
            tapeList.Add(new TapePair(ta.name, ta.ToString()));
        }
#else
        string filePath = Application.persistentDataPath + @"/Tapes/";
        if (!Directory.Exists(filePath)) 
            Directory.CreateDirectory(filePath);
        string versionPath = Application.persistentDataPath + @"/Tapes/version";
        string verString = "";
        if (File.Exists(versionPath)) {
            verString = File.ReadAllText(versionPath);
        }
        if (verString != Application.version) {
            foreach(TextAsset ta in textAssets) {
                File.WriteAllText(filePath + ta.name, ta.ToString());
            }
            File.WriteAllText(versionPath, Application.version);
        }
#endif
        readTapeScript = null;

        currentMode = Mode.Idle;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void setTapeScript(TapeScript t)
    {
        if (currentMode != Mode.Idle)
            return;
//        if (readTapeScript != null)
//            return;
        readTapeScript = t;
        minitapePos = t.gameObject.transform.position 
            + new Vector3(0.0f, 0.10f, 0.0f);
        currentMode = Mode.Load;
        instantiateTapes();
    }

    public void startRename(Vector3 pos)
    {
        if (currentMode != Mode.Idle)
            return;
        currentMode = Mode.Rename;
        minitapePos = pos;
        instantiateTapes();
    }
    public void startDelete(Vector3 pos)
    {
        if (currentMode != Mode.Idle)
            return;
        currentMode = Mode.Delete;
        minitapePos = pos;
        instantiateTapes();
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    public void startDownload(Vector3 pos)
    {
        if (currentMode != Mode.Idle)
            return;
        currentMode = Mode.Download;
        minitapePos = pos;
        instantiateTapes();
    }
#endif

    public void instantiateTapes()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        tapes = new GameObject[tapeList.Count];

        int i = 0;
        foreach(TapePair tp in tapeList) {
            tapes[i] = Instantiate(miniTapePrefab,
                minitapePos + new Vector3(0.0f, 0.01f * i, 0.0f), 
                miniTapePrefab.transform.rotation);
            tapes[i].GetComponent<MiniTapeScript>().setName(tp.name);
            tapes[i].GetComponent<MiniTapeScript>().setData(tp.content);
            tapes[i].GetComponent<MiniTapeScript>().setTapeLibraryScript(this);
            tapes[i].GetComponent<MiniTapeScript>().setFont(font);
            i++;
        }
#else
        string filePath = Application.persistentDataPath + @"/Tapes/";

        DirectoryInfo dir = new DirectoryInfo(filePath);
        List<FileInfo> infoList = new List<FileInfo>();

        infoList.AddRange(dir.GetFiles("*.ptw"));
        infoList.AddRange(dir.GetFiles("*.ptr"));
        infoList.AddRange(dir.GetFiles("*.ptp"));
        infoList.AddRange(dir.GetFiles("*.txt"));
        tapes = new GameObject[infoList.Count];

        int i = 0;
        foreach (FileInfo f in infoList)
        {
            tapes[i] = Instantiate(miniTapePrefab,
                minitapePos + new Vector3(0.0f, 0.01f * i, 0.0f), 
                miniTapePrefab.transform.rotation);
            tapes[i].GetComponent<MiniTapeScript>().setName(f.Name);
            tapes[i].GetComponent<MiniTapeScript>().setData(System.IO.File.ReadAllText(f.ToString()));
            tapes[i].GetComponent<MiniTapeScript>().setTapeLibraryScript(this);
            tapes[i].GetComponent<MiniTapeScript>().setFont(font);
            i++;
        }
#endif
    }
    public void selected(MiniTapeScript m)
    {
        currentFileName = m.getName();
        switch(currentMode) {
            case Mode.Load:
                if (currentFileName.Substring(currentFileName.Length - 4, 4).Equals(".txt")) {
                    string s;
                    byte[] tapeData = System.Text.Encoding.ASCII.GetBytes(m.getData());
                    for(int i = 0; i < tapeData.Length; i++) 
                    {
                        tapeData[i] = ascii_to_tape[tapeData[i]];
                    }
                    readTapeScript.readByteArray(tapeData);
                }
                else 
                {
                    readTapeScript.readString(m.getData());
                }
                readTapeScript = null;
                currentMode = Mode.Idle;
                break;
            case Mode.Delete:
                dialogPanel = Instantiate(deletePanelPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                dialogPanel.transform.SetParent(canvas.transform, false);
                dialogPanel.transform.Find("DeleteText").gameObject.GetComponent<Text>().text = 
                    "Delete \"" + currentFileName + "\" ?";
                {
                    UIButtonScript b;
                    b = dialogPanel.transform.Find("DeleteButton").gameObject.GetComponent<UIButtonScript>();
                    b.tapeLibrary = this;
                    b.action = "Delete";
                    b = dialogPanel.transform.Find("CancelButton").gameObject.GetComponent<UIButtonScript>();
                    b.tapeLibrary = this;
                    b.action = "Cancel";
                }
                break;
            case Mode.Rename:
                globalVariablesScript.setKeyDisabled(true);
                dialogPanel = Instantiate(renamePanelPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                dialogPanel.transform.SetParent(canvas.transform, false);
                dialogPanel.transform.Find("RenameText").gameObject.GetComponent<Text>().text = 
                    "Rename \"" + currentFileName + "\" to ?";
                {
                    UIButtonScript b;
                    b = dialogPanel.transform.Find("RenameButton").gameObject.GetComponent<UIButtonScript>();
                    b.tapeLibrary = this;
                    b.action = "Rename";
                    b = dialogPanel.transform.Find("CancelButton").gameObject.GetComponent<UIButtonScript>();
                    b.tapeLibrary = this;
                    b.action = "Cancel";
                }
                break;
#if UNITY_WEBGL && !UNITY_EDITOR
            case Mode.Download: 
                AddTapeDownload(currentFileName, m.getData()); 
                currentMode = Mode.Idle;
                break;
#endif
            default:
                break;
        }

        for (int i = 0; i < tapes.Length; i++)
        {
            Destroy(tapes[i]);
        }
        tapes = null;
        readTapeScript = null;
    }

    public void dialogCallback(string action)
    {
        bool moveError = false;
        if (action == "Delete")
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        for(int i = 0 ; i < tapeList.Count; i++) {
            TapePair tp = tapeList[i]; 
            if (tp.name.Equals(currentFileName)) {
                tapeList.RemoveAt(i);
                break;
            }
        }
#else
            File.Delete(Application.persistentDataPath + @"/Tapes/"
                + currentFileName);
#endif
        }

        if (action == "Rename")
        {
            string newFileName = 
                dialogPanel.transform.Find("InputField").gameObject.GetComponent<InputField>().text;
            string extension = System.IO.Path.GetExtension(newFileName);
            if (!(extension.Equals(".ptw") || extension.Equals(".ptr") || 
                extension.Equals(".ptp") || extension.Equals(".txt"))) {
                moveError = true;
            }
            else
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                for(int i = 0 ; i < tapeList.Count; i++) {
                    TapePair tp = tapeList[i]; 
                    if (tp.name.Equals(newFileName)) {
                        moveError = true;
                    }
                }
                if (!moveError) {
                    for(int i = 0 ; i < tapeList.Count; i++) {
                        TapePair tp = tapeList[i]; 
                        if (tp.name.Equals(currentFileName)) {
                            tp.name = newFileName;
                        }
                    }
                }
#else
                try 
                {
                    File.Move(
                        Application.persistentDataPath + @"/Tapes/" + currentFileName, 
                        Application.persistentDataPath + @"/Tapes/" + newFileName);
                }
                catch(Exception ex) 
                {
                    moveError = true;
                }
#endif
            }
        }

        if (action == "ErrorOk")
        {
            // Nothing to do.
        }

        Destroy(dialogPanel);

        if (moveError)
        {
            dialogPanel = Instantiate(errorPanelPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            dialogPanel.transform.SetParent(canvas.transform, false);
            {
                UIButtonScript b;
                b = dialogPanel.transform.Find("OkButton").gameObject.GetComponent<UIButtonScript>();
                b.tapeLibrary = this;
                b.action = "ErrorOk";
            }
        }

        globalVariablesScript.setKeyDisabled(false);
        currentMode = Mode.Idle;
    }


    public void save(string s, string name = null)
    {
        string fileNameBase, fileName;

#if UNITY_WEBGL && !UNITY_EDITOR
        if (name == null) {
            fileNameBase = System.DateTime.Now.ToString("yyyyMMddHHmmss");
            fileName = fileNameBase + ".ptp";
        }
        else 
        {
            fileName = name;
        }
        tapeList.Add(new TapePair(fileName, s));
#else
        fileNameBase = Application.persistentDataPath + @"/Tapes/" + System.DateTime.Now.ToString("yyyyMMddHHmmss");
        for(int i=0; ; i++) {
            if (i == 0)
                fileName = fileNameBase + ".ptp";
            else
                fileName = fileNameBase + i + ".ptp";
            if (!System.IO.File.Exists(fileName))
                break;
        }

        File.WriteAllText(fileName, s);
#endif
    }
}
