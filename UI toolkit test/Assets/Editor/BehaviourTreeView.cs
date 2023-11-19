using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System;
using System.Linq;

public class BehaviourTreeView : GraphView
{
    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits> { }

    public Action<BehaviourNodeView> onNodeSelect;
    private BehaviourTree tree;

    public BehaviourTreeView()
    {
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet);
    }

    BehaviourNodeView FindNodeView(BehaviourNode node)
    {
        return GetNodeByGuid(node.guid) as BehaviourNodeView;
    }

    internal void PopulateView(BehaviourTree tree)
    {
        this.tree = tree;

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;
        
        tree.nodes.ForEach(n => CreateNodeView(n));

        tree.nodes.ForEach(n =>
        {
            var children = tree.GetChildren(n);
            children.ForEach(c =>
            {
                var parentView = FindNodeView(n);
                var childView = FindNodeView(c);

                var edge = parentView.output.ConnectTo(childView.input);
                AddElement(edge);
            });
        });
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports
                .ToList()
                .Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node)
                .ToList();
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if(graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach(ele =>
            {
                BehaviourNodeView nodeView = ele as BehaviourNodeView;
                if(nodeView != null)
                {
                    tree.DeleteNode(nodeView.node);
                }

                Edge edge = ele as Edge;
                if(edge != null)
                {
                    BehaviourNodeView parentView = edge.output.node as BehaviourNodeView;
                    BehaviourNodeView childView = edge.input.node as BehaviourNodeView;
                    tree.RemoveChild(parentView.node, childView.node);
                }
            });
        }

        if(graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(edge =>
            {
                BehaviourNodeView parentView = edge.output.node as BehaviourNodeView;
                BehaviourNodeView childView = edge.input.node as BehaviourNodeView;
                tree.AddChild(parentView.node, childView.node);
            });
        }
        return graphViewChange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        base.BuildContextualMenu(evt);

        var types = TypeCache.GetTypesDerivedFrom<BehaviourNode>();
        foreach(var t in types)
        {
            evt.menu.AppendAction($"[{t.BaseType.Name}] {t.Name}", _ => CreateNode(t));
        }
    }

    void CreateNode(Type t)
    {
        BehaviourNode node = tree.CreateNode(t);
        CreateNodeView(node);
    }

    void CreateNodeView(BehaviourNode node)
    {
        BehaviourNodeView nodeView = new BehaviourNodeView(node);
        nodeView.onNodeSelect = onNodeSelect;
        AddElement(nodeView);
    }
}
