using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameConf GameConf
    {
        get; private set;
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Application.targetFrameRate = 60;
            GameConf = Resources.Load<GameConf>("GameConf");
            // 保证一个唯一性
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 存在多个就删除
            Destroy(gameObject);
        }
    }
}
