using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConeheadZombie : ZombieBase
{
    protected override int MaxHp { get; } = 640;
    protected override float walkOneTime { get; } = 6;
    protected override float attackValue { get; } = 100;
    protected override GameObject Prefab { get { return GameManager.instance.GameConf.ConeheadZombie; } }
    public override void InitZombieHpState()
    {
        zombieHpState = new ZombieHpState(
            0,
            new List<int>() {MaxHp, 270},
            new List<string>(){"ConeheadZombie_Walk"},
            new List<string>(){"ConeheadZombie_Attack"},
            new List<UnityAction>() {null, HpStateEvent}
        );
    }

    public override void OnDead()
    {
        
    }

    /// <summary>
    /// 生命值达到一个值就变成普通僵尸
    /// </summary>
    public void HpStateEvent()
    {
        // 先召唤一个普通僵尸
        Zombie zombie =  ZombieManager.Instance.CreateStandardZombie((int)currGrid.Point.y, transform.position);
        // 同步动画
        zombie.InitFromOtherZombieCreate(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        // 自身死亡但是不播放动画
        State = ZombieState.NoPlayDead;
    }
}
