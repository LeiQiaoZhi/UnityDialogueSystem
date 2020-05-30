using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class DialogueWindow : EditorWindow
{
    float leftMargin;
    float fullWidth;

    Sentence startingSentence;
    bool follow;

    [MenuItem("Window/Dialogue")]
    public static void ShowWindow()
    {
        GetWindow<DialogueWindow>("Dialogue");
        
    }

    private void OnGUI()
    {
        float leftMargin = position.width * 0.05f;
        float fullWidth = position.width - leftMargin * 2;

        startingSentence = (Sentence)EditorGUI.ObjectField(new Rect(leftMargin, 10, fullWidth, 20), "starting sentence", startingSentence, typeof(Sentence), false);

        if (startingSentence!=null)
        {        
            startingSentence.text = EditorGUI.TextField(new Rect(leftMargin, 30, fullWidth, 20),"Text", startingSentence.text);
        }

        //if (GUI.Button(new Rect(leftMargin,60,fullWidth,20),"Button"))
        //{
        //    if (startingSentence==null||startingSentence.nextSentence == null)
        //    {
        //        // show error
        //    }
        //    else
        //    {
        //        //FollowDialogue();
        //        follow = true;
        //    }
        //}


        if (startingSentence == null || startingSentence.nextSentence == null)
        {
            // show error
        }
        else
        {
            //FollowDialogue();
            follow = true;
        }

        if (follow)
        {            
            Sentence currentSentence;
            currentSentence = startingSentence;
            int h = 0;

            while (currentSentence.nextSentence != null)
            {
                currentSentence = currentSentence.nextSentence;
                currentSentence = (Sentence)EditorGUI.ObjectField(new Rect(leftMargin, 60 + h*55, fullWidth, 20), "sentence", currentSentence, typeof(Sentence), false);
                currentSentence.text = EditorGUI.TextField(new Rect(leftMargin, 85 + h*55, fullWidth, 20), "Text", currentSentence.text);
                h++;
            }
        }


    }

    void FollowDialogue()
    {
        Debug.Log("follow");
        startingSentence.nextSentence.text = EditorGUI.TextField(new Rect(leftMargin, 90, fullWidth, 20), "Text", startingSentence.nextSentence.text);
    }

}
