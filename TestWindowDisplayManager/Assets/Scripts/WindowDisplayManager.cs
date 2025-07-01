using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Microsoft.Win32;
using Forms = System.Windows.Forms;

public class WindowDisplayManager : MonoBehaviour
{
    [Header("目標螢幕索引（0 = 主螢幕）")]
    public int targetScreenIndex = 0;

    void OnEnable()
    {
        SystemEvents.DisplaySettingsChanged += OnDisplayChanged;
    }

    void OnDisable()
    {
        SystemEvents.DisplaySettingsChanged -= OnDisplayChanged;
    }

    void Start()
    {
        MoveToTargetScreen();
    }

    void OnDisplayChanged(object sender, EventArgs e)
    {
        Debug.Log("螢幕設定變更偵測到，開始重新配置視窗。");
        MoveToTargetScreen();
    }

    public void MoveToTargetScreen()
    {
        var screens = Forms.Screen.AllScreens;

        if (targetScreenIndex >= screens.Length)
        {
            Debug.LogWarning($"指定的螢幕 {targetScreenIndex} 不存在，目前螢幕數量：{screens.Length}");
            return;
        }

        var screen = screens[targetScreenIndex];

        int screenX = screen.Bounds.X;
        int screenY = screen.Bounds.Y;
        int width = screen.Bounds.Width;
        int height = screen.Bounds.Height;

        Debug.Log($"移動到螢幕 {targetScreenIndex}，位置({screenX},{screenY})，尺寸 {width}x{height}");

        IntPtr hwnd = GetActiveWindow();
        MoveWindow(hwnd, screenX, screenY, width, height, true);
    }

    // Windows API
    [DllImport("user32.dll")]
    static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
}
