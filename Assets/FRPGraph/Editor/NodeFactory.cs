using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FRPGraph.Editor.Nodes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace FRPGraph.Editor
{
    public static class NodeFactory
    {
        private static readonly Vector2 defaultNodeSize = new Vector2(100, 150);
        
        public static FrpNode CreateNode(FrpNodeData frpNodeData)
        {
            var inputPortList = new List<(Type portType, string portName)>();
            foreach (var argumentName in frpNodeData.OperatorInfo.OperatorArgumentNames)
            {
                inputPortList.Add((null, argumentName));
            }

            var node = GeneratePorts(
                inputPortList, 
                new List<(Type portType, string portName)>{(null, frpNodeData.OperatorInfo.OperatorReturnName)},
                frpNodeData);
            return node;
        }

        public static FrpNode GeneratePorts(IEnumerable<(Type portType, string portName)> inputPorts,
            IEnumerable<(Type portType, string portName)> outputPorts, FrpNodeData frpNodeData)
        {
            var node = new FrpNode(frpNodeData);
            
            foreach (var (inputPort, index) in inputPorts.Select((value, index) => (value, index)))
            {
                var port = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, inputPort.portType);
                var addTextFieldToEdge = (Action<Port>) (p =>
                {
                    var text = new TextField("") {value = "New Connection"};
                    p.connections
                        .Select(edge => edge.edgeControl)
                        .Where(control => control.Q<TextField>() == null)
                        .ToList()
                        .ForEach(control => control.Add(text));
                });
                var fieldInfo = typeof(Port).GetField("OnConnect", BindingFlags.NonPublic | BindingFlags.Instance);
                var defaultOnConnect = (Action<Port>)fieldInfo.GetValue(port);
                if (defaultOnConnect == null)
                {
                    fieldInfo.SetValue(port, addTextFieldToEdge);
                }
                else
                {
                    defaultOnConnect += addTextFieldToEdge;
                }
                port.portName = inputPort.portName;
                node.inputContainer.Add(port);
            }
            
            foreach (var (outputPort, index) in outputPorts.Select((value, index) => (value, index)))
            {
                var port = node.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, outputPort.portType);
                port.portName = outputPort.portName;
                node.outputContainer.Add(port);
            }
            
            var textField = new TextField(string.Empty);
            textField.RegisterValueChangedCallback(evt =>
            {
                node.frpNodeData.CodeText = evt.newValue;
            });
            textField.multiline = true;
            textField.SetValueWithoutNotify(frpNodeData.CodeText);
            node.mainContainer.Add(textField);

            node.RefreshExpandedState();
            node.RefreshPorts();
            
            node.SetPosition(new Rect(frpNodeData.Position, defaultNodeSize));

            node.title = frpNodeData.OperatorInfo.OperatorName;
            node.AddToClassList(frpNodeData.OperatorInfo.OperatorName.ToLower());

            return node;
        }
    }
}
