using System;
using System.Collections.Generic;
using System.Linq;

namespace NewFrpGraph.Operators
{
    [Serializable]
    public class OperatorInfo
    {
        public string OperatorName;
        public string OperatorReturnName;
        public List<string> OperatorArgumentNames;

        public OperatorInfo(string operatorName, string operatorReturnName, params string[] operatorArgumentNames)
        {
            OperatorName = operatorName;
            OperatorReturnName = operatorReturnName;
            OperatorArgumentNames = operatorArgumentNames.ToList();
        }
    }

    public static class OperatorInfoObjects
    {
        public static OperatorInfo OneE = new OperatorInfo("OneE", "Event");
        public static OperatorInfo MapE = new OperatorInfo("MapE", "Event", "Event");
        public static OperatorInfo MergeE = new OperatorInfo("MergeE", "Event", "Event1", "Event2");
        public static OperatorInfo SwitchE = new OperatorInfo("SwitchE", "Event", "Behavior<Event>");
        public static OperatorInfo FilterE = new OperatorInfo("FilterE", "Event", "Event");
        public static OperatorInfo ConstantB = new OperatorInfo("ConstantB", "Behavior");
        public static OperatorInfo LiftB = new OperatorInfo("LiftB", "Behavior", "Behavior");
        public static OperatorInfo Lift2B = new OperatorInfo("Lift2B", "Behavior", "Behavior1", "Behavior2");
        public static OperatorInfo SwitchB = new OperatorInfo("SwitchB", "Behavior", "Behavior<Behavior>");
        public static OperatorInfo EndE = new OperatorInfo("EndE", "null", "Event");
        public static OperatorInfo EndB = new OperatorInfo("EndB" ,"null", "Behavior");
        public static OperatorInfo DomInputB = new OperatorInfo("DomInputB", "Behavior");
        public static OperatorInfo DomOutputB = new OperatorInfo("DomOutputB", "null", "Behavior");
        public static OperatorInfo DomEnableB = new OperatorInfo("DomEnableB", "null", "Behavior");
        public static OperatorInfo DebugB = new OperatorInfo("DebugB", "null", "Behavior");
    }
}
