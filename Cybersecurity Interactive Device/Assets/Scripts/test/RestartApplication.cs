using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class RestartApplication : MonoBehaviour
{

    public void Restart()
    {
        string exePath = Application.dataPath.Replace("_Data", ".exe");
        if (File.Exists(exePath))
        {
            Process.Start(exePath);  // 啟動新的應用程式
            //Application.Quit();      // 關閉當前應用程式
        }
    }
}
