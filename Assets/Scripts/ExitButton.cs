using System;
using UnityEngine;
using UnityEngine.SceneManagement;



    
public class ExitButton: MonoBehaviour
{
    public Grid grid; 
    private void OnMouseDown()
    {
        StartCoroutine(grid.LoadScene("MainScreen"));
        
    }
}
