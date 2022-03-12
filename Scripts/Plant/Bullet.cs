using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer spriteRenderer;
    // 攻击力
    private int attackValue;
    // 是否击中
    private bool isHit;
    public void Init(int attackValue, Vector2 pos)
    {
        transform.position = pos;
        _rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //  施加一个向右的力，匀速运动
        _rigidbody2D.AddForce(Vector2.right*300);
        this.attackValue = attackValue;

        // 由于放进去的子弹变成击落状态重力被设置成1了，所以这里要恢复
        _rigidbody2D.gravityScale = 0;
        isHit = false;
        // 修改成正常状态的图片
        spriteRenderer.sprite = GameManager.instance.GameConf.Bullet1Nor;
    }
    
    void Update()
    {
        if (isHit) return;
        // 朝Z轴旋转
        transform.Rotate(new Vector3(0,0,-15f));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Zombie")
        {
            isHit = true;
            // 播放僵尸被击中的音效
            AudioManager.instance.PlayEFAudio(GameManager.instance.GameConf.ZombieHurtByPea);
            // 让僵尸受伤
            other.GetComponentInParent<ZombieBase>().Hurt(attackValue);
            // 击中后修改成击中图片
            spriteRenderer.sprite = GameManager.instance.GameConf.BulletHit;
            // 暂停自己的运动
            _rigidbody2D.velocity = Vector2.zero;
            // 恢复重力下落
            _rigidbody2D.gravityScale = 1;
            // 最后销毁(延迟调用)
            Invoke("Destroy",0.5f);
        }
    }

    private void Destroy()
    {
        // 取消延迟调用
        CancelInvoke();
        // 把自己放进缓存池
        PoolManager.Instance.PushObj(GameManager.instance.GameConf.Bullet1,gameObject);
    }
}
