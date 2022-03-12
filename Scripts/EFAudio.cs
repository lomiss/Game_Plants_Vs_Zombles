using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EFAudio : MonoBehaviour
{
    private AudioSource audioSource;

    // 外部调用，初始化并播放音效
    public void Init(AudioClip clip)
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(clip);
    }

    private void Update()
    {   
        //如果播放完毕，就将该音效对象放进对象池中
        if (audioSource.isPlaying == false)
        {
            PoolManager.Instance.PushObj(GameManager.instance.GameConf.EFAudio, gameObject);
        }
    }
}
