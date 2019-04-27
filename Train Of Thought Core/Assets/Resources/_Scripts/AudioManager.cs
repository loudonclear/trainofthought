using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;

    public AudioSource music;
    public AudioSource decisionSound;

    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    public void PlayDecisionSound( AudioClip audio)
    {
        if (decisionSound.isPlaying)
        {
            decisionSound.Stop();
        }
        decisionSound.clip = audio;
        decisionSound.Play();

    }
}