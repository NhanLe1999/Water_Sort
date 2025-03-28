using System.IO;
using UnityEngine;

public class TakeScreenShot : MonoBehaviour
{
#if UNITY_EDITOR
    private string folderPath;
    // Update is called once per frame

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    private void Start()
    {
        var root = new DirectoryInfo(Application.dataPath);
        folderPath = root.Parent.Parent.FullName + "/Screenshots/";


        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            string filePath = folderPath + "/Screenshot_" + System.DateTime.Now.Ticks.ToString() + ".png";
            ScreenCapture.CaptureScreenshot(filePath);
            Debug.Log("TakeSceenshot: " + filePath);
        }
    }
#endif
}
