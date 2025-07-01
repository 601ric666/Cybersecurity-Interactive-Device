using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class WindowDisplayUI : MonoBehaviour
{
    [Header("主螢幕 UI 元件")]
    public Button cloneButton;
    public Button extendButton;
    public Button displayMoveButton;  // 單一按鈕
    public Dropdown screenDropdown;
    public Text infoText;

    void Start()
    {
        cloneButton.onClick.AddListener(SwitchToClone);
        extendButton.onClick.AddListener(SwitchToExtend);
        displayMoveButton.onClick.AddListener(MoveDisplay0To1AndDisplay1To2);

        UpdateScreenDropdown();
        UpdateInfo($"目前偵測到 {Display.displays.Length} 個螢幕");

        // 啟動所有螢幕
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }

        // 預設 Canvas 分配
        ApplyDefaultCanvasToDisplays();
    }

    void ApplyDefaultCanvasToDisplays()
    {
        MoveDisplay0To1AndDisplay1To2();
        UpdateInfo("已將 Canvas 預設分配到對應螢幕 (Display0→螢幕1, Display1→螢幕2)");
    }

    public void SwitchToClone()
    {
        Process.Start("cmd.exe", "/C DisplaySwitch.exe /clone");
        UpdateInfo("切換為鏡像模式 (Clone)");
    }

    public void SwitchToExtend()
    {
        Process.Start("cmd.exe", "/C DisplaySwitch.exe /extend");
        UpdateInfo("切換為擴展模式 (Extend)");
    }

    /// <summary>
    /// Display0 → 螢幕1 (index 0)
    /// Display1 → 螢幕2 (index 1)
    /// </summary>
    void MoveDisplay0To1AndDisplay1To2()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>();

        int count0 = 0;//執行一次
        int count1 = 0;

        foreach (var canvas in canvases)
        {
            if (canvas.targetDisplay == 0)
            {
                canvas.targetDisplay = 0; // 螢幕1
                count0++;
            }
            else if (canvas.targetDisplay == 1)
            {
                canvas.targetDisplay = 1; // 螢幕2
                count1++;
            }
        }

        UpdateInfo($"Display0 Canvas 移動到螢幕1：{count0} 個\nDisplay1 Canvas 移動到螢幕2：{count1} 個");
    }

    void UpdateScreenDropdown()
    {
        screenDropdown.ClearOptions();
        int displayCount = Display.displays.Length;
        var options = new System.Collections.Generic.List<string>();
        for (int i = 0; i < displayCount; i++)
        {
            options.Add($"螢幕 {i + 1}");
        }
        screenDropdown.AddOptions(options);
        screenDropdown.value = 0;
        screenDropdown.RefreshShownValue();
    }

    void UpdateInfo(string message)
    {
        if (infoText != null)
        {
            infoText.text = message;
            UnityEngine.Debug.Log(message);
        }
    }
}
