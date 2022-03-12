using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEFObj : MonoBehaviour
{
    private Animator animator;
    private bool isOver;
    protected abstract GameObject PrefabForObjPool { get; }
    protected abstract String animationName { get; }
    public void Init(Vector2 pos, string AnimationName = null)
    {
        animator = GetComponent<Animator>();
        transform.position = pos;
        isOver = true;
        animator.speed = 1;
        // 如果参数不为空，就播放参数这个动画
        if (AnimationName != null)
        {
            // 重新从0开始播放，防止从中间播放
            animator.Play(AnimationName,0,0);
        }
        else
        {
            animator.Play(animationName,0,0);
        }
    }
    
    void Update()
    {
        // 该效果已经触发，并且播放已经结束，normalizedTime在[0,1]之间
        if (isOver&&animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            // 播放完毕
            animator.speed = 0;
            isOver = false;
            // 2s之后销毁自身
            Invoke("Destroy",2);
        }
    }

    private void Destroy()
    {
        CancelInvoke();
        // 把自己放进缓存池
        PoolManager.Instance.PushObj(PrefabForObjPool,gameObject);
    }
}
