using System;
using NewFrpGraph;
using NewFrpGraph.Operators;
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
        public OperatorInfo OperatorInfo;


        public FrpNodeData Clone()
        {
            return new FrpNodeData
            {
                Guid = string.Copy(Guid),
                CodeText = string.Copy(CodeText),
                Position = Position,
                OperatorInfo = OperatorInfo
            };
        }
    }
}
