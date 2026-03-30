using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadControlsMenu()
    {
        SceneManager.LoadScene("ControlsMenu");
    }

    public void LoadOptionsMenu()
    {
        SceneManager.LoadScene("OptionsMenu");
    }

    public void LoadStartMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }
}
