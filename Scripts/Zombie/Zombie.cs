using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Zombie : ZombieBase
{
    protected override int MaxHp { get; } = 270;
    protected override float walkOneTime { get; } = 6;
    protected override float attackValue { get; } = 100;
    protected override GameObject Prefab { get { return GameManager.instance.GameConf.Zombie; } }
    
    public override void InitZombieHpState()
    {
        // 从[1,4)中随机一个整数
        int rangeWalk = Random.Range(1, 4);
        string walkAnimationStr = "";
        // 在初始化时随机播放一个动画
        switch (rangeWalk)
        {
            case 1:
                walkAnimationStr = "Zombie_Walk1";
                break;
            case 2:
                walkAnimationStr = "Zombie_Walk2";
                break;
            case 3:
                walkAnimationStr = "Zombie_Walk3";
                break;
        }
        zombieHpState = new ZombieHpState(
            0,
            new List<int>() {MaxHp, 90},
            new List<string>(){walkAnimationStr, "Zombie_LostHead"},
            new List<string>(){"Zombie_Attack", "Zombie_LostHeadAttack"},
            new List<UnityAction>() {null,CheckLostHead}
        );
    }
    
    public override void OnDead()
    {
        // 创建一个死亡身体，用于体现效果（为了简便，实现方式和头一样）
        Zombie_DieBody body = PoolManager.Instance.GetObj(GameManager.instance.GameConf.Zombie_DieBody)
            .GetComponent<Zombie_DieBody>();
        // 不需要设置父物体，直接初始化以及和它的位置
        body.Init(animator.transform.position);
    }

    private void CheckLostHead()
    {
        if (!isLostHead)
        {
            // 头已经掉了
            isLostHead = true;
            // 创建一个头
            Zombie_Head head = PoolManager.Instance.GetObj(GameManager.instance.GameConf.Zombie_Head)
                .GetComponent<Zombie_Head>();
            // 不需要设置父物体，直接初始化以及和它的位置
            head.Init(animator.transform.position);
            //GameObject.Instantiate(GameManager.instance.GameConf.Zombie_Head, animator.transform.position, Quaternion.identity, null);
            // 动画文本被改变，此使需要检测状态，才能做到动画马上更新
            CheckState();
        }
    }

    /// <summary>
    /// 从其余僵尸那里初始化
    /// </summary>
    public void InitFromOtherZombieCreate(float time)
    {
        // 因为路障僵尸走路和普通僵尸走路一样，所以把行走动画改成walk3，如果僵尸是在攻击被打掉路障的，那么它会根据条件把动画马上切换成攻击
        zombieHpState.hpLimitWalkAnimationStr[0] = "Zombie_Walk3";
        animator.Play("Zombie_Walk3", 0, time);
    }
}
