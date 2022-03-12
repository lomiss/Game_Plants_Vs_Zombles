using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunFlower : PlantBase
{
    // 创建阳光所需要的时间
    private float createSunTime = 24;
    // 太阳花变红时所需要的时间
    private float goldWantTime = 1.5f;
    // 赋值最大血量
    public override float MaxHp
    {
        get
        {
            return 300;
        }
    }
    
    // 重载初始化放置函数，这里对于太阳花而言，需要每隔一段时间创建太阳花
    protected override void OnInitForPlace()
    {
        InvokeRepeating("CreateSun", createSunTime, createSunTime);
    }
    
    // 创建阳光
    private void CreateSun()
    {
        StartCoroutine(ColorEF(goldWantTime, new Color(1,0.6f,0),0.05f, InstantiateSun));
    }
    
    private void InstantiateSun()
    {
        Sun sun = PoolManager.Instance.GetObj(GameManager.instance.GameConf.Sun).GetComponent<Sun>();
        sun.transform.SetParent(transform);
        // 生成阳光，由于从缓存池中取出来，只是改变了阳光实例的父物体，为太阳花，但是并没有改变位置，所以初始化时要把太阳花的位置传进去
        sun.InitForSunFlower(transform.position);
    }
}
