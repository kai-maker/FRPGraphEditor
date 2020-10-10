using System;
using System.Collections.Generic;
using System.Linq;
using Boo.Lang.Runtime;
using FRPGraph.Editor.Nodes;
using NewFrpGraph;
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
            switch (frpNodeData.serializedOperatorType)
            {
                case SerializedOperatorType.Map:
                {
                    var node = GeneratePorts(
                        new List<(Type portType, string portName)>{(typeof(Stream), portName: "Sin")}, 
                        new List<(Type portType, string portName)>{(typeof(Stream), "Sout")},
                        frpNodeData);
                    return node;
                }
                case SerializedOperatorType.Snapshot:
                {
                    var node = GeneratePorts(
                        new List<(Type portType, string portName)>
                        {
                            (typeof(Stream), portName: "Sin"),
                            (typeof(Cell), portName: "Cin")
                        }, 
                        new List<(Type portType, string portName)>{(typeof(Stream), "Sout")},
                        frpNodeData);
                    return node;
                }
                case SerializedOperatorType.Merge:
                {
                    var node = GeneratePorts(
                        new List<(Type portType, string portName)>
                        {
                            (typeof(Stream), portName: "Sin0"),
                            (typeof(Stream), portName: "Sin1")
                        }, 
                        new List<(Type portType, string portName)>{(typeof(Stream), "Sout")},
                        frpNodeData);
                    return node;
                }
                case SerializedOperatorType.Lift:
                {
                    var node = GeneratePorts(
                        new List<(Type portType, string portName)>
                        {
                            (typeof(Cell), portName: "Cin0"),
                            (typeof(Cell), portName: "Cin1")
                        }, 
                        new List<(Type portType, string portName)>{(typeof(Cell), "Cout")},
                        frpNodeData);
                    return node;
                }
                case SerializedOperatorType.Filter:
                {
                    var node = GeneratePorts(
                        new List<(Type portType, string portName)>
                        {
                            (typeof(Stream), portName: "Sin")
                        }, 
                        new List<(Type portType, string portName)>{(typeof(Stream), "Sout")},
                        frpNodeData);
                    return node;
                }
                case SerializedOperatorType.SwitchS:
                    return GeneratePorts(
                        new List<(Type portType, string portName)>{(typeof(Cell<Stream>), portName: "Cell<Stream>0")}, 
                        new List<(Type portType, string portName)>{(typeof(Stream), "Stream")},
                        frpNodeData);
                
                case SerializedOperatorType.SwitchC:
                    return GeneratePorts(
                        new List<(Type portType, string portName)>{(typeof(Cell<Cell>), portName: "Cell<Cell>0")}, 
                        new List<(Type portType, string portName)>{(typeof(Stream), "Stream")},
                        frpNodeData);
                
                case SerializedOperatorType.Cell_Stream:
                    return GeneratePorts(
                        new List<(Type portType, string portName)>(), 
                        new List<(Type portType, string portName)>{(typeof(Cell<Stream>), "Cell_Stream")},
                        frpNodeData);
                
                case SerializedOperatorType.Cell_Cell:
                    return GeneratePorts(
                        new List<(Type portType, string portName)>(), 
                        new List<(Type portType, string portName)>{(typeof(Cell<Cell>), "Cell_Cell")},
                        frpNodeData);
                
                default: throw new RuntimeException($"No definition found for the operator {frpNodeData.serializedOperatorType}");
            }
        }

        public static FrpNode GeneratePorts(IEnumerable<(Type portType, string portName)> inputPorts,
            IEnumerable<(Type portType, string portName)> outputPorts, FrpNodeData frpNodeData)
        {
            var node = new FrpNode(frpNodeData);
            
            foreach (var (inputPort, index) in inputPorts.Select((value, index) => (value, index)))
            {
                var port = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, inputPort.portType);
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

            node.title = frpNodeData.serializedOperatorType.ToString();
            node.AddToClassList(frpNodeData.serializedOperatorType.ToString().ToLower());

            return node;
        }
    }
}
