using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    private void Awake()
    {
        instance = this;
        transform.position = new Vector3(-3.02f, 0.2f, -10);
    }

    public void StartMove(UnityAction action)
    {
        // 一开始往右，然后回归，回归到终点时调用传进来的委托方法
        MoveForLvStart(() => MoveForLvStartBack(action));
    }
    
    /// <summary>
    /// 关卡开始时的移动
    /// </summary>
    private void MoveForLvStart(UnityAction action)
    {
        StartCoroutine(DoMove(2.83f,action));
    }

    /// <summary>
    /// 关卡开始时的摄像机回归
    /// </summary>
    private void MoveForLvStartBack(UnityAction action)
    {
        StartCoroutine(DoMove(-3.02f,action));
    }

    IEnumerator DoMove(float targetPosX, UnityAction action)
    {
        // 获取摄像机移动到的一个目标地点
        Vector3 target = new Vector3(targetPosX, transform.position.y, -10);
        // 获取一个标准的方向向量
        Vector2 dir = (target - transform.position).normalized;
        // 如果距离目标点大于0.1则一直移动
        while (Vector2.Distance(target, transform.position) > 0.1)
        {
            yield return new WaitForSeconds(0.035f);
            transform.Translate(dir * 0.1f);
        }
        // 此时摄像机到达最右边，停顿1.5s
        yield return new WaitForSeconds(0.8f);
        // 执行事件
        if (action != null) action(); 
    }
    
}
