using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class PlantBase : MonoBehaviour
{
    // 只能让子类调用
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    // 当前植物所在的网格
    protected Grid currGrid;
    // 植物的生命值
    protected float hp;
    protected PlantType plantType;
    protected virtual Vector2 offset { get; } = Vector2.zero;

    public virtual bool ZombieCanEat { get; } = true;

    // 攻击间隔
    protected virtual float attackCD { get; }
    // 攻击力
    protected virtual int attackValue { get; }   
    
    public float Hp
    {
        get => hp;
        protected set
        {
            hp = value;
            // 做生命值发生变化瞬间要做的事情
            HpUpdateEvent();
        }
    }

    // 最大生命值，每个植物的最大生命值不一样，所以使用抽象方法【抽象方法只能在抽象类中进行声明】
    public abstract float MaxHp { get; }

    /// <summary>
    /// 用于创建时 网格变化时的更新
    /// </summary>
    /// <param name="gridPos"></param>
    public void UpdateForCreate(Vector2 gridPos)
    {
        transform.position = gridPos + offset;
    }
    
    /// <summary>
    /// 通用情况下的初始化
    /// </summary>
    protected void InitForAll(PlantType type)
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        plantType = type;
    }
    
    // 创建时的初始化
    public void InitForCreate(bool inGrid, PlantType type, Vector2 pos)
    {
        // 获取组件
        InitForAll(type);
        transform.position = pos + offset;
        // 拖拽时不播放动画
        animator.speed = 0;
        // 在网格中
        if (inGrid)
        {
            spriteRenderer.sortingOrder = -1;
            spriteRenderer.color = new Color(1, 1, 1, 0.6f); // 设置透明
        }
        // 不在网格中
        else
        {
            spriteRenderer.sortingOrder = 1;
            spriteRenderer.color = new Color(1, 1, 1, 1);
        }
    }
    
    // 放置植物的初始化
    public void InitForPlace(Grid grid, PlantType type)
    {
        InitForAll(type);
        spriteRenderer.color = new Color(1, 1, 1, 1);
        // 种下后该植物的生命值等于最大生命值
        Hp = MaxHp;
        currGrid = grid;
        // 将当前植物基类脚本赋值给网格
        currGrid.CurrPlantBase = this;
        // 当前植物的位置是网格的位置（中心）
        transform.position = grid.Position+offset;
        // 恢复动画
        animator.speed = 1;
        spriteRenderer.sortingOrder = 0;
        OnInitForPlace();
    }
    /// <summary>
    /// 受伤方法，被僵尸攻击时调用
    /// </summary>
    public void Hurt(float hurtValue)
    {
        Hp -= hurtValue;
        StartCoroutine(ColorEF(0.2f, new Color(0.5f, 0.5f, 0.5f), 0.05f, null));
        if (Hp <= 0)
        {
            // 死亡
            Dead();
        }
    }
    
    /// <summary>
    /// 颜色变化效果
    /// </summary>
    /// <returns></returns>
    protected IEnumerator ColorEF(float wantTime, Color targetColor, float delayTime, UnityAction fun)
    {
        float currTime = 0;
        float lerp;
        while (currTime < wantTime)
        {
            yield return new WaitForSeconds(delayTime);
            lerp = currTime / wantTime;
            currTime += delayTime;
            // 实现一个从白到红的插值计算，lerp为0就是白色(原色)，如果为1就是Color(1,0.6f,0)
            spriteRenderer.color = Color.Lerp(Color.white, targetColor, lerp);
        }
        // 恢复原来的附加色(白色)
        spriteRenderer.color = Color.white;
        if (fun != null) fun();
    }
    
    public void Dead()
    {
        // CurrPlantBase修改为空的同时，也将currGrid.HavePlant改为false了，这样Move函数中就可以防止出错
        if (currGrid != null)
        {
            // 由于植物是放进缓存池的，所以相应的变量也要置空
            currGrid.CurrPlantBase = null;
            currGrid = null;
        }
        // 子类会继承，所以这里要写
        StopAllCoroutines();
        CancelInvoke();
        PoolManager.Instance.PushObj(PlantManager.instance.GetPlantByType(plantType), gameObject);
    }
    
    // 创建虚基类（放置后的操作）
    protected virtual void OnInitForPlace(){ }
    protected virtual void HpUpdateEvent(){ }
    
    protected virtual void OnInitForAll(){ }
    
    protected virtual void OnInitForCreate(){ }
    
}
