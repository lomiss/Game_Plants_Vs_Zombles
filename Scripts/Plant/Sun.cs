using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Sun : MonoBehaviour
{
    // 下落的目标点Y
    private float downTargetPosY;
    // 是否来自天空
    private bool isFromSky;
    private void Update()
    {
        // 区别对待防止bug
        if (!isFromSky) return;
        if (transform.position.y <= downTargetPosY)
        {
            Invoke("DestroySun",5);
            return;
        }
        transform.Translate(Vector3.down * Time.deltaTime);
    }

    // 鼠标点击阳光时触发增加阳光数量，该函数需要碰撞器才有效果
    private void OnMouseDown()
    {
        PlayerManager.instance.SunNum += 25;
        // 将屏幕坐标转化为世界坐标
        Vector3 sunNumPos = Camera.main.ScreenToWorldPoint(UIManager.instance.GetSunNumTextPos());
        // 由于Z不可预测，把它变为0，注意最好别sunNumPos.z=0，应该改变的不是sunNumPos本身的值
        sunNumPos = new Vector3(sunNumPos.x, sunNumPos.y, 0);
        FlyAnimation(sunNumPos);
        AudioManager.instance.PlayEFAudio(GameManager.instance.GameConf.SunClick);
    }
    
    /// <summary>
    /// 当阳光从天空中初始化的方法
    /// </summary>
    /// <param name="downTargetPosY"></param>
    /// <param name="createPosX"></param>
    /// <param name="createPosY"></param>
    public void InitForSky(float downTargetPosY, float createPosX, float createPosY)
    {
        this.downTargetPosY = downTargetPosY;
        transform.position = new Vector3(createPosX, createPosY, 0);
        isFromSky = true;
    }
    
    /// <summary>
    /// 阳光来自太阳花的初始化
    /// </summary>
    public void InitForSunFlower(Vector2 pos)
    {
        // 修改自身的位置
        transform.position = pos;
        // 肯定来自太阳花
        isFromSky = false;
        StartCoroutine(DoJump());
        // 跳跃动作完后5s没有被点击则销毁
        Invoke("DestroySun",5);
    }

    private IEnumerator DoJump()
    {
        // 随机获得是向左跳还是向右跳
        bool isLeft = Random.Range(0, 2) == 0;
        // 获取当前太阳花Y轴的大小
        Vector3 startPos = transform.position;
        float x;
        if (isLeft)
        {
            // 往左，就把方向向量x设为负数
            x = -0.035f;
        }
        else
        {
            // 往右，就把方向向量x设为负数
            x = 0.035f;
        }
        // 把上升的速度弄成匀变速
        float speed = 0;
        // 将高度设置为当前y上方1.5，在往左上方或者右上方移动时，判断是否超过，超过y就设置为负数，即开始掉落
        while (transform.position.y <= startPos.y + 1)
        {
            yield return new WaitForSeconds(0.005f);
            // 上升时速度每次加0.005
            speed += 0.005f;
            // 在Y轴上加速
            transform.Translate(new Vector3(x,0.05f + speed,0));
        }
        // 恢复到原来的y轴水平
        while (transform.position.y >= startPos.y)
        {
            yield return new WaitForSeconds(0.005f);
            // 下降时速度每次加0.002
            speed += 0.002f;
            transform.Translate(new Vector3(x,-0.05f - speed,0));
        }
    }
    
    private void FlyAnimation(Vector3 pos)
    {
        StartCoroutine(DoFly(pos));
    }

    IEnumerator DoFly(Vector3 pos)
    {
        // 获得阳光到阳光文本的方向向量
        // Vector3.normalized的特点是当前向量是不改变的并且【返回】一个新的规范化的向量（长度为1）；
        // Vector3.Normalize的特点是改变当前向量，然后当前向量长度是1
        Vector3 direction = (pos - transform.position).normalized;
        while (Vector3.Distance(pos, transform.position) > 0.5f)
        {
            yield return new WaitForSeconds(0.01f);
            transform.Translate(direction); // 往这个方向移动
        }
        DestroySun();
    }
    
    private void DestroySun()
    {
        // 取消自身全部协程和延迟调用
        StopAllCoroutines();
        CancelInvoke();
        // 放进缓存池，不做真实销毁
        PoolManager.Instance.PushObj(GameManager.instance.GameConf.Sun, gameObject);
    }
}
