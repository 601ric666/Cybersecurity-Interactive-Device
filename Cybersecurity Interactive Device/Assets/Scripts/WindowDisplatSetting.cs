using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class WindowDisplaySwitch : MonoBehaviour
{
    public DisplaySetting displaySetting;
    private int PlayTwice = 0;
    private Coroutine invokee;

    void Awake()
    {

    }

    public void SwitchToClone()//鏡像
    {
        Process.Start("cmd.exe", "/C DisplaySwitch.exe /clone");
        invokee = StartCoroutine(Invokee());
    }
    public void SwitchToExtend()//擴展
    {
        Process.Start("cmd.exe", "/C DisplaySwitch.exe /extend");
        invokee = StartCoroutine(Invokee());
    }


    IEnumerator Invokee()
    {
        if(PlayTwice <= 2)
        {
            yield return new WaitForSeconds(2f);
            invokee = StartCoroutine(Invokee());
            //displaySetting.DisableAndRestartSetting();
            PlayTwice++;
        }
        else
        {
            PlayTwice = 0;
        }
        
    }
}
