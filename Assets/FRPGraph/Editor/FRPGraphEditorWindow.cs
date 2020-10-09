using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class FRPGraphEditorWindow : EditorWindow
{
    private FRPGraphView _graphView;
    private string _fileName = "New Narrative";
    
    [MenuItem("Graph/FRP Graph")]
    public static void OpenFRPGraphEditorWindow()
    {
        var window = GetWindow<FRPGraphEditorWindow>();
        window.titleContent = new GUIContent("FRP Graph");
    }

    public void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
        GenerateMiniMap();
        GenerateBlackBoard();
    }

    private void GenerateBlackBoard()
    {
        var blackboard = new Blackboard(_graphView);
        blackboard.Add(new BlackboardSection{title = "Exposed Properties"});
        blackboard.addItemRequested = _blackbord => { _graphView.AddPropertyToBlackBoard(new ExposedProperty());};
        blackboard.editTextRequested = (blackboard1, element, newValue) =>
        {
            var oldPropertyName = ((BlackboardField) element).text;
            if (_graphView.ExposedProperties.Any(x => x.PropertyName == newValue))
            {
                EditorUtility.DisplayDialog("Error", "This property name is already exists, please chose another one!",
                    "OK");
                return;
            }

            var propertyIndex = _graphView.ExposedProperties.FindIndex(x => x.PropertyName == oldPropertyName);
            _graphView.ExposedProperties[propertyIndex].PropertyName = newValue;
            ((BlackboardField) element).text = newValue;
        };
        
        blackboard.SetPosition(new Rect(10,30, 200, 300));
        _graphView.Add(blackboard);
        _graphView.Blackboard = blackboard;
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }

    private void ConstructGraphView()
    {
        _graphView = new FRPGraphView(this)
        {
            name = "FRP Graph"
        };
        
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void GenerateToolbar()
    {
        var toolbar = new Toolbar();
        
        var fileNameTextField = new TextField("File name:");
        fileNameTextField.SetValueWithoutNotify(_fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
        toolbar.Add(fileNameTextField);
        
        toolbar.Add(new Button(() => RequestDataOperation(true)){text = "Save Data"});
        toolbar.Add(new Button(() => RequestDataOperation(false)){text = "Load Data"});
        
        toolbar.styleSheets.Add(Resources.Load<StyleSheet>("ToolBar"));

        
        rootVisualElement.Add(toolbar);
    }

    private void GenerateMiniMap()
    {
        var miniMap = new MiniMap
        {
            anchored = true
        };
        var cords = _graphView.contentViewContainer.WorldToLocal(new Vector2(maxSize.x - 10, 30));
        miniMap.SetPosition(new Rect(cords.x, cords.y, 200, 140));
        _graphView.Add(miniMap);
    }

    private void RequestDataOperation(bool save)
    {
        if (string.IsNullOrEmpty(_fileName))
        {
            EditorUtility.DisplayDialog("Invalid file name!", "Please enter a vaild file name.", "OK");
        }

        var saveUtility = GraphSaveUtility.GetInstance(_graphView);
        if(save)
            saveUtility.SaveGraph(_fileName);
        else
            saveUtility.LoadGraph(_fileName);
    }
}
