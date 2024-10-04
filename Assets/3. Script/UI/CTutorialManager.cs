using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CTutorialManager : MonoBehaviour
{
    public Image[] blinkingImages;       // 여러 개의 RawImage 배열로 변경
    public TextMeshProUGUI tutorialText;    // TextMeshProUGUI로 된 텍스트
    public string[] tutorialSteps;          // 튜토리얼 단계별 텍스트 배열
    public float blinkSpeed = 0.5f;         // 깜빡이는 속도

    private int currentStep = 0;            // 현재 단계
    private bool isTutorialActive = true;   // 튜토리얼 활성 상태
    private Coroutine currentBlinkingCoroutine;  // 현재 깜빡임 코루틴을 저장

    private void Start()
    {
        DisplayStep(currentStep);  // 첫 단계 표시
        currentBlinkingCoroutine = StartCoroutine(BlinkImage(currentStep));  // 첫 번째 이미지를 깜빡임 시작
    }

    private void Update()
    {
        if (isTutorialActive && Input.GetKeyDown(KeyCode.Return))  // Enter 키로 단계 진행
        {
            NextStep();
        }
    }
    private void DisplayStep(int stepIndex)
    {
        if (stepIndex < tutorialSteps.Length)
        {
            tutorialText.text = tutorialSteps[stepIndex];  // 현재 단계에 맞는 텍스트 표시
        }
    }

    private IEnumerator BlinkImage(int stepIndex)
    {
        // 현재 단계의 이미지를 깜빡임
        while (true)
        {
            blinkingImages[stepIndex].enabled = !blinkingImages[stepIndex].enabled;
            yield return new WaitForSeconds(blinkSpeed);
        }
    }

    private void NextStep()
    {
        // 이전 단계의 깜빡임을 중지
        if (currentBlinkingCoroutine != null)
        {
            StopCoroutine(currentBlinkingCoroutine);  // 이전 코루틴 중지
        }

        // 이전 단계의 이미지 끄기
        if (currentStep < blinkingImages.Length)
        {
            blinkingImages[currentStep].enabled = false;
        }

        currentStep++;

        if (currentStep < tutorialSteps.Length)
        {
            DisplayStep(currentStep);  // 다음 단계 텍스트 표시
            currentBlinkingCoroutine = StartCoroutine(BlinkImage(currentStep));  // 다음 이미지 깜빡임 시작
        }
        else
        {
            EndTutorial();  // 튜토리얼 종료
        }
    }

    private void EndTutorial()
    {
        isTutorialActive = false;

        // 현재 깜빡임 중지
        if (currentBlinkingCoroutine != null)
        {
            StopCoroutine(currentBlinkingCoroutine);
        }

        // 모든 이미지를 끔
        foreach (var img in blinkingImages)
        {
            img.enabled = false;
        }

        tutorialText.text = "";  // 튜토리얼 텍스트 초기화
        this.gameObject.SetActive(false);  // 튜토리얼 매니저 비활성화
    }
}
