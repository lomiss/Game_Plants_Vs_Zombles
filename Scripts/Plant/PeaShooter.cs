using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 植物的基类
/// </summary>
public class PeaShooter : PlantBase
{
    public override float MaxHp
    {
        get { return 300; }
    }
    // 是否可以攻击
    private bool canAttack = true;
    // 攻击力
    protected override int attackValue { get; } = 20;
    // 攻击间隔
    protected override float attackCD { get; } = 1.4f;

    // 实例化子弹相对于豌豆射手的偏移量
    private Vector3 createBulletOffestPos = new Vector2(0.562f, 0.396f);
    
    protected override void OnInitForPlace()
    {
        canAttack = true;
        // 每0.2s攻击
        InvokeRepeating("Attack",0,0.2f);
    }
    
    /// <summary>
    /// 攻击方法-循环检测
    /// </summary>
    private void Attack()
    {
        if (canAttack == false) return;
        // 从僵尸管理器获取一个离我最近的僵尸
        ZombieBase zombie = ZombieManager.Instance.GetZombieBylineMinDistance((int) currGrid.Point.y, transform.position);
        // 前方没有僵尸则跳出
        if (zombie == null) return; 
        // 僵尸必须在草坪上
        if ((int)zombie.CurrGrid.Point.x == 8 &&
            Vector2.Distance(zombie.transform.position, zombie.CurrGrid.Position) > 1.5f) return;
        // 如果僵尸不在我的右边，则跳出
        if (zombie.transform.position.x < transform.position.x) return;
        // 此使开始发射,实例化一个子弹
        Bullet bullet = PoolManager.Instance.GetObj(GameManager.instance.GameConf.Bullet1).GetComponent<Bullet>();
        bullet.transform.SetParent(transform);
        //Bullet bullet = GameObject.Instantiate<GameObject>(GameManager.instance.GameConf.Bullet1, transform.position + createBulletOffestPos, Quaternion.identity, transform).GetComponent<Bullet>();
        bullet.Init(attackValue, transform.position + createBulletOffestPos);
        CDEnter();
        canAttack = false;
    }
    
    private void CDEnter()
    {
        // 开始计算冷却
        StartCoroutine(CalCD());
    }

    // 计算冷却时间
    IEnumerator CalCD()
    {
        yield return new WaitForSeconds(attackCD);
        // 冷却时间结束
        canAttack = true;
    }
    
}
