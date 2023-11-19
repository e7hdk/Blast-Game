using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Congrat : MonoBehaviour
{
    public GameObject congratScreen;
    // Start is called before the first frame update
    void Start()
    {
        congratScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowCongratScreen()
    {
        
        StartCoroutine(WaitForMenu());

        
    }

    public IEnumerator WaitForMenu()
    {
        congratScreen.SetActive(true);
        Animator animator = GetComponent<Animator>();
        
        animator.Play("Congrat");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("MainScreen");
    }
}
