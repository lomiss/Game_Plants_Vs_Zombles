using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    // 创建单例
    private static PoolManager instance;
    public static PoolManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PoolManager();
            }
            return instance;
        }
    }

    private GameObject poolObj;
    
    // 预制体=>实例化后的具体游戏物体
    private Dictionary<GameObject, List<GameObject>> poolDataDic = new Dictionary<GameObject, List<GameObject>>();

    /// <summary>
    /// 从缓存池中获取第一个obj
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    public GameObject GetObj(GameObject prefab)
    {
        GameObject obj = null;
        // 如果字典中存在预制体这个key并且数量大于0
        if (poolDataDic.ContainsKey(prefab) && poolDataDic[prefab].Count > 0)
        {
            // 返回list的第一个
            obj = poolDataDic[prefab][0];
            // 同时移除第一个
            poolDataDic[prefab].RemoveAt(0);
        }
        else
        {
            obj = GameObject.Instantiate(prefab);
        }
        obj.SetActive(true);
        // 让其没有父物体
        obj.transform.SetParent(null);
        return obj;
    }

    public void PushObj(GameObject prefab, GameObject obj)
    {
        // 判断是否有根目录，没有则创建一个空游戏物体作为根
        if (poolObj == null) poolObj = new GameObject("poolObj");
        // 存在这个key
        if (poolDataDic.ContainsKey(prefab))
        {
            // 把物体放进去
            poolDataDic[prefab].Add(obj);
        }
        else
        {
            // 创建这个预制体的缓存池数据
            poolDataDic.Add(prefab,new List<GameObject>(){obj});
        }
        // 如果根目录下没有预制体命名的子物体
        if (poolObj.transform.Find(prefab.name) == false)
        {
            // 则创建，并且父节点是poolObj
            new GameObject(prefab.name).transform.SetParent(poolObj.transform);
        }
        // 隐藏
        obj.SetActive(false);
        // 设置父物体为根目录下的该预制体
        obj.transform.SetParent(poolObj.transform.Find(prefab.name));
    }

    // 清除所有的数据
    public void Clear()
    {
        poolDataDic.Clear();
    }
}
