using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 卡片的四种状态
/// </summary>
public enum CardState
{
    // 有阳光有CD
    CanPlace,
    // 有阳光没有CD
    NotCD,
    // 没有阳光有CD
    NotSun,
    // 都没有
    NotAll
}

public class UIPlantGrid : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    // 遮罩图片的img组件
    private Image maskImg;
    // 自身的组件
    private Image image;
    // 需要阳关数量的Text
    private Text wantSunText;
    
    // 种植需要多少阳光
    public int wantSunNum;
    // 冷却时间，几秒可以放置一次植物
    public float CDTime;
    // 当前时间：用于冷却时间的计算
    private float currTimeForCd;
    // 所需阳光是否足够
    private bool isSunEnough;
    // 是否可以放置植物（是否有CD）
    private bool canPlace;
    // 植物的预制体
    private GameObject prefab;
    // 用来创建的植物
    private PlantBase plant;
    // 在网格中的植物，是透明的
    private PlantBase plantInGrid;
    // 当前卡片所对应的植物类型
    public PlantType CardPlantType;
    private CardState cardState = CardState.NotAll;
    // 是否需要放置植物
    private bool wantPlace;
    // wantPlace被修改时执行
    public bool WantPlace
    {
        get => wantPlace;
        set
        {
            wantPlace = value;
            if (wantPlace)
            {
                // 获取预制体的资源
                prefab = PlantManager.instance.GetPlantByType(CardPlantType);
                plant = PoolManager.Instance.GetObj(prefab).GetComponent<PlantBase>();
                plant.transform.SetParent(PlantManager.instance.transform);
                // 开始实例化，由于这里用到的是PlantBase脚本，所以顺带获取这个脚本组件
                //plant = Instantiate(prefab, Vector3.zero, Quaternion.identity, PlantManager.instance.transform).GetComponent<PlantBase>();
                // 不在网格中的植物，也就是拖拽的植物
                plant.InitForCreate(false, CardPlantType, Vector2.zero);
                UIManager.instance.CurrCard = this;
            }
            else
            {
                if (plant != null)
                {
                    plant.Dead();
                    plant = null;
                }
            }
        }
    }
    
    // CanPlace被修改时执行
    public bool CanPlace
    {
        get => canPlace;
        set
        {
            canPlace = value;
            // 检测状态
            CheckState();
        }
    }
    
    /// <summary>
    /// 状态检测
    /// </summary>
    private void CheckState()
    {
        // 有阳光 有CD
        if (canPlace && PlayerManager.instance.SunNum >= wantSunNum)
        {
            CardState = CardState.CanPlace;
            isSunEnough = true;
            // 有阳光 没有CD
        }else if (!canPlace && PlayerManager.instance.SunNum >= wantSunNum)
        {
            CardState = CardState.NotCD;
            isSunEnough = true; 
            // 没有阳光 有CD    
        }else if (canPlace && PlayerManager.instance.SunNum < wantSunNum)
        {
            CardState = CardState.NotSun;
            isSunEnough = false;
        }
        // 既没有阳光也没有CD
        else
        {
            CardState = CardState.NotAll;
            isSunEnough = false;
        }
    }
    
    private CardState CardState
    {
        get => cardState;
        set
        {
            // 如果修改的值和当前值一样，那么直接跳出
            if (cardState == value)
            {
                return;
            }
            cardState = value;
            switch (cardState)
            {
                case CardState.CanPlace:
                    // CD没有遮罩 自身是明亮的
                    maskImg.fillAmount = 0;
                    image.color = Color.white;
                    break;
                case CardState.NotCD:
                    // CD有遮罩 自身是明亮的
                    CDEnter();
                    image.color = Color.white;
                    break;
                case CardState.NotSun:
                    // CD没有遮罩，自身是昏暗的
                    maskImg.fillAmount = 0;
                    image.color = new Color(0.75f,0.75f,0.75f);
                    break;
                case CardState.NotAll:
                    // CD有遮罩 自身是昏暗的
                    CDEnter();
                    image.color = new Color(0.75f,0.75f,0.75f);
                    break;
            }
        }
    }
    
    private void Start()
    {
        maskImg = transform.Find("Mask").GetComponent<Image>();
        // 获取文本组件并赋值
        wantSunText = transform.Find("Text").GetComponent<Text>();
        wantSunText.text = wantSunNum.ToString();
        image = GetComponent<Image>();
        // 一开始时所有的植物都是没有CD的
        CanPlace = true;
        // 使用事件委托的方法，当SunNum的值发生变化时执行CheckState函数
        PlayerManager.instance.AddSunNumUpdateActionListener(CheckState);
        // 注册方法
        LVManager.instance.AddLVStartActionLinstener(OnLVStartAction);
    }
    
    private void Update()
    {
        // 如果需要放置植物，并且要放置的植物不能为空
        if (WantPlace && plant != null)
        {
            // 让植物跟随我们的鼠标
            Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // 每帧获得当前鼠标最近的网格
            Grid grid = GridManager.instance.GetGridByWorldPos(mousePoint);
            // 拖拽的植物实时跟着鼠标动
            plant.transform.position = new Vector3(mousePoint.x, mousePoint.y, 0);
            // 如果我距离网格比较近并且这个网格没有植物，需要在网格上出现一个透明的植物，当前鼠标的世界坐标和距离鼠标最近的网格的世界坐标的距离
            if (grid.HavePlant==false && Vector2.Distance(mousePoint, grid.Position) < 1.5)
            {
                if (plantInGrid == null)
                {
                    plantInGrid = PoolManager.Instance.GetObj(prefab).GetComponent<PlantBase>();
                    plantInGrid.transform.SetParent(PlantManager.instance.transform);
                    // 由于plant是个脚本，所以需要获得这个脚本对应的游戏对象，实例化后再获得脚本，以便后续调用处理
                    //plantInGrid = Instantiate(plant.gameObject, grid.Position, Quaternion.identity, PlantManager.instance.transform).GetComponent<PlantBase>();
                    // 在网格中的拟创建的虚拟植物
                    plantInGrid.InitForCreate(true, CardPlantType, grid.Position); 
                }
                else
                {
                    plantInGrid.UpdateForCreate(grid.Position);
                }
                // 如果点击鼠标需要放置植物
                if (Input.GetMouseButtonDown(0))
                {
                    // 既然已经点击，那么把拖拽中的植物，放在网格点上【删除，写在InitForPlace函数中了】
                    // plant.transform.position = grid.Position;
                    // 实现真正的放置，由于plant是个脚本了，所以可以直接调用函数
                    plant.InitForPlace(grid, CardPlantType);
                    // 然后将存储太阳花GameObject的plant清空，和已经放置的植物实际上没有关系了
                    plant = null;
                    // 将网格中的虚拟植物游戏对象销毁
                    plantInGrid.Dead();
                    plantInGrid = null;
                    // 不需要种植了，那么状态改变销毁plant
                    WantPlace = false;
                    // 进入冷却
                    CanPlace = false;
                    AudioManager.instance.PlayEFAudio(GameManager.instance.GameConf.Place);
                    // 种植成功需要减少玩家的阳光
                    PlayerManager.instance.SunNum -= wantSunNum;
                }
            }
            else
            {
                if (plantInGrid != null)
                {
                    plantInGrid.Dead();
                    plantInGrid = null;
                }
            }
        }
        // 监听鼠标右键，实现取消放置状态
        if (Input.GetMouseButtonDown(1))
        {
            CancelPlace();
        }
    }
    
    /// <summary>
    /// 取消放置
    /// </summary>
    private void CancelPlace()
    {
        if(plant != null) plant.Dead();
        if(plantInGrid != null) plantInGrid.Dead();
        plant = null;
        plantInGrid = null;
        WantPlace = false;
    }
    
    /// <summary>
    /// 在关卡开始时需要做的事情
    /// </summary>
    private void OnLVStartAction()
    {
        CancelPlace();
        // 重置CD
        CanPlace = true;
    }
    
    private void CDEnter()
    {
        // 如果已经在减CD状态了则跳过
        if (maskImg.fillAmount != 0) return;
        // 完全遮住，表示不可以控制
        maskImg.fillAmount = 1;
        // 遮住后，开始计算冷却
        StartCoroutine(CalCD());
    }

    // 计算冷却时间
    IEnumerator CalCD()
    {
        float calCD = (1 / CDTime) * 0.1f; // 1s减少阴影的值 * 0.1s = 0.1s减少的阴影的值
        currTimeForCd = CDTime;
        while (currTimeForCd >= 0)
        {
            yield return new WaitForSeconds(0.1f);
            maskImg.fillAmount -= calCD;
            currTimeForCd -= 0.1f;
        }
        // 冷却时间结束，可以放置了
        CanPlace = true;
    }

    // 鼠标移入执行
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!CanPlace || !isSunEnough) return;
        transform.localScale = new Vector2(1.05f, 1.05f);
    }

    // 鼠标移出执行
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!CanPlace || !isSunEnough) return;
        transform.localScale = new Vector2(1f, 1f);
    }

    // 鼠标点击操作
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!CanPlace || !isSunEnough) return;
        AudioManager.instance.PlayEFAudio(GameManager.instance.GameConf.ButtonClick);
        if (!WantPlace)
        {
            WantPlace = true;
        }
    }
}
