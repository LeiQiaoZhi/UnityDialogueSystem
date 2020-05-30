using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;


public class DialogueGraph : EditorWindow
{
    ObjectField sentenceField;
    DialogueGraphView graphView;
    Toolbar toolbar;
    Sentence startingSentence;

    [MenuItem("Tools/Dialogue Graph")]
    public static void OpenDGraphWindow()
    {
        var window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent( "Dialogue Graph");
    }

    private void OnEnable()
    {
        Construct();
        ConstructToolBar();
    }

    void Construct()
    {
        graphView = new DialogueGraphView
        {
            name = "Dialogue Graph"
        };

        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
        
        graphView.SendToBack();
    }


    void ConstructToolBar()
    {
        toolbar = new Toolbar();
        //var createNodeBtn = new Button(clickEvent: () => { graphView.RenderNode(graphView.CreateDialogueNode("Dialogue Node")); })
        //{
        //    text = "Create Dialogue Node"
        //};
       
        var btn = new Button(clickEvent: () =>
        {
            DialogueSO dialogue = (DialogueSO)sentenceField.value;
            startingSentence = dialogue.startingSentence;
            if (startingSentence!=null)
            {
                rootVisualElement.Remove(graphView);
                Construct();
                graphView.AutoGenerateNodes(startingSentence);
            }

        })
        {
            text = "Show Dialogue Graph"
        };

        var saveBtn = new Button(clickEvent: () => { graphView.SaveConnections(); })
        {
            text = "Save"
        };
        var clearBtn = new Button(clickEvent: () => { Clear(); })
        {
            text = "Clear"
        };

        sentenceField = new ObjectField();
        sentenceField.objectType = typeof(DialogueSO);
        toolbar.Add(sentenceField);
        toolbar.Add(btn);
        toolbar.Add(saveBtn);
        toolbar.Add(clearBtn);


        rootVisualElement.Add(toolbar);
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(graphView);
    }

    void Clear()
    {
        rootVisualElement.Remove(graphView);
        rootVisualElement.Remove(toolbar);

        Construct();
        ConstructToolBar();

    }



}
