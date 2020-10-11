using System;
using System.Collections.Generic;
using FRPGraph.Editor;
using FRPGraph.Editor.Nodes;
using NewFrpGraph;
using NewFrpGraph.Operators;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private FRPGraphView _graphView;
    private EditorWindow _window;
    private Texture2D _indentationIcon;

    public void Init(EditorWindow window, FRPGraphView graphView)
    {
        _window = window;
        _graphView = graphView;
        
        _indentationIcon = new Texture2D(1, 1);
        _indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
        _indentationIcon.Apply();
    }
    
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var tree = new List<SearchTreeEntry>();
        tree.Add(new SearchTreeGroupEntry(new GUIContent("Create Elements")));
        var infos = typeof(OperatorInfoObjects).GetFields();
        foreach (var fieldInfo in infos)
        {
            var operatorInfo = (OperatorInfo)fieldInfo.GetValue(null);
            var entry = new SearchTreeEntry(new GUIContent(operatorInfo.OperatorName, _indentationIcon))
            {
                level = 1,
                userData = operatorInfo
            };
            tree.Add(entry);
        }
        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        var worldMousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent,
            context.screenMousePosition - _window.position.position);
        var localMousePosition = _graphView.contentViewContainer.WorldToLocal(worldMousePosition);
        
        switch (searchTreeEntry.userData)
        {
            case Group group:
                var rect = new Rect(localMousePosition, _graphView.DefaultCommentBlockSize);
                _graphView.CreateCommentBlock(rect);
                return true;
            case SerializedOperatorType type:
                _graphView.AddElement(NodeFactory.CreateNode(new FrpNodeData
                {
                    CodeText = "hogehoge",
                    // = type,
                    Position = localMousePosition,
                    Guid = Guid.NewGuid().ToString()
                }));
                return true;
            case OperatorInfo info:
                _graphView.AddElement(NodeFactory.CreateNode(new FrpNodeData
                {
                    CodeText = "Write code here",
                    OperatorInfo = info,
                    Position = localMousePosition,
                    Guid = Guid.NewGuid().ToString()
                }));
                return true;
        }

        return false;
    }
}
