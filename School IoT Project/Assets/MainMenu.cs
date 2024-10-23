using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public CanvasGroup mainGroup;
    public CanvasGroup levelGroup;
    [Scene]
    public string nextLevel;

    private void Awake()
    {
        ShowMainMenu();
        HideLevelMenu();
    }

    public void LoadFirstLevel()
    {
        if (!string.IsNullOrEmpty(nextLevel))
        {
            SceneManager.LoadScene(nextLevel);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ShowMainMenu()
    {
        mainGroup.alpha = 1f;
        mainGroup.interactable = true;
        mainGroup.blocksRaycasts = true;
    }

    public void HideMainMenu()
    {
        mainGroup.alpha = 0f;
        mainGroup.interactable = false;
        mainGroup.blocksRaycasts = false;
    }

    public void ShowLevelMenu()
    {
        levelGroup.alpha = 1f;
        levelGroup.interactable = true;
        levelGroup.blocksRaycasts = true;
    }

    public void HideLevelMenu()
    {
        levelGroup.alpha = 0f;
        levelGroup.interactable = false;
        levelGroup.blocksRaycasts = false;
    }
}
