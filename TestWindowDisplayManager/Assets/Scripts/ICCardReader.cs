using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using PCSC;
using PCSC.Monitoring;

public class ICCardReader : MonoBehaviour
{
    private ISCardMonitor monitor;
    private string[] readerNames;

    [Header("Debug 訊息顯示")]
    public Text debugText;   // 指定在畫面上的 Text UI


    void Start()
    {
        InitializeReader();
    }
    void Update()
    {
        
    }

    void OnDestroy()
    {
        if (monitor != null)
        {
            monitor.Cancel();
            monitor.Dispose();
        }
    }

    private void InitializeReader()
    {
        var contextFactory = ContextFactory.Instance;
        using (var context = contextFactory.Establish(SCardScope.System))
        {
            readerNames = context.GetReaders();

            if (readerNames == null || readerNames.Length == 0)
            {
                Debug.Log("沒有偵測到讀卡機");
                debugText.text = "沒有偵測到讀卡機";
                return;
            }

            monitor = MonitorFactory.Instance.Create(SCardScope.System);

            monitor.CardInserted += (sender, args) =>
            {
                Debug.Log($"卡片已放上：{args.ReaderName}");
                debugText.text = $"卡片已放上：{args.ReaderName}";
            };

            monitor.CardRemoved += (sender, args) =>
            {
                Debug.Log($"卡片已移除：{args.ReaderName}");
                debugText.text = $"卡片已移除：{args.ReaderName}";
            };

            monitor.Initialized += (sender, args) =>
            {
                Debug.Log("讀卡監聽已啟動");
                debugText.text ="讀卡監聽已啟動";
            };

            monitor.StatusChanged += (sender, args) =>
            {
                Debug.Log($"狀態變更：{args.NewState} at {args.ReaderName}");
                //debugText.text = $"狀態變更：{args.NewState} at {args.ReaderName}";
            };

            monitor.Start(readerNames);
            Debug.Log("開始監聽 IC 卡...");
        }
    }
}