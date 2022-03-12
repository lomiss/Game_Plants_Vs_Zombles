using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OverPanel : MonoBehaviour
{
    private Image image;
    private Image panel;
    
    void Start()
    {
        image = transform.Find("Image").GetComponent<Image>();
        panel = transform.Find("Panel").GetComponent<Image>();
        image.gameObject.SetActive(false);
        panel.gameObject.SetActive(false);
    }

    public void Over()
    {
        // 显示图片
        image.gameObject.SetActive(true);
        // 让panel逐渐变成黑色
        panel.gameObject.SetActive(true);
        StartCoroutine(PanelColorEF());
    }

    IEnumerator PanelColorEF()
    {
        float a = 0;
        while (a < 1)
        {
            a += 0.02f;
            panel.color = new Color(0, 0, 0, a);
            yield return new WaitForSeconds(0.05f);
        }
        // 此时已经全黑，停顿2s，回到主界面
        yield return new WaitForSeconds(2f);
        DoBackMainScene();
    }

    private void DoBackMainScene()
    {
        SceneManager.LoadScene("Start");
    }
}
