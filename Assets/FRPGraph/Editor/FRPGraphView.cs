using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class FRPGraphView : GraphView
{
    public Vector2 defaultNodeSize = new Vector2(100, 150);
    public Vector2 DefaultCommentBlockSize = new Vector2(300, 200);

    public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();
    public Blackboard Blackboard;
    private NodeSearchWindow _searchWindow;

    public FRPGraphView(EditorWindow editorWindow)
    {
        styleSheets.Add(Resources.Load<StyleSheet>("FRPGraph"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        styleSheets.Add(Resources.Load<StyleSheet>("FrpNode"));
        
        AddSearchWindow(editorWindow);
    }

    private void AddSearchWindow(EditorWindow editorWindow)
    {
        _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
        _searchWindow.Init(editorWindow, this);
        nodeCreationRequest = context =>
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        foreach (var port in ports.ToList())
        {
            if(startPort.node == port.node ||
               startPort.direction == port.direction ||
               startPort.portType != port.portType)
                continue;
            compatiblePorts.Add(port);
        }
        return compatiblePorts;
    }

    public void ClearBlackBoardAndExposedProperties()
    {
        ExposedProperties.Clear();
        Blackboard.Clear();
    }

    public Group CreateCommentBlock(Rect rect, CommentBlockData commentBlockData = null)
    {
        if (commentBlockData == null)
            commentBlockData = new CommentBlockData();
        var group = new Group
        {
            autoUpdateGeometry = true,
            title = commentBlockData.Title
        };
        AddElement(group);
        group.SetPosition(rect);
        return group;
    }

    public void AddPropertyToBlackBoard(ExposedProperty exposedProperty)
    {
        var localPropertyName = exposedProperty.PropertyName;
        var localPropertyValue = exposedProperty.PropertyValue;
        while (ExposedProperties.Any(x => x.PropertyName == localPropertyName))
        {
            localPropertyName = $"{localPropertyName}(1)";
        }
        
        var property = new ExposedProperty();
        property.PropertyName = localPropertyName;
        property.PropertyValue = localPropertyValue;
        ExposedProperties.Add(property);
        
        var container = new VisualElement();
        var blackboardField = new BlackboardField{ text = property.PropertyName, typeText = "string property"};
        container.Add(blackboardField);

        var propertyValueTextField = new TextField("Value:")
        {
            value = property.PropertyValue
        };
        propertyValueTextField.RegisterValueChangedCallback(evt =>
        {
            var changingPropertyIndex = ExposedProperties.FindIndex(x => x.PropertyName == property.PropertyName);
            ExposedProperties[changingPropertyIndex].PropertyValue = evt.newValue;
        });
        var blackBoardValueRow = new BlackboardRow(blackboardField, propertyValueTextField);
        container.Add(blackBoardValueRow);
        
        Blackboard.Add(container);
    }
}
