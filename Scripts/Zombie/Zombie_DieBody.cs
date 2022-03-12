using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie_DieBody : BaseEFObj
{
    protected override string animationName { get; } = "Zombie_DieBody";
    protected override GameObject PrefabForObjPool
    {
        get
        {
            return GameManager.instance.GameConf.Zombie_DieBody;
        }
    }
    
    /// <summary>
    /// 用于炸死时的初始化
    /// </summary>
    public void InitForBoomDie(Vector2 pos)
    {
        Init(pos, "Zombie_BoomDie");
    }
}
