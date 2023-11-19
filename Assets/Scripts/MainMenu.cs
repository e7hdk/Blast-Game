using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu: MonoBehaviour
{
    public Animator crossFade;
    public TextMeshProUGUI levelText;
    private int level;
    public GameObject screen;
    public void Start()
    {
        screen.SetActive(false);
        level = PlayerPrefs.GetInt("Level", 1);
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
            //SceneManager.LoadSceneAsync("SampleScene1");
            StartCoroutine(LoadScene());
        } 
        
        
    }

    public IEnumerator LoadScene()
    {
        screen.SetActive(true);
        crossFade.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("SampleScene1");
    }
}