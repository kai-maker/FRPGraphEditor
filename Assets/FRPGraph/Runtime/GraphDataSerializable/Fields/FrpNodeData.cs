using System;
using NewFrpGraph;
using UnityEngine;

namespace FRPGraph.Editor.Nodes
{
    [Serializable]
    public class FrpNodeData
    {
        [SerializeField]
        public string Guid;
        public string CodeText;
        public Vector2 Position;
        public OperatorType OperatorType;


        public FrpNodeData Clone()
        {
            return new FrpNodeData
            {
                Guid = string.Copy(Guid),
                CodeText = string.Copy(CodeText),
                Position = Position,
                OperatorType = OperatorType
            };
        }
    }
}
