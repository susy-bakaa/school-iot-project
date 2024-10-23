using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [Scene]
    public string scene;
    public bool auto = false;

    private void Awake()
    {
        if (auto)
            Load();
    }

    public void Load()
    {
        SceneManager.LoadScene(scene);
    }
}
