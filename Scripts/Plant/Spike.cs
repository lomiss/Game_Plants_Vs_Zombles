using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : PlantBase
{
    // 攻击力
    protected override int attackValue { get; } = 20;
    // 攻击间隔
    protected override float attackCD { get; } = 1f;
    protected override Vector2 offset { get; } = new Vector2(0,-0.4f);
    // 僵尸是否可以吃这个植物
    public override bool ZombieCanEat { get; } = false;
    public override float MaxHp { get { return 300; } }
    protected override void OnInitForPlace()
    {
        // 每attackCD秒检测有没有敌人被我攻击
        InvokeRepeating("CheckAttack",0,attackCD);
    }

    private void CheckAttack()
    {
        // 找到可以被我攻击的敌人并且附加伤害
        List<ZombieBase> zombies = ZombieManager.Instance.GetZombies((int)currGrid.Point.y, transform.position, 0.665f);
        if (zombies == null) return;
        for (int i = 0; i < zombies.Count; i++)
        {
            zombies[i].Hurt(attackValue);
        }
    }
}
