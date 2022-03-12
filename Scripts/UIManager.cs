using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private Text sunNumText;
    private GameObject mainPanel;
    private UIPlantGrid currCard;
    public UIPlantGrid CurrCard
    {
        get => currCard;
        set
        {
            // 如果当前和currCard一样就跳过，因为不需要取消currCard所代表的植物
            if (currCard == value) return;
            // 如果已经有植物待种植
            if (currCard != null)
            {
                // 就将该植物取消种植
                currCard.WantPlace = false;
            }
            currCard = value;
        }
    }
    private LVStartEF LVStartEF;
    private LVInfoPanel LVInfoPanel;
    private SetPanel SetPanel;
    private OverPanel OverPanel;
    private void Awake()
    {
        instance = this;
        mainPanel = transform.Find("MainPanel").gameObject;
        // 可能阳光初始值设置后，sunNumText还未获取，所以放在Awake里
        sunNumText = transform.Find("MainPanel/SunNumText").GetComponent<Text>();
        LVStartEF = transform.Find("LVStartEF").GetComponent<LVStartEF>();
        LVInfoPanel = transform.Find("LVInfoPanel").GetComponent<LVInfoPanel>();
        SetPanel = transform.Find("SetPanel").GetComponent<SetPanel>();
        OverPanel = transform.Find("OverPanel").GetComponent<OverPanel>();
        SetPanel.gameObject.SetActive(false);
    }

    // 更新阳光数字
    public void UpdateSunNum(int num)
    {
        sunNumText.text = num.ToString();
    }

    // 获取阳光数量文本的世界坐标
    public Vector3 GetSunNumTextPos()
    {
        // transform.position是个结构体，修改pos.x实际上是对position的复制体修改，所以如果修改最后还要transform.position = pos;
        // 这里得到的是阳光UI文本的屏幕坐标，之所以有Z轴，是用来区分摄像机不同视角下的屏幕坐标
        return sunNumText.transform.position;
    }

    public void SetMainPanelActive(bool isShow)
    {
        mainPanel.SetActive(isShow);
    }
    
    /// <summary>
    /// 显示关卡开始时的动画
    /// </summary>
    public void ShowLVStartEF()
    {
        LVStartEF.Show();
    }
    
    // 在UI管理器中它们
    public void UpdateDayNum(int day)
    {
        LVInfoPanel.UpdateDayNum(day);
    }

    public void UpdateStageNum(int stage)
    {
        LVInfoPanel.UpdateStageNum(stage);
    }

    public void ShowSetPanel()
    {
        AudioManager.instance.PlayEFAudio(GameManager.instance.GameConf.Pause);
        SetPanel.Show(true);
    }

    public void GameOver()
    {
        OverPanel.Over();
    }
    
}
