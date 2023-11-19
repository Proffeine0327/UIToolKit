using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BehaviourNode : ScriptableObject
{
    public Vector2 position;
    public string guid;

    public List<BehaviourNode> children = new();
}