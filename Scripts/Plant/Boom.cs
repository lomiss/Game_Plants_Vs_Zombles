using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boom : BaseEFObj
{
    protected override GameObject PrefabForObjPool
    {
        get
        {
            return GameManager.instance.GameConf.BoomObj;
        }
    }

    protected override string animationName { get; } = "Boom";
}
