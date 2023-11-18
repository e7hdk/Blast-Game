using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class ExitButton: MonoBehaviour
    {
        private void OnMouseDown()
        {
            SceneManager.LoadSceneAsync("MainScreen");
        }
    }
}