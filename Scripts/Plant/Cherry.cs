using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cherry : PlantBase
{
    protected override int attackValue { get; } = 1800;
    public override float MaxHp { get; } = 300;
    protected override void OnInitForPlace()
    {
        StartCoroutine(CheckBoom());
    }
    
    /// <summary>
    /// 检测爆炸
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckBoom()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.05f);
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                Boom();
            }
        }
    }

    private void Boom()
    {
        // 播放爆炸音效
        AudioManager.instance.PlayEFAudio(GameManager.instance.GameConf.Boom);
        // 找到可以被我攻击的敌人并且附加伤害（自己为圆心，2.25为半径）
        List<ZombieBase> zombies = ZombieManager.Instance.GetZombies(transform.position,2.25f);
        if (zombies == null) return;
        for (int i = 0; i < zombies.Count; i++)
        {
            zombies[i].BoomHurt(attackValue);
        }
        // 生成攻击特效
        Boom boom = PoolManager.Instance.GetObj(GameManager.instance.GameConf.BoomObj).GetComponent<Boom>();
        boom.Init(transform.position);
        // 自身死亡
        Dead();
    }
}
