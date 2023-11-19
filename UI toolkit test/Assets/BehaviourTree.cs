using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif

[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject
{
    public BehaviourNode rootNode;
    public List<BehaviourNode> nodes = new();

#if UNITY_EDITOR
    public BehaviourNode CreateNode(Type t)
    {
        BehaviourNode node = CreateInstance(t) as BehaviourNode;
        node.name = t.Name;
        node.guid = GUID.Generate().ToString();
        nodes.Add(node);

        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();
        return node;
    }

    public void DeleteNode(BehaviourNode node)
    {
        nodes.Remove(node);
        AssetDatabase.RemoveObjectFromAsset(node);
        AssetDatabase.SaveAssets();
    }

    public void AddChild(BehaviourNode parent, BehaviourNode child)
    {
        parent.children.Add(child);
    }

    public void RemoveChild(BehaviourNode parent, BehaviourNode child)
    {
        parent.children.Remove(child);
    }

    public List<BehaviourNode> GetChildren(BehaviourNode parent)
    {
        return parent.children;
    }
#endif
}
