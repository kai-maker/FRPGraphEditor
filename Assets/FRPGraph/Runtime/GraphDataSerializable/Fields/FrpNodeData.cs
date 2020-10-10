using System;
using NewFrpGraph;
using UnityEngine;
using UnityEngine.Serialization;

namespace FRPGraph.Editor.Nodes
{
    [Serializable]
    public class FrpNodeData
    {
        [SerializeField]
        public string Guid;
        public string CodeText;
        public Vector2 Position;
        [FormerlySerializedAs("OperatorType")] public SerializedOperatorType serializedOperatorType;


        public FrpNodeData Clone()
        {
            return new FrpNodeData
            {
                Guid = string.Copy(Guid),
                CodeText = string.Copy(CodeText),
                Position = Position,
                serializedOperatorType = serializedOperatorType
            };
        }
    }
}
