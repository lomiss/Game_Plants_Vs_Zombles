using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour
{
    public void GoEndless()
    {
        // 请理对象池
        PoolManager.Instance.Clear();
        // 播放音效
        AudioManager.instance.PlayEFAudio(GameManager.instance.GameConf.ButtonClick);
        Time.timeScale = 1;
        // 防止切换场景把点击音效给吃了
        Invoke("DoGoEndless",0.5f);
    }

    private void DoGoEndless()
    {
        SceneManager.LoadScene("Endless");
    }

    public void Quit()
    {
        AudioManager.instance.PlayEFAudio(GameManager.instance.GameConf.ButtonClick);
        Application.Quit();
    }
}
