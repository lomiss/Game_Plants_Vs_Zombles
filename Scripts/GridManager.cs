using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;
    private void Awake()
    {
        instance = this;
        CreateGridsBaseGrid();
    }
    private List<Vector2> pointList = new List<Vector2>();
    private List<Grid> GridList = new List<Grid>();
    
    private void Start()
    {
        //CreateGridsBasePointList();
        //GreateGridsBaseColl();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //print(GetGridPointByMouse());
        }
    }

    // 基于碰撞的形式创建
    private void GreateGridsBaseColl()
    {
        // 由于这个预制体比较简单，所以直接用代码来定义，创建一个预制体网格
        GameObject prefabGrid = new GameObject();
        // 设置碰撞器大小
        prefabGrid.AddComponent<BoxCollider2D>().size = new Vector2(1, 1.5f);
        // 父物体是网格管理器
        prefabGrid.transform.SetParent(transform);
        // 位置就是网格管理器的位置
        prefabGrid.transform.position = this.transform.position;
        prefabGrid.name = 0 + "-" + 0;

        for (int i = 0; i < 9; ++i)
        {
            for (int j = 0; j < 5; ++j)
            {
                // 每次实例时需要增加偏移量，让每个网格的上下和左右保持一定的距离
                GameObject grid =  GameObject.Instantiate(prefabGrid,transform.position+new Vector3(1.33f*i,1.63f*j,0), 
                    Quaternion.identity, transform);
                grid.name = i + "-" + j;
            }
        }
    }

    // 创建网格基于坐标的形式
    private void CreateGridsBasePointList()
    {
        for (int i = 0; i < 9; ++i)
        {
            for (int j = 0; j < 5; ++j)
            {
                // 将每个点预先保存在二维list中
                pointList.Add(transform.position+new Vector3(1.33f*i,1.63f*j,0));
            }
        }
    }
    
    // 创建脚本的形式形式
    private void CreateGridsBaseGrid()
    {
        for (int i = 0; i < 9; ++i)
        {
            for (int j = 0; j < 5; ++j)
            {
                // 由于该脚本依附的游戏对象是在根目录，所以transform.position是世界坐标
                GridList.Add(new Grid(new Vector2(i,j),
                    transform.position+new Vector3(1.33f*i,1.63f*j,0),false));
            }
        }
    }

    // 通过鼠标获取离鼠标最近的【网格坐标点】
    public Vector2 GetGridPointByMouse(Vector2 worldPos)
    {
        return GetGridByWorldPos(worldPos).Position;
    }
    
    // 通过世界坐标获取离鼠标最近的【网格】
    public Grid GetGridByWorldPos(Vector2 worldPos)
    {
        float dis = 999999;
        Grid grid = null;
        for (int i = 0; i < GridList.Count; ++i)
        {
            // 这里是基于世界坐标来计算距离的
            float mouseToGrid = Vector2.Distance(worldPos, GridList[i].Position);
            if (mouseToGrid < dis)
            {
                dis = mouseToGrid;
                grid = GridList[i];
            }
        }
        return grid;
    }
    
    // 通过鼠标获得网格
    public Grid GetGridByMouse()
    {
        return GetGridByWorldPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
    
    /// <summary>
    /// 通过Y轴来寻找一个网格，从下往上0开始
    /// </summary>
    /// <param name="verticalNum"></param>
    /// <returns></returns>
    public Grid GetGridByVerticalNum(int verticalNum)
    {
        // 遍历所有的网格
        for (int i = 0; i < GridList.Count; ++i)
        {
            // 如果得到了最后一行verticalNum列时返回这个网格
            if (GridList[i].Point == new Vector2(8, verticalNum))
            {
                return GridList[i];
            }
        }
        return null;
    }
}
