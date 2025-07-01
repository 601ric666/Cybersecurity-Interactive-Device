using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class WindowDisplaySwitch : MonoBehaviour
{
    public DisplaySetting displaySetting;
    private int playTwice = 0;
    private Coroutine invokee;

    public void SwitchToClone() // 鏡像
    {
        Process.Start("cmd.exe", "/C DisplaySwitch.exe /clone");
        invokee = StartCoroutine(Invokee());
    }

    public void SwitchToExtend() // 擴展
    {
        Process.Start("cmd.exe", "/C DisplaySwitch.exe /extend");
        invokee = StartCoroutine(Invokee());
    }

    IEnumerator Invokee()
    {
        if (playTwice <= 2)
        {
            yield return new WaitForSeconds(2f);
            invokee = StartCoroutine(Invokee());
            // 若需要可以加上自動重新偵測
            // displaySetting.DisableAndRestartSetting();
            playTwice++;
        }
        else
        {
            playTwice = 0;
        }
    }
}
