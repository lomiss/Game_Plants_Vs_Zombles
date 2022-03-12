using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Zombie_Head : BaseEFObj
{
    protected override string animationName { get; } = "Zombie_Head";
    protected override GameObject PrefabForObjPool
    {
        get
        {
            return GameManager.instance.GameConf.Zombie_Head;
        }
    }
    
}
