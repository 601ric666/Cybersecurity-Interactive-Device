using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOnlyOneLayer : MonoBehaviour
{
    [Header("指定只顯示的 Layer 名稱")]
    public string targetLayerName = "UI_Display2";

    void Start()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("找不到 Main Camera！");
            return;
        }

        int targetLayer = LayerMask.NameToLayer(targetLayerName);
        if (targetLayer == -1)
        {
            Debug.LogError($"Layer '{targetLayerName}' 不存在，請先新增此 Layer");
            return;
        }

        // 將相機的 Culling Mask 設成只顯示目標 Layer
        cam.cullingMask = 1 << targetLayer;
    }
}
