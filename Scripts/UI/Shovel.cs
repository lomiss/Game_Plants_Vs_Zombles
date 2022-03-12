using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shovel : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    private Transform shovelImg;
    // 是否在使用铲子中
    private bool isShovel;

    public bool IsShovel
    {
        get => isShovel;
        set
        {
            isShovel = value;
            // 需要铲除植物
            if (isShovel)
            {
                AudioManager.instance.PlayEFAudio(GameManager.instance.GameConf.Shovel);
                shovelImg.localRotation = Quaternion.Euler(0,0,45);
            }
            // 把铲子放回去
            else
            {
                AudioManager.instance.PlayEFAudio(GameManager.instance.GameConf.Shovel);
                shovelImg.localRotation = Quaternion.Euler(0,0,0);
                shovelImg.transform.position = transform.position;
            }
        }
    }

    private void Start()
    {
        shovelImg = transform.Find("Image");
        LVManager.instance.AddLVStartActionLinstener(OnLVStartAction);
    }

    private void Update()
    {
        if (IsShovel)
        {
            shovelImg.position = Input.mousePosition;
            // 点击鼠标左键判断是否要铲除植物
            if (Input.GetMouseButtonDown(0))
            {
                Grid grid = GridManager.instance.GetGridByMouse();
                // 如果获取网格上没有植物则return
                if (grid.CurrPlantBase == null) return;
                // 如果鼠标离植物的距离小于1.5f，则铲除这个植物
                if (Vector2.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition),
                    grid.CurrPlantBase.transform.position) < 1.5f)
                {
                    grid.CurrPlantBase.Dead();
                    IsShovel = false;
                }
            }
            // 点击鼠标右键取消铲子选中状态
            if (Input.GetMouseButtonDown(1))
            {
                IsShovel = false;
            }
        }
    }
    // 鼠标进入
    public void OnPointerEnter(PointerEventData eventData)
    {
        shovelImg.transform.localScale = new Vector2(1.4f, 1.4f);
    }
    // 鼠标退出
    public void OnPointerExit(PointerEventData eventData)
    {
        shovelImg.transform.localScale = new Vector2(1f, 1f);
    }
    // 鼠标点击
    public void OnPointerClick(PointerEventData eventData)
    {
        // 如果需要铲植物
        if (!IsShovel)
        {
            IsShovel = true;
        }
    }
    
    private void OnLVStartAction()
    {
        IsShovel = false;
    }
}
