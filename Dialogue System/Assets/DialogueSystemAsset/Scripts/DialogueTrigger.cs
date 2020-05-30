using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueSO dialogue;

    public DialogueManager dialogueManager;

    public void StartDialogue()
    {
        dialogueManager.StartDialogue(dialogue);
    }

}
