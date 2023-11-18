using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject screen;
    
    // Start is called before the first frame update
    void Start()
    {
        screen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowLoseScreen()
    {
        screen.SetActive(true);
        Animator animator = GetComponent<Animator>();
        if (animator)
        {
            animator.Play("GameOver");
        }
    }

    public void ExitNew()
    {
        SceneManager.LoadSceneAsync("MainScreen");
    }
    
}
