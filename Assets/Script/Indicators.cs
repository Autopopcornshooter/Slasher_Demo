using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Indicators : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI PlayerLV;
    [SerializeField]
    private TextMeshProUGUI MonsterKill;
    [SerializeField]
    private TextMeshProUGUI Time;
    [SerializeField]
    private Slider EXP_Bar;

    private void FixedUpdate()
    {
        Refresh_PlayerLV();
        Refresh_MonsterKill();
        Refresh_EXPslider();
        Refresh_Time();

    }
    public void Refresh_PlayerLV()
    {
        PlayerLV.text = PlayerCtrl.Instance().playerLV.ToString();
    }
    public void Refresh_MonsterKill()
    {
        MonsterKill.text = PlayerCtrl.Instance().killPoint + "Kill";
    }
    public void Refresh_Time()
    {
        int time = GameManager.Instance().GetTime();
        int minute=time/60;
        int sec = time - minute * 60;

        Time.text = minute + ":" + sec;
    }
    public void Refresh_EXPslider()
    {
        PlayerCtrl playerCtrl=PlayerCtrl.Instance();
        EXP_Bar.value = playerCtrl.playerEXP / playerCtrl.GetMaxEXP();
    }
}
