using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReplayButton : MonoBehaviour
{
    public Grid grid;
    private void OnMouseDown()
    {
        StartCoroutine(grid.LoadScene("SampleScene1"));
        //SceneManager.LoadSceneAsync("SampleScene1");
        
    }
}
