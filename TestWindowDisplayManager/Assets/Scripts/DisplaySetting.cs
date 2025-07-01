using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySetting : MonoBehaviour
{
    [Header("顯示螢幕數量的文字")]
    public Text displayInfoText;  // UI Text，需在 Inspector 指定

    private Coroutine autoSetting;
    private string info;

    void Start()
    {
        autoSetting = StartCoroutine(AutoSetting());
    }

    public void StartAutoSetting()
    {
        if (autoSetting != null)
        {
            StopCoroutine(autoSetting);
        }
        autoSetting = StartCoroutine(AutoSetting());
    }

    IEnumerator AutoSetting()
    {
        Debug.Log("=== DisplaySetting 啟動 ===");

        info = "";

        for (int i = 1; i <= 8; i++)  // 預設最多到 displays[8]
        {
            if (i < Display.displays.Length)
            {
                int width = Display.displays[i].renderingWidth;
                int height = Display.displays[i].renderingHeight;

                if (width > 0 && height > 0)
                {
                    Display.displays[i].Activate();
                    string msg = $"Display {i} 已啟用，解析度 {width} x {height}\n";
                    Debug.Log(msg);
                    info += msg;
                }
                else
                {
                    string msg = $"Display {i} 不可用（解析度為 0）\n";
                    Debug.LogWarning(msg);
                    info += msg;
                }
            }
            else
            {
                string msg = $"Display {i} 尚未存在\n";
                Debug.Log(msg);
                info += msg;
            }

            yield return new WaitForSeconds(1f);
        }

        info = "偵測到的螢幕數量: " + Display.displays.Length + "\n";
        Debug.Log(info);

        if (displayInfoText != null)
        {
            displayInfoText.text = info + $"\n運作時間: {Time.time:F1} 秒";
        }
    }

    public void DisableAndRestartSetting()
    {
        Debug.Log("停用除主螢幕外的所有Display");

        if (autoSetting != null)
        {
            StopCoroutine(autoSetting);
        }

        string info = "僅啟用主螢幕\n";
        info += "偵測到的螢幕數量: " + Display.displays.Length + "\n";

        Debug.Log(info);

        if (displayInfoText != null)
        {
            displayInfoText.text = info + $"\n運作時間: {Time.time:F1} 秒";
        }

        StartCoroutine(RestartAfterDelay());
    }

    IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        StartAutoSetting();
    }
}
