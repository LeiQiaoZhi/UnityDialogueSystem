using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Dialogue",menuName ="Dialogue")]
public class DialogueSO : ScriptableObject
{       
    //public string with;                  
    public Sentence startingSentence;
    public bool isAvailable;                          
}



//public class Sentence
//{
//    public string from;
//    [TextArea(3,10)]
//    public string text;
//    public Options[] options;
//}

//[System.Serializable]
//public class Options
//{
//    public string option;
//    public int nextInt;
    
//}