using System;
using System.Collections.Generic;
using FRPGraph.Editor.Nodes;
using UnityEngine;

[Serializable]
public class FrpGraphContainer : ScriptableObject
{
    public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
    public List<FrpNodeData> FrpNodeData = new List<FrpNodeData>();
    public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();
    public List<CommentBlockData> CommentBlockData = new List<CommentBlockData>();
}
