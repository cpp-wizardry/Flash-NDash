using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class Setting : MonoBehaviour
{

    [SerializeField]
    public GameObject menu;
    public GameObject Info;
    public AudioClip menusound;
    private AudioSource sfxSource;
    public bool state = true;


    void Start()
    {
        Info.SetActive(false);
        sfxSource = this.AddComponent<AudioSource>();
    }


    public void switchmenu()
    {
        state = !state;
        menu.SetActive(state);
        Info.SetActive(!state);
        sfxSource.PlayOneShot(menusound);
        
    } 


}
