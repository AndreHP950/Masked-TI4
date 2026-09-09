using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;
    public static AudioManager instance;
    public AudioClip mainMusic;
    public AudioClip[] audios;
    public AudioMixer mixer;
    public Slider masterSlider, musicSlider, sfxSlider;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        musicSource.clip = mainMusic;
        musicSource.Play();

        // Os métodos abaixo conferem se o usuário já jogou o jogo e tem os presets de volume alterados, se sim, vai carregar os últimos presets de volumes, se não vai definir o preset padrão
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            LoadMasterVolume();
        }
        else
        {
            SetMasterVolume();
        }

        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadMusicVolume();
        }
        else
        {
            SetMusicVolume();
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            LoadSFXVolume();
        }
        else
        {
            SetSFXVolume();
        }
    }

    /*
     * Métodos para tocar faixas de audio com base no array de AudioClips
     */
    public void PlayMusic(AudioClip clip)
    {
        musicSource.PlayOneShot(clip);
    }
    public void PlayAmbiance(int index) // Talvez coloque junto de SFX dependendo se a gente for ter muito som ambiente, se não tiver pode ser retirado
    {
        sfxSource.clip = audios[index];
        sfxSource.Play();
    }
    public void PlaySFX(int index)
    {
        sfxSource.clip = audios[index];
        sfxSource.PlayOneShot(audios[index]);
    }
    /*
     * Métodos para parar de reproduzir o audio
     */
    public void StopAmbiance()
    {
        sfxSource.Stop();
    }
    public void StopMusic()
    {
        sfxSource.Stop();
    }
    public void StopSFX()
    {
        sfxSource.Stop();
    }
    /*
     * Os métodos abaixo salvam as configurações de volume que o usuário inseriu durante o jogo e salvam quando ele sair do jogo
     */
    public void SetMasterVolume()
    {
        mixer.SetFloat("Master", Mathf.Log10(masterSlider.value) * 20);
        PlayerPrefs.SetFloat("masterVolume", masterSlider.value);
        PlayerPrefs.Save();
    }
    public void SetMusicVolume()
    {
        mixer.SetFloat("Music", Mathf.Log10(musicSlider.value) * 20);
        PlayerPrefs.SetFloat("musicVolume", musicSlider.value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume()
    {
        mixer.SetFloat("SFX", Mathf.Log10(sfxSlider.value) * 20);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        PlayerPrefs.Save();
    }

    /*
     * Os métodos abaixo carregam os presets de volume que o usuário inseriu previamente/na ultima vez que ele entrou no jogo
     */
    private void LoadMasterVolume() 
    {
        masterSlider.value = PlayerPrefs.GetFloat("masterVolume", 0.7f);
        SetMasterVolume();
    }
    private void LoadMusicVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume", 0.7f);
        SetMusicVolume();
    }
    private void LoadSFXVolume()
    {
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.7f);
        SetSFXVolume();
    }
}