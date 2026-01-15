using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private const string DIFFICULTY_KEY = "GameDifficulty";

    public void PlayEasy()
    {
        StartGame(Difficulty.Easy);
    }

    public void PlayNormal()
    {
        StartGame(Difficulty.Normal);
    }

    public void PlayHard()
    {
        StartGame(Difficulty.Hard);
    }

    private void StartGame(Difficulty difficulty)
    {
        PlayerPrefs.SetInt(DIFFICULTY_KEY, (int)difficulty);
        PlayerPrefs.Save();

        SceneManager.LoadScene("GameScene");
    }

    public void GoToMenu(){
        SceneManager.LoadScene("MenuScene");
    }
}
