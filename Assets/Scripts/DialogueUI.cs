using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text nameText;
    public TMP_Text dialogueText;
    public Image portraitImage;

    public static DialogueUI instance;

    void Awake()
    {
        instance = this;
        panel.SetActive(false);
    }

    public void ShowDialogue(string npcName, Sprite portrait, string dialogue)
    {
        panel.SetActive(true);
        nameText.text = npcName;
        portraitImage.sprite = portrait;
        dialogueText.text = dialogue;
    }


    public void HideDialogue()
    {
        panel.SetActive(false);
    }
}

