using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BackgroundMusic : MonoBehaviour
{
    int sceneIndex=-1;
    AudioSource backMusicSource;
    Sound song;

    private void Start()
    {
        backMusicSource = gameObject.AddComponent<AudioSource>();
    }
    void Update()
    {
        if( sceneIndex!= SceneManager.GetActiveScene().buildIndex)
        {
            
            switch (SceneManager.GetActiveScene().buildIndex)
            {
                case 0:
                    song = FindObjectOfType<AudioManager>().GetSoundByName("CitySong");
                    sceneIndex = SceneManager.GetActiveScene().buildIndex;

                    break;
                case 1:
                    song = FindObjectOfType<AudioManager>().GetSoundByName("Cyber1");
                    sceneIndex = SceneManager.GetActiveScene().buildIndex;
                    
                    break;
                case 2:
                    song = FindObjectOfType<AudioManager>().GetSoundByName("CitySong");
                    sceneIndex = SceneManager.GetActiveScene().buildIndex;
                   
                    break;
               
            }
            backMusicSource.clip = song.clip;
            backMusicSource.Play();
            backMusicSource.loop = true;
        }        
    }
}
