using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;

public class BehaviourNodeView : Node
{
    public Action<BehaviourNodeView> onNodeSelect;
    public BehaviourNode node;
    public Port input;
    public Port output;

    public BehaviourNodeView(BehaviourNode node)
    {
        this.node = node;
        this.title = node.name;
        this.viewDataKey = node.guid;
        style.left = node.position.x;
        style.top = node.position.y;

        CreateInputPorts();
        CreateOutPutPorts();
    }

    private void CreateOutPutPorts()
    {
        input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));

        if(input != null)
        {
            input.portName = "";
            inputContainer.Add(input);
        }
    }

    private void CreateInputPorts()
    {
        output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));

        if(output != null)
        {
            output.portName = "";
            outputContainer.Add(output);
        }
    }

    public override void SetPosition(Rect newPos)
    {
        newPos.position = Snapping.Snap(newPos.position, Vector2.one * 20);
        base.SetPosition(newPos);
        node.position.x = newPos.xMin;
        node.position.y = newPos.yMin;
    }

    public override void OnSelected()
    {
        base.OnSelected();
        onNodeSelect?.Invoke(this);
    }
}