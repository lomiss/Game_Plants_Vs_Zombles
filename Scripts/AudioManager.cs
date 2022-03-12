using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // 保证一个唯一性
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 存在多个就删除
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 播放特效音乐
    /// </summary>
    public void PlayEFAudio(AudioClip clip)
    {
        // 从对象池获取一个音效物体
        EFAudio ef = PoolManager.Instance.GetObj(GameManager.instance.GameConf.EFAudio).GetComponent<EFAudio>();
        // 开始播放
        ef.Init(clip);
    }
}
