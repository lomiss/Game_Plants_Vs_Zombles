using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;


public enum ZombieState
{
    Idel,
    Walk,
    Attack,
    Dead,
    NoPlayDead
}

public abstract class ZombieBase : MonoBehaviour
{
    // 僵尸的状态
    protected ZombieState state;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected Grid currGrid;
    
    // 生命值
    protected int hp;
    // 最大生命值
    protected abstract int MaxHp { get; }
    // 僵尸速度表示1格speed秒，所以1/(speed)是每秒1/(speed)格 单位：格/秒
    protected abstract float walkOneTime { get; }
    // 攻击力
    protected abstract float attackValue { get; }
    protected abstract GameObject Prefab { get; }
    
    
    public ZombieState State
    {
        get => state;
        // 如果状态被写入，则播放相关动画
        set
        {
            state = value;
            CheckState();
        }
    }
    protected ZombieHpState zombieHpState;

    public void Init(int lineNum, int orderNum, Vector3 pos)
    {
        hp = MaxHp;
        InitZombieHpState();
        transform.position = pos;
        Find();
        GetGridByVerticalNum(lineNum);
        CheckOrder(orderNum);
        isLostHead = false;
        State = ZombieState.Idel;
    }

    /// <summary>
    /// 初始化ZombieHpState
    /// </summary>
    public abstract void InitZombieHpState();
    /// <summary>
    /// 死亡瞬间要做的事情
    /// </summary>
    public abstract void OnDead();

    public int Hp
    {
        get => hp;
        set
        {
            hp = value;
            // 更新生命值的状态
            zombieHpState.UpdateZombieHpState(hp);
            if (hp <= 0)
            {
                State = ZombieState.Dead;
            }
        }
    } 
    
    // 是否已经失去头
    protected bool isLostHead;
    // 是否在攻击中
    private bool isAttackState;

    // 只有get访问器，只能读不能修改
    public Grid CurrGrid
    {
        get => currGrid;
    }
    
    /// <summary>
    /// 状态检测
    /// </summary>
    protected void CheckState()
    {
        switch (State)
        {
            case ZombieState.Idel:
                // 播放行走动画，但是要卡在第一帧
                animator.Play(zombieHpState.GetCurrWalkAnimationStr(),0,0);
                animator.speed = 0;
                break;
            case ZombieState.Walk:
                // 从上一动画播放的normalizedTime继续播放，而不是从0帧开始
                animator.Play(zombieHpState.GetCurrWalkAnimationStr(),0,animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                animator.speed = 1;
                break;
            case ZombieState.Attack:
                // 从上一动画播放的normalizedTime继续播放，而不是从0帧开始
                animator.Play(zombieHpState.GetCurrAttackAnimationStr(),0,animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                animator.speed = 1;
                break;
            case ZombieState.Dead:
                Dead();
                break;
            case ZombieState.NoPlayDead:
                Dead(false);
                break;
        }
    }

    void Find()
    {
        // 获得子物体的动画组件
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    
    /// <summary>
    /// 检查排序
    /// </summary>
    private void CheckOrder(int orderNum)
    {
        // 我在哪条草坪
        // 越靠0越大，反之越小
        // 0层越大(400-499)，4层越小(0-99)
        int startNum = 0;
        switch ((int) CurrGrid.Point.y)
        {
            case 0:
                startNum = 400;
                break;
            case 1:
                startNum = 300;
                break;
            case 2:
                startNum = 200;
                break;
            case 3:
                startNum = 100;
                break;
            case 4:
                startNum = 0;
                break;
        }
        // 改变精灵的排序顺序
        spriteRenderer.sortingOrder = startNum + orderNum;
    }
    
    void Update()
    {
        // 每帧中判断状态
        FSM();
    }

    /// <summary>
    /// 状态检测
    /// </summary>
    private void FSM()
    {
        switch (State)
        {
            case ZombieState.Idel:
                State = ZombieState.Walk;
                break;
            case ZombieState.Walk:
                // 一直向左走并且遇到植物会攻击，攻击结束继续走
                Move();
                break;
            case ZombieState.Attack:
                if (isAttackState) break;
                Attack(currGrid.CurrPlantBase);
                break;
        }
    }
    
    /// <summary>
    /// 获取一个网格，并决定在第几排出现
    /// </summary>
    /// <param name="verticalNum"></param>
    private void GetGridByVerticalNum(int verticalNum)
    {
        // 得到网格
        currGrid = GridManager.instance.GetGridByVerticalNum(verticalNum);
        // x坐标不变，修正它的y坐标为网格的世界坐标
        transform.position = new Vector3(transform.position.x, currGrid.Position.y, 0);
    }
    
    /// <summary>
    /// 僵尸移动
    /// </summary>
    private void Move()
    {
        // 没有得到网格，没有僵尸
        if (currGrid == null) return;
        currGrid = GridManager.instance.GetGridByWorldPos(transform.position);
        // 当前网格中有植物 并且 它在僵尸的左边 且 距离很近 而且 可以被僵尸吃
        if (currGrid.HavePlant && currGrid.CurrPlantBase.ZombieCanEat && currGrid.CurrPlantBase.transform.position.x < transform.position.x
                               && transform.position.x - currGrid.CurrPlantBase.transform.position.x < 0.3f)
        {
            // 开始攻击植物,切换状态
            State = ZombieState.Attack;
            return;
        // 如果我在最左边的网格并且我已经越过了它    
        }
        if (currGrid.Point.x == 0&&currGrid.Position.x - transform.position.x > 1f)
        {
            // 我们要走到终点（房子）
            Vector2 pos = transform.position;
            Vector2 target = new Vector2(-9.17f, -1.37f);
            // 标准化并且乘上3f，目的是为了加速，当然写在translate里面也是可以的
            Vector2 dir = (target - pos).normalized * 3f;
            transform.Translate(dir*(1/walkOneTime)*Time.deltaTime);
            // 如果我距离终点门很近，意味着游戏结束
            if (Vector2.Distance(target, pos) < 0.05f)
            {
                // 触发游戏结束逻辑
                LVManager.instance.GameOver();
            }
            return;
        }
        // -1.33f是一个网格的距离，所以-1.33f*(1/speed)等于每秒走了1.33f*(1/speed)的距离【速度】,然后乘上时间Time.deltaTime，就是每帧走的距离
        transform.Translate(new Vector2(-1.33f,0)*(1/walkOneTime)*Time.deltaTime);
    }

    private void Attack(PlantBase plant)
    {
        isAttackState = true;
        // 植物相关逻辑
        StartCoroutine(DoHurtPlant(plant));
    }

    /// <summary>
    /// 附加伤害给植物
    /// </summary>
    /// <returns></returns>
    IEnumerator DoHurtPlant(PlantBase plant)
    {
        int num = 0;
        // 植物生命值大于0才扣血(plant是形参，所以currGrid.CurrPlantBase=null，是不会出现空引用的)
        // 防止僵尸持有这个植物后，这个植物又被其余僵尸给吃了而出错，所以plant要判空
        while (plant != null && plant.Hp > 0)
        {
            // 每5次伤害播放一次
            if (num % 5 == 0)
            {
                // 播放尸吃植物的音效
                AudioManager.instance.PlayEFAudio(GameManager.instance.GameConf.ZombieEat);
            }
            num += 1;
            plant.Hurt(attackValue/5);
            // 每0.2s扣一次血
            yield return new WaitForSeconds(0.2f);
        }
        isAttackState = false;
        // 植物死亡后继续走
        State = ZombieState.Walk;
    }

    public void Hurt(int plantAttackValue)
    {
        Hp -= plantAttackValue;
        if(State!=ZombieState.Dead && State != ZombieState.NoPlayDead) 
            StartCoroutine(ColorEF(0.2f, new Color(0.4f, 0.4f, 0.4f), 0.05f, null));
    }

    public void BoomHurt(int attackValue)
    {
        if (attackValue >= Hp)
        {
            // 炸死逻辑
            State = ZombieState.NoPlayDead;
            Zombie_DieBody body = PoolManager.Instance.GetObj(GameManager.instance.GameConf.Zombie_DieBody)
                .GetComponent<Zombie_DieBody>();
            // 不需要设置父物体，直接初始化以及和它的位置
            body.InitForBoomDie(animator.transform.position);
        }
        else
        {
            // 普通受伤逻辑
            Hurt(attackValue);
        }
    }

    public void Dead(bool playOndead=true)
    {   
        // 是否播放僵尸死亡动画
        if(playOndead) OnDead();
        // 告诉僵尸管理器僵尸死了
        ZombieManager.Instance.RemoveZombie(this);
        StopAllCoroutines();
        currGrid = null;
        // 放进缓存池
        PoolManager.Instance.PushObj(Prefab, gameObject);
    }
    
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

    public void StartMove()
    {
        State = ZombieState.Walk;
    }
}
