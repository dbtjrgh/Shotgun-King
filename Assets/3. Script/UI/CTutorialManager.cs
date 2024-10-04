using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CTutorialManager : MonoBehaviour
{
    public Image[] blinkingImages;       // ���� ���� RawImage �迭�� ����
    public TextMeshProUGUI tutorialText;    // TextMeshProUGUI�� �� �ؽ�Ʈ
    public string[] tutorialSteps;          // Ʃ�丮�� �ܰ躰 �ؽ�Ʈ �迭
    public float blinkSpeed = 0.5f;         // �����̴� �ӵ�

    private int currentStep = 0;            // ���� �ܰ�
    private bool isTutorialActive = true;   // Ʃ�丮�� Ȱ�� ����
    private Coroutine currentBlinkingCoroutine;  // ���� ������ �ڷ�ƾ�� ����

    private void Start()
    {
        DisplayStep(currentStep);  // ù �ܰ� ǥ��
        currentBlinkingCoroutine = StartCoroutine(BlinkImage(currentStep));  // ù ��° �̹����� ������ ����
    }

    private void Update()
    {
        if (isTutorialActive && Input.GetKeyDown(KeyCode.Return))  // Enter Ű�� �ܰ� ����
        {
            NextStep();
        }
    }
    private void DisplayStep(int stepIndex)
    {
        if (stepIndex < tutorialSteps.Length)
        {
            tutorialText.text = tutorialSteps[stepIndex];  // ���� �ܰ迡 �´� �ؽ�Ʈ ǥ��
        }
    }

    private IEnumerator BlinkImage(int stepIndex)
    {
        // ���� �ܰ��� �̹����� ������
        while (true)
        {
            blinkingImages[stepIndex].enabled = !blinkingImages[stepIndex].enabled;
            yield return new WaitForSeconds(blinkSpeed);
        }
    }

    private void NextStep()
    {
        // ���� �ܰ��� �������� ����
        if (currentBlinkingCoroutine != null)
        {
            StopCoroutine(currentBlinkingCoroutine);  // ���� �ڷ�ƾ ����
        }

        // ���� �ܰ��� �̹��� ����
        if (currentStep < blinkingImages.Length)
        {
            blinkingImages[currentStep].enabled = false;
        }

        currentStep++;

        if (currentStep < tutorialSteps.Length)
        {
            DisplayStep(currentStep);  // ���� �ܰ� �ؽ�Ʈ ǥ��
            currentBlinkingCoroutine = StartCoroutine(BlinkImage(currentStep));  // ���� �̹��� ������ ����
        }
        else
        {
            EndTutorial();  // Ʃ�丮�� ����
        }
    }

    private void EndTutorial()
    {
        isTutorialActive = false;

        // ���� ������ ����
        if (currentBlinkingCoroutine != null)
        {
            StopCoroutine(currentBlinkingCoroutine);
        }

        // ��� �̹����� ��
        foreach (var img in blinkingImages)
        {
            img.enabled = false;
        }

        tutorialText.text = "";  // Ʃ�丮�� �ؽ�Ʈ �ʱ�ȭ
        this.gameObject.SetActive(false);  // Ʃ�丮�� �Ŵ��� ��Ȱ��ȭ
    }
}
