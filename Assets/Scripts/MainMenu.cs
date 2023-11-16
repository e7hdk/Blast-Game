using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu: MonoBehaviour
{
    public TextMeshProUGUI levelText;

    public void Start()
    {
        var level = PlayerPrefs.GetInt("Level", 1);
        if (level > 10)
        {
            levelText.text = "Finished";
        }
        levelText.text = $"Level {level}";
    }

    public void PLayGame()
    {
        if (PlayerPrefs.GetInt("Level") <= 10)
        {
            SceneManager.LoadSceneAsync("SampleScene");
        }    
        
    }
}