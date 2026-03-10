using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject creditsPanel;

    //开始游戏
    public void StartGame()
    {
        SceneManager.LoadScene("GameScenes");
    }

    //打开制作人名单
    public void OpenCredits()
    {
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(true);
        }
    }

    //关闭制作人名单
    public void CloseCredits()
    {
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(false);
        }
    }

    //退出游戏
    public void QuitGame()
    {
        Debug.Log("Quit Game");

        Application.Quit();
    }
}