using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public enum LVState
{
    // 开始游戏
    Start,
    // 战斗中
    Fight,
    // 结束
    Over
}

public class LVManager : MonoBehaviour
{
    public static LVManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        CurrLV = 1;
    }

    private bool isOver;
    // 在刷新僵尸中
    private bool isUpdateZombie;
    // 当前第几天 关卡数
    private int currLV;
    public int CurrLV
    {
        get => currLV;
        set
        {
            currLV = value;
            StartLV(currLV);
        }
    }
    
    // 关卡中的阶段 波数
    private int stageInLV;
    public int StageInLV
    {
        get => stageInLV;
        set
        {
            stageInLV = value;
            UIManager.instance.UpdateStageNum(stageInLV - 1);
            // 一共有2波
            if (stageInLV > 2)
            {
                // 杀掉当前关卡的全部僵尸，就进入下一关
                ZombieManager.Instance.AddAllZombieDeadActionLinstener(OnAllZombieDeadAction);
                // 把状态切换到关卡结束，这样就不会生成僵尸了
                CurrLvState = LVState.Over;
            }
            
        }
    }

    private UnityAction LVStartAction;
    
    private LVState currLvState;
    public LVState CurrLvState
    {
        get => currLvState;
        set
        {
            currLvState = value;
            switch (currLvState)
            {
                case LVState.Start:
                    // 隐藏UI面板
                    UIManager.instance.SetMainPanelActive(false);
                    // 刷新僵尸秀的僵尸
                    ZombieManager.Instance.UpdateZombie(5,ZombieType.Zombie);
                    // 摄像机移动到右侧观察关卡僵尸
                    CameraController.instance.StartMove(LVStartCameraBackAction);
                    break;
                case LVState.Fight:
                    // 显示主面板
                    UIManager.instance.SetMainPanelActive(true);
                    // 每20s后刷新一只僵尸
                    UpdateZombie(3, 1);
                    break;
                case LVState.Over:
                    break;
            }
        }
    }
    
    // 开始关卡
    public void StartLV(int lv)
    {
        if (isOver) return;
        currLV = lv;
        UIManager.instance.UpdateDayNum(currLV);
        StageInLV = 1;
        CurrLvState = LVState.Start;
    }

    private void Update()
    {
        if (isOver) return;
        FSM();
    }

    public void FSM()
    {
        switch (CurrLvState)
        {
            case LVState.Start:
                break;
            case LVState.Fight:
                // 刷僵尸，如果没有刷新僵尸则刷新僵尸
                if (isUpdateZombie == false)
                {
                    // 意味着是最后一波
                    if (stageInLV == 2)
                    {
                        ZombieManager.Instance.UpdateZombie(1,ZombieType.FlagZombie);
                    }
                    // 僵尸刷新的时间
                    float updateTime = Random.Range(15 - stageInLV/2, 20 - stageInLV/2);
                    // 僵尸刷新的数量
                    int updateNum = Random.Range(1, 3+currLV);
                    UpdateZombie(updateTime, updateNum);
                }
                break;
            case LVState.Over:
                break;
        }
    }

    /// <summary>
    /// 关卡开始时，摄像机回归后要执行的方法
    /// </summary>
    private void LVStartCameraBackAction()
    {
        // 让阳光开始创建
        SkySunManager.instance.StartCreateSun(6);
        // 开始显示UI特效
        UIManager.instance.ShowLVStartEF();
        // 清除僵尸秀的僵尸
        ZombieManager.Instance.ClearZombie();
        // 切换战斗状态
        CurrLvState = LVState.Fight;
        // 关卡开始时需要做的事情
        if (LVStartAction != null) LVStartAction();
    }

    private void UpdateZombie(float delay, int zombieNum)
    {
        // 每delay刷新zombieNum个僵尸
        StartCoroutine(DoUpdateZombie(delay, zombieNum));
    }

    IEnumerator DoUpdateZombie(float delay, int zombieNum)
    {
        isUpdateZombie = true;
        yield return new WaitForSeconds(delay);
        // 测试生成僵尸
        ZombieManager.Instance.UpdateZombie(zombieNum,ZombieType.BucketheadZombie);
        // 开始移动
        ZombieManager.Instance.ZombieStartMove();
        isUpdateZombie = false;
        StageInLV += 1;
    }
    
    /// <summary>
    /// 添加关卡开始事件的监听者
    /// </summary>
    public void AddLVStartActionLinstener(UnityAction action)
    {
        LVStartAction += action;
    }
    /// <summary>
    /// 当全部僵尸死亡触发的事件
    /// </summary>
    private void OnAllZombieDeadAction()
    {
        // 更新天数
        CurrLV += 1;
        // 执行一次之后，自己移除委托
        ZombieManager.Instance.RemoveAllZombieDeadActionLinstener(OnAllZombieDeadAction);
    }

    /// <summary>
    /// 游戏结束的第一入口
    /// </summary>
    public void GameOver()
    {
        StopAllCoroutines();
        isOver = true;
        // 效果
        AudioManager.instance.PlayEFAudio(GameManager.instance.GameConf.ZombieEat); // 脑子被吃的音效
        AudioManager.instance.PlayEFAudio(GameManager.instance.GameConf.GameOver);  // 游戏GG的音效
        // 逻辑
        SkySunManager.instance.StopCreateSun();
        ZombieManager.Instance.ClearZombie();
        // UI
        UIManager.instance.GameOver();
    }
}
