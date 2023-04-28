using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    void Awake()
    {
        DontDestroyOnLoad(this);

        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

    public void Play(string name, float newPitch, float newVolume)
    {
       Sound s =  System.Array.Find(sounds, sound=> sound.name ==name);
        s.source.pitch = newPitch;
        s.source.volume = newVolume;
        s.source.Play();
    }
    public void ChangeVolume( Sound s ,float newVolume)
    {
        s.source.volume = newVolume;
    }
    public Sound GetSoundByName(string name)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        return s;
    }
}
