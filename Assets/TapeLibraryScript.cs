using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TapeLibraryScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject miniTapePrefab;

    private GameObject[] tapes;

    private TapeScript readTapeScript;
    [SerializeField]
    private Texture2D font;
    [SerializeField]
    private List<TextAsset> textAssets;

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
#endif

    void Start()
    {
#if UNITY_WEBGL
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
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void setTapeScript(TapeScript t)
    {
        if (readTapeScript != null)
            return;
        readTapeScript = t;

#if UNITY_WEBGL
        tapes = new GameObject[tapeList.Count];

        int i = 0;
        foreach(TapePair tp in tapeList) {
            tapes[i] = Instantiate(miniTapePrefab,
                t.gameObject.transform.position + new Vector3(-0.025f, 0.12f + 0.01f * i, 0.0f), 
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
                t.gameObject.transform.position + new Vector3(-0.025f, 0.12f + 0.01f * i, -0.05f), 
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
        readTapeScript.readString(m.getData());
        for (int i = 0; i < tapes.Length; i++)
        {
            Destroy(tapes[i]);
        }
        tapes = null;
        readTapeScript = null;
    }

    public void save(string s)
    {
        string fileNameBase, fileName;

#if UNITY_WEBGL
        fileNameBase = System.DateTime.Now.ToString("yyyyMMddHHmmss");
        fileName = fileNameBase + ".ptp";
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
