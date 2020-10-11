using System;
using System.Collections.Generic;
using System.Linq;

namespace NewFrpGraph.Operators
{
    [Serializable]
    public class OperatorInfo
    {
        public readonly string OperatorName;
        public readonly string OperatorReturnName;
        public readonly List<string> OperatorArgumentNames;

        public OperatorInfo(string operatorName, string operatorReturnName, params string[] operatorArgumentNames)
        {
            OperatorName = operatorName;
            OperatorReturnName = operatorReturnName;
            OperatorArgumentNames = operatorArgumentNames.ToList();
        }
    }

    public static class OperatorInfoObjects
    {
        public static OperatorInfo MapE = new OperatorInfo("MapE", "Event", "Event");
        public static OperatorInfo MergeE = new OperatorInfo("MergeE", "Event", "Event1", "Event2");
        public static OperatorInfo SwitchE = new OperatorInfo("SwitchE", "Event", "Behavior<Event>");
        public static OperatorInfo FilterE = new OperatorInfo("FilterE", "Event", "Event");
        public static OperatorInfo SwitchB = new OperatorInfo("SwitchB", "Behavior", "Behavior<Behavior>");
    }
}
