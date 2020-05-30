using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Linq;
using System;

public class DialogueGraphView : GraphView
{
    private readonly Vector2 entryPosition = new Vector2(200, 200);

    private readonly Vector2 moveSpeed = new Vector2(50, 50);


    private readonly Vector2 defaultNodeSize = new Vector2(200, 200        );

    public DialogueGraphView()
    {
        //constructor
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ClickSelector());
        var contentDragger = new ContentDragger
        {
            panSpeed = moveSpeed
        };
        this.AddManipulator(contentDragger);
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        //AddElement(GenerateEntryNode());

        style.backgroundColor = new Color(0.16f, 0.16f, 0.16f);
    }

    public DialogueNode GenerateEntryNode(Sentence sentence)
    {
        var node = new DialogueNode
        {
            title = sentence.from.Value,
            Layer = 1,
            Index = 1,
            sentence = sentence,
            GUID = System.Guid.NewGuid().ToString(),
            conversationText = "Start Text",
            isEntryPoint = true
        };

        var generatedPort = GeneratePort(node, Direction.Output);        
        generatedPort.portName = "Next";
        node.outputPorts.Add(generatedPort);
        node.outputContainer.Add(generatedPort);

        var text = new TextField();
        text.SetValueWithoutNotify(sentence.text);
        text.multiline = true;
        text.style.maxWidth = 200;
        //text.style.position = Position.Relative;
        text.style.flexBasis = 150f;        
        text.style.flexDirection = FlexDirection.Column;
        text.style.flexWrap = Wrap.Wrap;
        text.RegisterValueChangedCallback((evt) => sentence.text = evt.newValue);
        node.titleContainer.Add(text);



        // refresh the editor graphics after changing the node
        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(entryPosition, defaultNodeSize));
        return node;
    
    }

    public void RenderNode(DialogueNode node)
    {
        AddElement(node);
    }



    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        //return base.GetCompatiblePorts(startPort, nodeAdapter);
        var compatiblePorts = new List<Port>();
        ports.ForEach(funcCall: port =>
        {
            if (startPort != port && startPort.node != port.node)
            {
                compatiblePorts.Add(port);
            }
        });

        return compatiblePorts;
    }

    public DialogueNode CreateDialogueNode(string nodeName)
    {
        var newDialogueNode = new DialogueNode
        {
            title = nodeName,
            GUID = System.Guid.NewGuid().ToString(),
            conversationText = "txt..."
        };

        var generatedPort = GeneratePort(newDialogueNode, Direction.Input, Port.Capacity.Multi);
        generatedPort.portName = "Input";
        newDialogueNode.inputContainer.Add(generatedPort);

        // create a add choice button
        var addChoiceBtn = new Button(clickEvent: () => { AddChoicePort(newDialogueNode); })
        {
            text = "Add Choice"
        };
        newDialogueNode.titleContainer.Add(addChoiceBtn);

        // refresh the editor graphics after changing the node
        newDialogueNode.RefreshExpandedState();
        newDialogueNode.RefreshPorts();

        newDialogueNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));


        return newDialogueNode;
    }

    public DialogueNode CreateFollowingNode(Sentence sentence, int layer, int index,DialogueNode parentNode, int parentPortIndex)
    {
        Debug.Log($"Layer{layer}, Index{index}, {sentence.name}");
        Debug.Log($"Parent: {parentNode.Layer},{ parentNode.Index}");

        var newDialogueNode = new DialogueNode
        {
            title = sentence.from.Value,
            Layer = layer,
            sentence = sentence,
            Index = index,
            GUID = System.Guid.NewGuid().ToString(),
            conversationText = "txt..."
        };

        var inputPort = GeneratePort(newDialogueNode, Direction.Input, Port.Capacity.Multi);
        newDialogueNode.inputPort = inputPort;
        inputPort.portName = "Previous";
        Edge edge = inputPort.ConnectTo(parentNode.outputPorts[parentPortIndex]);
        Add(edge);
    
        newDialogueNode.inputContainer.Add(inputPort);        

        newDialogueNode.SetPosition(new Rect(new Vector2(300*layer-100,250*index-50), defaultNodeSize));

        if (sentence.HasOptions())
        {
            for (int i = 0; i < sentence.options.Count; i++)
            {
                var port = GeneratePort(newDialogueNode, Direction.Output);
                var t = new TextField();
                t.SetValueWithoutNotify(sentence.options[i].text);
                t.RegisterValueChangedCallback((evt) => sentence.options[i].text = evt.newValue);
                t.multiline = true;
                t.style.flexBasis = 100f;

                port.contentContainer.Add(t);
                port.portName = (i+1).ToString();
                newDialogueNode.outputPorts.Add(port);
                newDialogueNode.outputContainer.Add(port);

            }
        }
        else
        {
            var text = new TextField();
            text.SetValueWithoutNotify(sentence.text);
            text.multiline = true;
            text.style.flexBasis = 150f;

            text.RegisterValueChangedCallback((evt) => sentence.text = evt.newValue);
            newDialogueNode.titleContainer.Add(text);
            //if (sentence.nextSentence!=null)
            //{
                var port = GeneratePort(newDialogueNode, Direction.Output);
                port.portName = "Next";
                newDialogueNode.outputPorts.Add(port);
                newDialogueNode.outputContainer.Add(port);
            //}
        }

        // refresh the editor graphics after changing the node
        newDialogueNode.RefreshExpandedState();
        newDialogueNode.RefreshPorts();
        
     

        return newDialogueNode;

    }

    

    private Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        // generate a port on the node
        // we dont care about the type of variable passes between the nodes too much
        // because we focus on the connection
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }

    private void AddChoicePort(DialogueNode node)
    {
        var generatedChoicePort = GeneratePort(node, Direction.Output);

        var portCount = node.outputContainer.Query(name: "connector").ToList().Count;
        generatedChoicePort.portName = $"Choice {portCount}";


        node.outputContainer.Add(generatedChoicePort);

        // refresh the editor graphics after changing the node
        node.RefreshExpandedState();
        node.RefreshPorts();
    }

    public void AutoGenerateNodes(Sentence startingSentence)
    {
        DialogueNode entryNode = GenerateEntryNode(startingSentence);
        RenderNode(entryNode);
        DialogueNode node = null;
        if (startingSentence.nextSentence != null)
        {
            node = CreateFollowingNode(startingSentence.nextSentence, 2, 1, entryNode, 0);
            RenderNode(node);

        }
        Recursion(startingSentence.nextSentence, 2, node);

    }        


    public void SaveConnections()
    {
        ResetSentenceLinks();

        ports.ForEach(funcCall: port =>
        {

            //Debug.Log("port");
            if (port.direction==Direction.Output)
            {
                DialogueNode outputNode = (DialogueNode)port.node;
                var edgeList = port.connections.ToList();
                if (edgeList.Count!=0)
                {
                    DialogueNode inputNode = (DialogueNode)edgeList[0].input.node;
                    Debug.Log($"{outputNode.sentence.name} is connected to {inputNode.sentence.name}");
                    if (outputNode.sentence.HasOptions())
                    {
                        if (port.portName!=null)
                        {
                            int index = int.Parse(port.portName) - 1;
                            outputNode.sentence.options[index].nextSentence = inputNode.sentence;
                        }
                    }
                    else
                    {
                        outputNode.sentence.nextSentence = inputNode.sentence;
                    }
                }
            }
        });
    }

    void ResetSentenceLinks()
    {
        ports.ForEach(funcCall: port =>
        {
            DialogueNode outputNode = (DialogueNode)port.node;
            if (outputNode.sentence.HasOptions())
            {
                foreach (var option in outputNode.sentence.options)
                {
                    option.nextSentence = null;
                }
            }
            else
            {
                outputNode.sentence.nextSentence = null;
            }
        });
        
    }




    int[] layers = new int[100];
    List<Sentence> repeatedSentence = new List<Sentence>();
    List<DialogueNode> repeatedNodes = new List<DialogueNode>();

    bool CheckRepeat(Sentence sentence)
    {
        foreach (var node in repeatedNodes)
        {
            if (node.sentence == sentence)
            {
                return true;
            }
        }
        return false;
    }

    DialogueNode GetRepeatedNode(Sentence sentence)
    {
        foreach (var node in repeatedNodes)
        {
            if (node.sentence == sentence)
            {
                return node;
            }
        }
        return null;
    }

    void Recursion(Sentence sentence, int layer, DialogueNode parentNode)
    {
        Sentence currentSentence = sentence;

        while (currentSentence != null)
        {

            if (!currentSentence.HasOptions())
            {

                currentSentence = currentSentence.nextSentence;
                if (currentSentence != null)
                {
                    bool repeated = CheckRepeat(currentSentence);
                    if (!repeated)
                    {
                        //render normally
                        RenderNormally();

                    }
                    else
                    {
                        // dont render, just connect
                        Debug.Log($"{currentSentence.name} repeated");
                        Debug.Log($"connected to {parentNode.sentence.name}'s {1} output port ");

                        ConnectRepeated();
                    }

                }
                return;
            }
            else
            {
                List<Choice> options = currentSentence.options;
                for (int i = 0; i < options.Count; i++)
                {
                    currentSentence = options[i].nextSentence;
                    if (currentSentence != null)
                    {
                        bool repeated = CheckRepeat(currentSentence);
                        if (!repeated)
                        {
                            //render normally
                            RenderNormally(i);

                        }
                        else
                        {
                            // dont render, just connect
                            Debug.Log($"{currentSentence.name} repeated");
                            Debug.Log($"connected to {parentNode.sentence.name}'s {i + 1} output port ");

                            ConnectRepeated(i);
                        }

                    }

                }

                return;
            }


        }

        void ConnectRepeated(int i = 0)
        {
            DialogueNode inputNode = GetRepeatedNode(currentSentence);
            if (inputNode != null)
            {
                Port inputPort = inputNode.inputPort;
                Port outputPort = parentNode.outputPorts[i];

                Edge edge = inputPort.ConnectTo(outputPort);
                Add(edge);
            }
        }

        void RenderNormally(int i = 0)
        {
            layers[layer + 1]++;
            DialogueNode node;
            node = CreateFollowingNode(currentSentence, layer + 1, layers[layer + 1], parentNode, i);
            RenderNode(node);
            //repeatedSentence.Add(currentSentence);
            repeatedNodes.Add(node);
            Recursion(currentSentence, layer + 1, node);
        }

    }
}
