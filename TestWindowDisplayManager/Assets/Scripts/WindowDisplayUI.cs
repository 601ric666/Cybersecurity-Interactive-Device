using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using Forms = System.Windows.Forms;
using Microsoft.Win32;

public class WindowDisplayUI : MonoBehaviour
{
    [Header("主螢幕 UI 元件")]
    public Button cloneButton;
    public Button extendButton;
    public Button moveButton;
    public Button activateDisplayAtIndexButton;
    public Dropdown screenDropdown;
    public Text infoText;

    private int targetScreenIndex = 0;
    private Coroutine activateDisplay2Routine;

    void Start()
    {
        cloneButton.onClick.AddListener(SwitchToClone);
        extendButton.onClick.AddListener(SwitchToExtend);
        moveButton.onClick.AddListener(MoveToTargetScreen);
        activateDisplayAtIndexButton.onClick.AddListener(ActivateDisplayAtIndex);
        screenDropdown.onValueChanged.AddListener(OnDropdownChanged);
        SystemEvents.DisplaySettingsChanged += OnDisplayChanged;

        UpdateScreenList();
        UpdateInfo();

        StartCoroutine(ActivateExtendAfterDelay());
    }

    void OnDestroy()
    {
        SystemEvents.DisplaySettingsChanged -= OnDisplayChanged;
    }

    void OnDropdownChanged(int index)
    {
        targetScreenIndex = index;
        UpdateInfo();
    }

    void OnDisplayChanged(object sender, EventArgs e)
    {
        UnityEngine.Debug.Log("螢幕設定變更偵測到");
        UpdateScreenList();
        UpdateInfo();
    }

    public void SwitchToClone()
    {
        Process.Start("cmd.exe", "/C DisplaySwitch.exe /clone");
        UpdateInfoText("切換到鏡像模式");
        UpdateScreenList();
    }

    public void SwitchToExtend()
    {
        Process.Start("cmd.exe", "/C DisplaySwitch.exe /extend");
        UpdateInfoText("切換至延伸模式中...");

        if (activateDisplay2Routine != null)
            StopCoroutine(activateDisplay2Routine);

        activateDisplay2Routine = StartCoroutine(ActivateDisplay2AfterExtendedDisplayReady());
        UpdateScreenList();
    }

    IEnumerator ActivateDisplay2AfterExtendedDisplayReady()
    {
        yield return new WaitForSeconds(3f);
        AutoAssignDisplayToRightmostScreen();
    }

    IEnumerator ActivateExtendAfterDelay()
    {
        yield return new WaitForSeconds(0.05f);
        SwitchToExtend();
    }

    void AutoAssignDisplayToRightmostScreen()
    {
        var screens = Forms.Screen.AllScreens;
        int rightmostIndex = 0;
        int maxX = int.MinValue;

        for (int i = 0; i < screens.Length; i++)
        {
            if (screens[i].Bounds.X > maxX)//偵測最右側螢幕範圍
            {
                maxX = screens[i].Bounds.X;
                rightmostIndex = i;
            }
        }

        if (rightmostIndex < Display.displays.Length)
        {
            //Display.displays[rightmostIndex].Deactivate(); 根本沒這東西ai掰的

            Display.displays[rightmostIndex].Activate();//啟動最右側螢幕
            UnityEngine.Debug.Log($"已啟用最右邊螢幕：Display {rightmostIndex}");
            UpdateInfoText($"已啟用最右邊螢幕：Display {rightmostIndex}");
        }
        else
        {
            UnityEngine.Debug.LogWarning("Unity 的 Display index 不包含最右側螢幕");
        }
    }

    public void MoveToTargetScreen()
    {
        var screens = Forms.Screen.AllScreens;

        if (targetScreenIndex >= screens.Length)
        {
            UnityEngine.Debug.LogWarning($"螢幕 {targetScreenIndex} 不存在");
            return;
        }

        var screen = screens[targetScreenIndex];

        IntPtr hwnd = GetActiveWindow();
        MoveWindow(hwnd, screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width, screen.Bounds.Height, true);

        UnityEngine.Debug.Log($"視窗已移動到螢幕 {targetScreenIndex}");
        UpdateInfoText($"視窗已移動到螢幕 {targetScreenIndex}");
        UpdateScreenList();
    }

    public void ActivateDisplayAtIndex()
    {
        int index = targetScreenIndex;

        if (index >= Display.displays.Length)
        {
            UnityEngine.Debug.LogWarning($"Display.displays 中沒有 index {index}");
            return;
        }

        var screens = Forms.Screen.AllScreens;
        if (index < screens.Length)
        {
            var bounds = screens[index].Bounds;
            Display.displays[index].Activate(bounds.Width, bounds.Height, 60);
            UpdateInfoText($"Display {index} 啟用並設為 {bounds.Width}x{bounds.Height}");
        }
        else
        {
            Display.displays[index].Activate();
            UpdateInfoText($"Display {index} 啟用（使用預設解析度）");
        }

        UnityEngine.Debug.Log($"Display {index} 已啟用");
        UpdateScreenList();
    }

    void UpdateScreenList()
    {
        screenDropdown.ClearOptions();
        var screens = Forms.Screen.AllScreens;

        List<string> options = new List<string>();
        for (int i = 0; i < screens.Length; i++)
        {
            options.Add($"螢幕 {i} ({screens[i].Bounds.Width}x{screens[i].Bounds.Height})");
        }

        screenDropdown.AddOptions(options);

        if (targetScreenIndex >= screens.Length)
            targetScreenIndex = 0;

        screenDropdown.value = targetScreenIndex;
    }

    void UpdateInfo()
    {
        var screens = Forms.Screen.AllScreens;
        string info = $"螢幕數量: {screens.Length}\n";

        for (int i = 0; i < screens.Length; i++)
        {
            info += $"螢幕 {i}: {screens[i].Bounds.Width}x{screens[i].Bounds.Height} @ {screens[i].Bounds.X},{screens[i].Bounds.Y}\n";
        }

        info += $"\n目前目標螢幕: {targetScreenIndex}";

        if (infoText != null)
            infoText.text = info;
    }

    void UpdateInfoText(string message)
    {
        if (infoText != null)
            infoText.text = message;
    }

    // Windows API
    [DllImport("user32.dll")]
    static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
}
