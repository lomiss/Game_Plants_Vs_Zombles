using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetPanel : MonoBehaviour
{
    public void Show(bool isShow)
    {
        gameObject.SetActive(isShow);
        // 如果显示出来则游戏暂停,这里暂停的实际上是动画、特效相关的
        if (isShow)
        {
            Time.timeScale = 0;
        }
        else
        {
            AudioManager.instance.PlayEFAudio(GameManager.instance.GameConf.ButtonClick);
            Time.timeScale = 1;
        }
    }

    public void BackMainScene()
    {
        PoolManager.Instance.Clear();
        AudioManager.instance.PlayEFAudio(GameManager.instance.GameConf.ButtonClick);
        Time.timeScale = 1;
        Invoke("DoBackMainScene",0.5f);
    }

    private void DoBackMainScene()
    {
        SceneManager.LoadScene("Start");
    }
    
    public void Quit()
    {
        AudioManager.instance.PlayEFAudio(GameManager.instance.GameConf.ButtonClick);
        Application.Quit();
    }
}
