using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CStoryManager : MonoBehaviour
{
    public Image storyImage;
    public TextMeshProUGUI storyText;
    public Sprite[] images;
    public string[] storyTexts;

    public float textSpeed = 0.05f;
    private int currentIndex = 0;
    private bool isTextComplete = false;
    private bool isSkippingText = false;

    void Start()
    {
        DisplayStory(currentIndex);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isTextComplete)
            {
                isSkippingText = true;
            }
            else
            {
                currentIndex++;
                if (currentIndex < images.Length)
                {
                    DisplayStory(currentIndex);
                }
                else
                {
                    SceneManager.LoadScene("TitleScene");
                }
            }
        }
    }

    void DisplayStory(int index)
    {
        storyImage.sprite = images[index];

        StartCoroutine(TypeText(storyTexts[index]));
    }

    IEnumerator TypeText(string text)
    {
        storyText.text = "";
        isTextComplete = false;
        isSkippingText = false;

        foreach (char letter in text.ToCharArray())
        {
            if (isSkippingText)
            {
                storyText.text = text;
                break;
            }
            storyText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }

        isTextComplete = true;
    }
}
