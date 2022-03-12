using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FlagZombie : ZombieBase
{
    protected override int MaxHp { get; } = 270;
    protected override float walkOneTime { get; } = 4;
    protected override float attackValue { get; } = 100;
    protected override GameObject Prefab { get { return GameManager.instance.GameConf.FlagZombie;} }
    public override void InitZombieHpState()
    {
        zombieHpState = new ZombieHpState(
            0,
            new List<int>() {MaxHp, 90},
            new List<string>(){"FlagZombie_Walk", "FlagZombie_LostHeadWalk"},
            new List<string>(){"FlagZombie_Attack", "FlagZombie_LostHeadAttack"},
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
}
