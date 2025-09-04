using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameModePanel;
    public GameObject difficultyPanel;
    public static GameManager.GameMode selectedMode;
    
    
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void OpenGameModeMenu()
    {
        mainMenuPanel.SetActive(false);
        gameModePanel.SetActive(true);
    }
    
    public void StartLocalGame()
    {
        selectedMode = GameManager.GameMode.LocalMultiPlayer;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene("Scenes/SampleScene");
    }

    public void StartVsAI()
    {
        selectedMode = GameManager.GameMode.AI;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene("Assets/Scenes/SampleScene.unity");
    }

    
    public void OpenDifficultyMenu()
    {
        gameModePanel.SetActive(false);
        difficultyPanel.SetActive(true);
    }

    public void ExitDifficultyMenu()
    {
        gameModePanel.SetActive(true);
        difficultyPanel.SetActive(false);
    }

    public void BackToMainMenu()
    {
        gameModePanel.SetActive(false);
        difficultyPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
