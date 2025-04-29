using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    // Singleton pattern để đảm bảo chỉ có một AudioManager trong game
    public static AudioManager instance;

    // AudioSources cho các loại nhạc khác nhau
    private AudioSource mainMenuMusic;
    private AudioSource gameplayMusic;
    private AudioSource battleMusic;

    // Danh sách các AudioClip
    public AudioClip mainMenuClip;    // "02 Opening (part 2).mp3"
    public AudioClip gameplayClip;     // "11 Vermilion City's Theme.mp3"
    public AudioClip battleClip;       // "24 Battle (VS Wild Pokemon).mp3"

    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SetupAudioSources();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetupAudioSources()
    {
        // Tạo AudioSource cho menu chính
        mainMenuMusic = gameObject.AddComponent<AudioSource>();
        mainMenuMusic.clip = mainMenuClip;
        mainMenuMusic.loop = true;
        mainMenuMusic.volume = 0.7f;

        // Tạo AudioSource cho gameplay
        gameplayMusic = gameObject.AddComponent<AudioSource>();
        gameplayMusic.clip = gameplayClip;
        gameplayMusic.loop = true;
        gameplayMusic.volume = 0.7f;

        // Tạo AudioSource cho battle
        battleMusic = gameObject.AddComponent<AudioSource>();
        battleMusic.clip = battleClip;
        battleMusic.loop = true;
        battleMusic.volume = 0.7f;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Dựa vào tên scene, phát nhạc tương ứng
        switch (scene.name)
        {
            case "Main Menu":
                PlayMainMenuMusic();
                break;
            case "SampleScene":
                PlayGameplayMusic();
                break;
            // Thêm case cho scene battle nếu có
            default:
                break;
        }
    }

    public void PlayMainMenuMusic()
    {
        StopAllMusic();
        mainMenuMusic.Play();
    }

    public void PlayGameplayMusic()
    {
        StopAllMusic();
        gameplayMusic.Play();
    }

    public void PlayBattleMusic()
    {
        StopAllMusic();
        battleMusic.Play();
    }

    private void StopAllMusic()
    {
        mainMenuMusic.Stop();
        gameplayMusic.Stop();
        battleMusic.Stop();
    }
} 