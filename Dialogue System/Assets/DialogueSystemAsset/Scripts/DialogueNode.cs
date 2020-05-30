using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class DialogueNode : Node
{
    public Port parentPort;
    public int Layer;
    public int Index;
    public Sentence sentence;
    public string GUID;
    public string conversationText;
    public bool isEntryPoint;

    public Port inputPort;
    public List<Port> outputPorts= new List<Port>();
}
