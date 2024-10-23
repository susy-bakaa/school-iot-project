using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public CanvasGroup group;
    [Scene]
    public string nextLevel;
    [Scene]
    public string previousLevel;

    private void Awake()
    {
        Hide();
    }

    public void Show()
    {
        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;
    }

    public void Hide()
    {
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;
    }

    public void NextLevel()
    {
        if (!string.IsNullOrEmpty(nextLevel))
            SceneManager.LoadScene(nextLevel);
    }

    public void PreviousLevel()
    {
        if (!string.IsNullOrEmpty(previousLevel))
            SceneManager.LoadScene(previousLevel);
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().path);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
