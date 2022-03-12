using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVStartEF : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        // 获得动画组件
        animator = GetComponent<Animator>();
        // 开始未激活状态
        gameObject.SetActive(false);
    }

    private void Update()
    {
        // 如果播放完毕，则取消激活
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 显示自身
    /// </summary>
    public void Show()
    {
        // 外部调用，激活，并且从开头播放
        gameObject.SetActive(true);
        animator.Play("LVStartEF",0,0);
    }
}
