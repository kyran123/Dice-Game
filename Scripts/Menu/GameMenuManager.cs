using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{
    public GameObject battleCanvas;
    public GameObject background;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            this.toggleScreen();
        }
    }

    public void toggleScreen()
    {
        this.battleCanvas.SetActive(!this.battleCanvas.activeSelf);
        this.background.SetActive(!this.background.activeSelf);
    }

    public void restart()
    {
        SceneManager.LoadScene("Board");
    }

    public void mainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
