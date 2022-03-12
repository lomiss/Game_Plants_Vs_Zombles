using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public enum PlantType
{
    // 太阳花
    SunFlower,
    // 豌豆射手
    Peashooter,
    // 坚果
    WallNut,
    // 地刺
    Spike,
    // 樱桃
    Cherry
}
public class PlantManager : MonoBehaviour
{
    public static PlantManager instance;
    private void Awake()
    {
        instance = this;
    }
    /// <summary>
    /// 根据类型返回预制体
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameObject GetPlantByType(PlantType type)
    {
        switch (type)
        {
            case PlantType.SunFlower:
                return GameManager.instance.GameConf.SunFlower;
            case PlantType.Peashooter:
                return GameManager.instance.GameConf.PeaShooter;
            case PlantType.WallNut:
                return GameManager.instance.GameConf.WallNut;
            case PlantType.Spike:
                return GameManager.instance.GameConf.Spike;
            case PlantType.Cherry:
                return GameManager.instance.GameConf.Cherry;
        }
        return null;
    }
}
