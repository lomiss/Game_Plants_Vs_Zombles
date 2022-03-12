using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SkySunManager : MonoBehaviour
{
    public static SkySunManager instance;
    private void Awake()
    {
        instance = this;
    }

    // 创建阳光时候的Y坐标
    private float createSunPosY = 6;

    // 创建阳光时最左和最右的X轴坐标，在这个范围内随机
    private float createSunMaxPosX = 3f;
    private float createSunMinPosX = -7.5f;
    
    // 阳光下落时最大和最小的Y轴坐标，在这个范围内随机
    private float sunDownMaxPosY = 2.5f;
    private float sunDownMinPosY = -3.7f;

    public void StartCreateSun(float delay)
    {
        // 获取预制体
        
        InvokeRepeating("CreateSun", delay,delay);
    }

    public void StopCreateSun()
    {
        CancelInvoke();
    }

    void CreateSun()
    {
        // 实例化后并获得Sun脚本对象
        Sun sun = PoolManager.Instance.GetObj(GameManager.instance.GameConf.Sun).GetComponent<Sun>();
        sun.transform.SetParent(transform);
        // 下落点Y和开始点X的随机
        float donwY = Random.Range(sunDownMaxPosY, sunDownMinPosY);
        float createX = Random.Range(createSunMinPosX, createSunMaxPosX);
        sun.InitForSky(donwY, createX, createSunPosY);
    }
}
