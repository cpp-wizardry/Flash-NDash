using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Range(0f, 1f)]
    public float AudioVolume = 1f;

    [SerializeField] private AudioSource[] BackgroundMusic;
    [SerializeField] private AudioClip[] BackgroundNoise;

    [SerializeField] private AudioClip GameFinished;
    [SerializeField] private AudioClip Flash;
    [SerializeField] private AudioClip VoiceLine;
    [SerializeField] private AudioClip CamOut;

    [SerializeField] private AudioClip NicePicture;
    [SerializeField] private AudioClip BadPicture;
    [SerializeField] private AudioClip MidPicture;
    [SerializeField] private AudioClip GameBegin;

    private AudioSource sfxSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        sfxSource = gameObject.AddComponent<AudioSource>();
    }

    public void SetVolume(float volume)
    {
        AudioVolume = Mathf.Clamp01(volume);
        sfxSource.volume = AudioVolume;

        foreach (var music in BackgroundMusic)
            if (music != null) music.volume = AudioVolume;
    }

    public void PlayBackgroundMusic(int index)
    {
        if (index < 0 || index >= BackgroundMusic.Length) return;

        StopAllBackgroundMusic();
        BackgroundMusic[index].volume = AudioVolume;
        BackgroundMusic[index].loop = true;
        BackgroundMusic[index].Play();
    }

    public void StopAllBackgroundMusic()
    {
        foreach (var music in BackgroundMusic)
            if (music != null) music.Stop();
    }

    public void PlayGameBegin() => PlayClip(GameBegin);
    public void PlayFlash() => PlayClip(Flash);
    public void PlayMidPicture() => PlayClip(MidPicture);
    public void PlayBadPicture() => PlayClip(BadPicture);
    public void PlayCamOut() => PlayClip(CamOut);
    public void PlayNicePicture() => PlayClip(NicePicture);
    public void PlayVoiceLine() => PlayClip(VoiceLine);
    public void PlayClip(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, AudioVolume);
    }
}