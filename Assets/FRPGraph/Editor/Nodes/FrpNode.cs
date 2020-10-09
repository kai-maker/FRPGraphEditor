using FRPGraph.Editor.Nodes;
using UnityEditor.Experimental.GraphView;

public class FrpNode : Node
{
    public FrpNode(FrpNodeData _frpNodeData)
    {
        frpNodeData = _frpNodeData;
    }
    public FrpNodeData frpNodeData;
}
