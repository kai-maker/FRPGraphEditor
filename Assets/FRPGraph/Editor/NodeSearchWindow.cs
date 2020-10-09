using System;
using System.Collections.Generic;
using FRPGraph.Editor;
using FRPGraph.Editor.Nodes;
using NewFrpGraph;
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
        var tree = new List<SearchTreeEntry>
        {
            new SearchTreeGroupEntry(new GUIContent("Create Elements"), 0),
            new SearchTreeGroupEntry(new GUIContent("Node"), 1),
            new SearchTreeEntry(new GUIContent("Map Node", _indentationIcon))
            {
                userData = OperatorType.Map, level = 2
            },
            new SearchTreeEntry(new GUIContent("Lift Node", _indentationIcon))
            {
                userData = OperatorType.Lift, level = 2
            },
            new SearchTreeEntry(new GUIContent("SnapShot Node", _indentationIcon))
            {
                userData = OperatorType.Snapshot, level = 2
            },
            new SearchTreeEntry(new GUIContent("Merge Node", _indentationIcon))
            {
                userData = OperatorType.Merge, level = 2
            },
            new SearchTreeEntry(new GUIContent("Filter Node", _indentationIcon))
            {
                userData = OperatorType.Filter, level = 2
            },
            new SearchTreeEntry(new GUIContent("SwitchS Node", _indentationIcon))
            {
                userData = OperatorType.SwitchS, level = 2
            },
            new SearchTreeEntry(new GUIContent("SwitchC Node", _indentationIcon))
            {
                userData = OperatorType.SwitchC, level = 2
            },
            new SearchTreeEntry(new GUIContent("Cell_Stream", _indentationIcon))
            {
                userData = OperatorType.Cell_Stream, level = 2
            },
            new SearchTreeEntry(new GUIContent("Cell_Cell", _indentationIcon))
            {
                userData = OperatorType.Cell_Cell, level = 2
            },
            new SearchTreeEntry(new GUIContent("Comment Block", _indentationIcon))
            {
                level = 1,
                userData = new Group()
            }
        };
        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        var worldMousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent,
            context.screenMousePosition - _window.position.position);
        var localMousePosition = _graphView.contentViewContainer.WorldToLocal(worldMousePosition);
        
        switch (SearchTreeEntry.userData)
        {
            case Group group:
                var rect = new Rect(localMousePosition, _graphView.DefaultCommentBlockSize);
                _graphView.CreateCommentBlock(rect);
                return true;
            case OperatorType type:
                _graphView.AddElement(NodeFactory.CreateNode(new FrpNodeData
                {
                    CodeText = "hogehoge",
                    OperatorType = type,
                    Position = localMousePosition,
                    Guid = Guid.NewGuid().ToString()
                }));
                return true;
        }

        return false;
    }
}
