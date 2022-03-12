using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    // 初始的阳光数量
    private int sunNum = 100;
    public int SunNum
    {
        get => sunNum;
        set
        {
            sunNum = value;
            UIManager.instance.UpdateSunNum(sunNum);
            if (SunNumUpdateAction != null) SunNumUpdateAction();
        }
    }
    // 阳光数量更新时的事件
    private UnityAction SunNumUpdateAction;
    private void Awake()
    {
        instance = this;
        
    }
    
    /// <summary>
    /// 添加阳光数量更新时的事件监听
    /// </summary>
    public void AddSunNumUpdateActionListener(UnityAction action)
    {
        SunNumUpdateAction += action;
    }
}
