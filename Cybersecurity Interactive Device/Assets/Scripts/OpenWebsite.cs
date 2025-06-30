using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Runtime.InteropServices;

public class OpenWebsite : MonoBehaviour
{
    [Header("要開啟的網址")]
    public string url = "https://www.google.com";

    [Header("Windows 選項")]
    public bool maximizeOnWindows = false;
    public BrowserType browserType = BrowserType.Default;

    public enum BrowserType
    {
        Default,  // 用系統預設瀏覽器
        Chrome,
        Edge,
        Firefox
    }

    public void OpenURL()
    {
        if (string.IsNullOrEmpty(url))
        {
            UnityEngine.Debug.LogWarning("網址為空，無法開啟！");
            return;
        }

#if UNITY_STANDALONE_WIN
        if (maximizeOnWindows && browserType != BrowserType.Default)
        {
            OpenURLMaximizedWindows();
            return;
        }
#endif
        // 通用方式
        Application.OpenURL(url);
    }

#if UNITY_STANDALONE_WIN
    private void OpenURLMaximizedWindows()
    {
        string browserCommand = "";

        switch (browserType)
        {
            case BrowserType.Chrome:
                browserCommand = $"start chrome --start-maximized {url}";
                break;
            case BrowserType.Edge:
                browserCommand = $"start msedge --start-maximized {url}";
                break;
            case BrowserType.Firefox:
                browserCommand = $"start firefox -new-window {url}";
                break;
            default:
                Application.OpenURL(url);
                return;
        }

        try
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c {browserCommand}")
            {
                CreateNoWindow = true,
                UseShellExecute = false
            });
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError("開啟瀏覽器失敗：" + ex.Message);
            Application.OpenURL(url);
        }
    }
#endif
}
