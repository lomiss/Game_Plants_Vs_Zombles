using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public enum ZombieType
{
    Zombie,
    FlagZombie,
    ConeheadZombie,
    BucketheadZombie
}

public class ZombieManager : MonoBehaviour
{
    public static ZombieManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Groan();
    }

    private int currOrderNum = 0;
    // 创建僵尸最大和最小的X坐标
    private float createMaxX = 8.5f;
    private float createMinX = 7.4f;
    // 所有僵尸都死亡的事件
    private UnityAction AllZombieDeadAction;
    public int CurrOrderNum
    {
        get => currOrderNum;
        set
        {
            currOrderNum = value;
            if (value > 50)
            {
                currOrderNum = 0;
            }
        }
    }
    
    /// <summary>
    /// 更新僵尸
    /// </summary>
    /// <param name="zombieNum"></param>
    public void UpdateZombie(int zombieNum, ZombieType zombieType)
    {
        for (int i = 0; i < zombieNum; ++i)
        {
            CreateZombie(Random.Range(0,5), zombieType);
        }
    }

    /// <summary>
    /// 强制请理所有的僵尸，不需要播放死亡倒地动画
    /// </summary>
    public void ClearZombie()
    {
        // 僵尸集合中存在僵尸
        while (zombies.Count > 0) zombies[0].Dead(false);
    }
    
    /// <summary>
    /// 获取一个X随机坐标为了创建僵尸
    /// </summary>
    private float GetPosXRangeForCreateZombie()
    {
        return Random.Range(createMinX, createMaxX);
    }

    public Zombie CreateStandardZombie(int lineNum, Vector2 pos)
    {
        GameObject prefab = GameManager.instance.GameConf.Zombie;
        Zombie zombie = PoolManager.Instance.GetObj(prefab).GetComponent<Zombie>();
        AddZombie(zombie);
        // 修改父物体
        zombie.transform.SetParent(transform);
        zombie.Init(lineNum, CurrOrderNum, pos);
        CurrOrderNum++;
        return zombie;
    } 
    
    /// <summary>
    /// 创建僵尸
    /// </summary>
    /// <param name="lineNum"></param>
    private void CreateZombie(int lineNum, ZombieType zombieType)
    {
        GameObject prefab = null;
        switch (zombieType)
        {
            case ZombieType.Zombie:
                prefab = GameManager.instance.GameConf.Zombie;
                break;
            case ZombieType.FlagZombie:
                prefab = GameManager.instance.GameConf.FlagZombie;
                break;
            case ZombieType.ConeheadZombie:
                prefab = GameManager.instance.GameConf.ConeheadZombie;
                break;
            case ZombieType.BucketheadZombie:
                prefab = GameManager.instance.GameConf.BucketheadZombie;
                break;
        }
        // 从缓存池中取出
        ZombieBase zombie = PoolManager.Instance.GetObj(prefab).GetComponent<ZombieBase>();
        AddZombie(zombie);
        // 修改父物体
        zombie.transform.SetParent(transform);
        //Zombie zombie = GameObject.Instantiate(GameManager.instance.GameConf.Zombie, new Vector2(posX,0), Quaternion.identity, transform).GetComponent<Zombie>();
        // 将随机到的X坐标传进去，Y坐标会根据lineNum对应的网格y坐标自动修正
        zombie.Init(lineNum, CurrOrderNum, new Vector3(GetPosXRangeForCreateZombie(),0,0));
        CurrOrderNum++;
    }
    
    private List<ZombieBase> zombies = new List<ZombieBase>();

    public void AddZombie(ZombieBase zombie)
    {
        zombies.Add(zombie);
    }
    
    public void RemoveZombie(ZombieBase zombie)
    {
        zombies.Remove(zombie);
        CheckAllZombieDeadForLV();
    }
    
    /// <summary>
    /// 获取一个距离最近的僵尸
    /// </summary>
    /// <param name="lineNum">植物当前的行</param>
    /// <param name="pos">植物当前的坐标</param>
    /// <returns></returns>
    public ZombieBase GetZombieBylineMinDistance(int lineNum, Vector3 pos)
    {
        ZombieBase zombie = null;
        float dis = 10000;
        for (int i = 0; i < zombies.Count; ++i)
        {
            // 遍历所有僵尸中等于传进来的lineNum（植物当前的行），并且返回最小距离的僵尸
            if ((int)zombies[i].CurrGrid.Point.y == lineNum && Vector2.Distance(pos, zombies[i].transform.position) < dis)
            {
                dis = Vector2.Distance(pos, zombies[i].transform.position);
                zombie = zombies[i];
            }
        }
        return zombie;
    }

    /// <summary>
    /// 获取指定Y轴、X轴的距离指定目标小于指定距离的僵尸们
    /// </summary>
    /// <returns></returns>
    public List<ZombieBase> GetZombies(int lineNum, Vector2 targetPos, float dis)
    {
        List<ZombieBase> tmp = new List<ZombieBase>();
        for (int i = 0; i < zombies.Count; ++i)
        {
            if ((int)zombies[i].CurrGrid.Point.y == lineNum
                && Vector2.Distance(new Vector2(targetPos.x, 0), new Vector2(zombies[i].transform.position.x+0.52f, 0)) < dis)
            {
                tmp.Add(zombies[i]);
            }
        }
        return tmp;
    }

    /// <summary>
    /// 获得指定距离范围内的全部僵尸
    /// </summary>
    /// <returns></returns>
    public List<ZombieBase> GetZombies(Vector2 targetPos, float dis)
    {
        List<ZombieBase> tmp = new List<ZombieBase>();
        for (int i = 0; i < zombies.Count; ++i)
        {
            if (Vector2.Distance(targetPos, zombies[i].transform.position)<dis)
            {
                tmp.Add(zombies[i]);
            }
        }
        return tmp;
    }
    
    public void ZombieStartMove()
    {
        for (int i = 0; i < zombies.Count; ++i)
        {
            zombies[i].StartMove();
        }
    }
    
    /// <summary>
    /// 为关卡管理器检查所有僵尸死亡时的事件
    /// </summary>
    private void CheckAllZombieDeadForLV()
    {
        if (zombies.Count == 0)
        {
            if (AllZombieDeadAction != null) AllZombieDeadAction();
        }
    }
    
    public void AddAllZombieDeadActionLinstener(UnityAction action)
    {
        AllZombieDeadAction += action;
    }
    
    public void RemoveAllZombieDeadActionLinstener(UnityAction action)
    {
        AllZombieDeadAction -= action;
    }

    /// <summary>
    /// 5秒进行一次，有一定概率播放僵尸呻吟音效
    /// </summary>
    private void Groan()
    {
        StartCoroutine(DoGroan());
    }
    
    IEnumerator DoGroan()
    {
        // 死循环
        while (true)
        {
            // 有僵尸驻场时
            if (zombies.Count > 0)
            {   
                // 1/3的概率可以播放
                if (Random.Range(0, 10) > 6)
                {
                    AudioManager.instance.PlayEFAudio(GameManager.instance.GameConf.ZombieGroan);
                }
            }
            // 每5s执行一次
            yield return new WaitForSeconds(5);
        }
    }
}
