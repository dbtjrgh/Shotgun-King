using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CSoundManager : MonoBehaviour
{
    public static CSoundManager Instance { get; private set; } // 싱글톤 인스턴스

    public AudioSource bgm_player;
    public AudioSource sfx_player;

    public Slider bgm_slider;
    public Slider sfx_slider;

    // 씬별 BGM 오디오 클립을 매핑
    public AudioClip[] bgmClips;

    public AudioClip[] sfxclips; // sfxclips 배열을 추가

    private Dictionary<string, AudioClip> sceneBgmMap;

    void Awake()
    {
        // 싱글톤 패턴 적용
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 삭제되지 않도록 설정

            // 씬 로드 이벤트 구독
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 있으면 중복 생성된 객체 삭제
            return;
        }

        bgm_player = GameObject.Find("BGM").GetComponent<AudioSource>();
        sfx_player = GameObject.Find("SFX").GetComponent<AudioSource>();

        bgm_slider = bgm_slider.GetComponent<Slider>();
        sfx_slider = sfx_slider.GetComponent<Slider>();

        bgm_slider.onValueChanged.AddListener(ChangeBgmSound);
        sfx_slider.onValueChanged.AddListener(ChangeSfxSound);

        // 씬 이름과 BGM 클립을 매핑하는 Dictionary 초기화
        sceneBgmMap = new Dictionary<string, AudioClip>()
        {
            { "TitleScene", bgmClips[0] },  // TitleScene에 첫 번째 BGM
            { "TutorialScene", bgmClips[1] },    // TutorialScene 씬에 두 번째 BGM
            { "island1-2StageScene", bgmClips[2] },  // island1-2StageScene 씬에 세 번째 BGM
            { "Valcano5StageScene", bgmClips[3] },  // Valcano5StageScene 씬에 4 번째 BGM
            { "EnddingScene", bgmClips[4] }  // EnddingScene 씬에 5 번째 BGM
        };
    }

    private void Update()
    {
        if (bgm_slider == null)
        {
            GameObject bgmSliderObject = GameObject.Find("BGMSlider");
            if (bgmSliderObject != null)
            {
                bgm_slider = bgmSliderObject.GetComponent<Slider>();
                if (bgm_slider != null)
                {
                    bgm_slider.onValueChanged.AddListener(ChangeBgmSound);
                }
            }
        }

        if (sfx_slider == null)
        {
            GameObject sfxSliderObject = GameObject.Find("SFXSlider");
            if (sfxSliderObject != null)
            {
                sfx_slider = sfxSliderObject.GetComponent<Slider>();
                if (sfx_slider != null)
                {
                    sfx_slider.onValueChanged.AddListener(ChangeSfxSound);
                }
            }
        }
    }

    public void PlaySfx(int index)
    {
        if (index < 0 || index >= sfxclips.Length)
        {
            Debug.LogWarning("SFX index out of range.");
            return;
        }

        sfx_player.PlayOneShot(sfxclips[index]);
    }

    // 씬이 로드될 때마다 호출되는 메서드
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBgmForCurrentScene();
    }

    void PlayBgmForCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (sceneBgmMap.ContainsKey(currentSceneName))
        {
            bgm_player.clip = sceneBgmMap[currentSceneName]; // 씬에 맞는 BGM 설정
            bgm_player.Play(); // BGM 재생
        }
        else
        {
            Debug.LogWarning("No BGM set for this scene.");
        }
    }

    void ChangeBgmSound(float value)
    {
        bgm_player.volume = value;
    }

    void ChangeSfxSound(float value)
    {
        sfx_player.volume = value;
    }

    // 씬 로드 이벤트 구독 해제 (오브젝트가 파괴될 때 호출)
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
