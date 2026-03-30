using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{

    public AudioMixer backgroundMusic;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetBackgroundVolume(float gameVolume)
    {
        //Allows for the background music's volume to be controlled based on the settings of the game volume within the game
        backgroundMusic.SetFloat("BackgroundMusic", gameVolume);
    }
}
