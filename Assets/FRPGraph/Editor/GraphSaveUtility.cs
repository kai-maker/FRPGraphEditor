using System.Collections.Generic;
using System.Linq;
using FRPGraph.Editor;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class GraphSaveUtility
{
    private FRPGraphView _targetGraphView;
    private FrpGraphContainer _containerCache;

    private List<Edge> Edges => _targetGraphView.edges.ToList();
    private List<FrpNode> Nodes => _targetGraphView.nodes.ToList().Cast<FrpNode>().ToList();

    private List<Group> CommentBlocks =>
        _targetGraphView.graphElements.ToList().Where(x => x is Group).Cast<Group>().ToList();

    public static GraphSaveUtility GetInstance(FRPGraphView targetGraphView)
    {
        return new GraphSaveUtility
        {
            _targetGraphView = targetGraphView
        };
    }

    public void SaveGraph(string fileName)
    {
        var frpGraphContainer = ScriptableObject.CreateInstance<FrpGraphContainer>();
        if (!SaveNodes(frpGraphContainer)) return;
        SaveExposedProperties(frpGraphContainer);
        SaveCommentBlocks(frpGraphContainer);

        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");

        AssetDatabase.CreateAsset(frpGraphContainer, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();
    }

    private void SaveExposedProperties(FrpGraphContainer dialogueContainer)
    {
        dialogueContainer.ExposedProperties.AddRange(_targetGraphView.ExposedProperties);
    }

    private void SaveCommentBlocks(FrpGraphContainer dialogueContainer)
    {
        foreach (var block in CommentBlocks)
        {
            var nodes = block.containedElements.Where(x => x is FrpNode).Cast<FrpNode>().Select(x => x.frpNodeData.Guid)
                .ToList();

            dialogueContainer.CommentBlockData.Add(new CommentBlockData
            {
                ChildNodes = nodes,
                Title = block.title,
                Position = block.GetPosition().position
            });
        }
    }

    public void LoadGraph(string fileName)
    {
        _containerCache = Resources.Load<FrpGraphContainer>(fileName);
        if (_containerCache == null)
        {
            EditorUtility.DisplayDialog("File Not Found", "Target dialogue graph file does not exists!", "OK");
            return;
        }

        ClearGraph();
        CreateNodes();
        ConnectNodes();
        //CreateExposedProperties();
        //GenerateCommentBlocks();
    }

    private void CreateExposedProperties()
    {
        _targetGraphView.ClearBlackBoardAndExposedProperties();

        foreach (var exposedProperty in _containerCache.ExposedProperties)
        {
            _targetGraphView.AddPropertyToBlackBoard(exposedProperty);
        }
    }

    private void ConnectNodes()
    {
        var connections = _containerCache.NodeLinks.ToList();
        for (var j = 0; j < connections.Count; ++j)
        {
            var baseNodeGuid = connections[j].BaseNodeGuid;
            var baseNode = Nodes.First(x => x.frpNodeData.Guid == baseNodeGuid);
            var targetNodeGuid = connections[j].TargetNodeGuid;
            var targetNode = Nodes.First(x => x.frpNodeData.Guid == targetNodeGuid);
            var edge = LinkNodes(baseNode.outputContainer[connections[j].BasePortNum].Q<Port>(),
                targetNode.inputContainer[connections[j].TargetPortNum].Q<Port>());
            
            //Load Connection Name
            {
                edge.edgeControl.Q<TextField>().value = connections[j].ConnectionName;
            }

            targetNode.SetPosition(new Rect(
                _containerCache.FrpNodeData.First(x => x.Guid==targetNodeGuid).Position,
                _targetGraphView.defaultNodeSize));
        }
    }

    private bool SaveNodes(FrpGraphContainer frpGraphContainer)
    {
        if (!Edges.Any()) return false;
        
        var edges = Edges.ToArray();
        for (var i = 0; i < edges.Length; ++i)
        {
            var outputNode = edges[i].output.node as FrpNode;
            var inputNode = edges[i].input.node as FrpNode;

            var basePortName = edges[i].output.portName;

            var basePortNames = new List<string>();
            for (int j=0; j<outputNode.outputContainer.childCount; ++j)
            {
                basePortNames.Add(outputNode.outputContainer[j].Q<Port>().portName);
            }

            var basePortIndex = basePortNames.FindIndex(x => x == basePortName);
            
            var targetPortName = edges[i].input.portName;

            var targetPortNames = new List<string>();
            for (int j=0; j<inputNode.inputContainer.childCount; ++j)
            {
                targetPortNames.Add(inputNode.inputContainer[j].Q<Port>().portName);
            }

            var targetPortIndex = targetPortNames.FindIndex(x => x == targetPortName);

            var connectionName = edges[i].Q<TextField>().value;
            
            frpGraphContainer.NodeLinks.Add(new NodeLinkData
            {
                ConnectionName = connectionName,
                BaseNodeGuid = outputNode.frpNodeData.Guid,
                BasePortName = basePortName,
                BasePortNum = basePortIndex,
                TargetNodeGuid = inputNode.frpNodeData.Guid,
                TargetPortName = targetPortName,
                TargetPortNum = targetPortIndex
            });
        }

        foreach (var node in Nodes)
        {
            node.frpNodeData.Position = node.GetPosition().position;
            frpGraphContainer.FrpNodeData.Add(node.frpNodeData.Clone());
        }

        return true;
    }

    private Edge LinkNodes(Port output, Port input)
    {
        var tempEdge = new Edge
        {
            output = output,
            input = input
        };
        
        tempEdge?.input.Connect(tempEdge);
        tempEdge?.output.Connect(tempEdge);
        _targetGraphView.Add(tempEdge);
        return tempEdge;
    }

    private void CreateNodes()
    {
        foreach (var nodeData in _containerCache.FrpNodeData)
        {
            var tempNode = NodeFactory.CreateNode(nodeData);
            _targetGraphView.AddElement(tempNode);
        }
    }

    private void ClearGraph()
    {
        foreach (var edge in Edges)
        {
            _targetGraphView.RemoveElement(edge);
        }
        
        foreach (var node in Nodes)
        {
            _targetGraphView.RemoveElement(node);
        }
    }

    private void GenerateCommentBlocks()
    {
        foreach (var commentBlock in CommentBlocks)
        {
            _targetGraphView.RemoveElement(commentBlock);
        }

        foreach (var commentBlockData in _containerCache.CommentBlockData)
        {
            var block = _targetGraphView.CreateCommentBlock(
                new Rect(commentBlockData.Position, _targetGraphView.DefaultCommentBlockSize), commentBlockData);
            block.AddElements(Nodes.Where(x => commentBlockData.ChildNodes.Contains(x.frpNodeData.Guid)));
        }
    }
}
