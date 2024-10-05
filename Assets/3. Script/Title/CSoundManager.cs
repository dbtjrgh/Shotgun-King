using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CSoundManager : MonoBehaviour
{
    public static CSoundManager Instance { get; private set; } // �̱��� �ν��Ͻ�

    public AudioSource bgm_player;
    public AudioSource sfx_player;

    public Slider bgm_slider;
    public Slider sfx_slider;

    // ���� BGM ����� Ŭ���� ����
    public AudioClip[] bgmClips;

    public AudioClip[] sfxclips; // sfxclips �迭�� �߰�

    private Dictionary<string, AudioClip> sceneBgmMap;

    void Awake()
    {
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� �������� �ʵ��� ����

            // �� �ε� �̺�Ʈ ����
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� ������ �ߺ� ������ ��ü ����
            return;
        }

        bgm_player = GameObject.Find("BGM").GetComponent<AudioSource>();
        sfx_player = GameObject.Find("SFX").GetComponent<AudioSource>();

        bgm_slider = bgm_slider.GetComponent<Slider>();
        sfx_slider = sfx_slider.GetComponent<Slider>();

        bgm_slider.onValueChanged.AddListener(ChangeBgmSound);
        sfx_slider.onValueChanged.AddListener(ChangeSfxSound);

        // �� �̸��� BGM Ŭ���� �����ϴ� Dictionary �ʱ�ȭ
        sceneBgmMap = new Dictionary<string, AudioClip>()
        {
            { "TitleScene", bgmClips[0] },  // TitleScene�� ù ��° BGM
            { "TutorialScene", bgmClips[1] },    // TutorialScene ���� �� ��° BGM
            { "island1-2StageScene", bgmClips[2] },  // island1-2StageScene ���� �� ��° BGM
            { "Valcano5StageScene", bgmClips[3] },  // Valcano5StageScene ���� 4 ��° BGM
            { "EnddingScene", bgmClips[4] }  // EnddingScene ���� 5 ��° BGM
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

    // ���� �ε�� ������ ȣ��Ǵ� �޼���
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBgmForCurrentScene();
    }

    void PlayBgmForCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (sceneBgmMap.ContainsKey(currentSceneName))
        {
            bgm_player.clip = sceneBgmMap[currentSceneName]; // ���� �´� BGM ����
            bgm_player.Play(); // BGM ���
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

    // �� �ε� �̺�Ʈ ���� ���� (������Ʈ�� �ı��� �� ȣ��)
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
