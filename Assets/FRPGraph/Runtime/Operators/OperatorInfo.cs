using System;
using System.Collections.Generic;
using System.Linq;
using Mono.CSharp;

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
        public static OperatorInfo OneE = new OperatorInfo("OneE", "out: E<T>");
        public static OperatorInfo MapE = new OperatorInfo("MapE", "out: E<T>", "a: E<T>");
        public static OperatorInfo MergeE = new OperatorInfo("MergeE", "out: E<T>", "a: E<T>", "b: E<T>");
        public static OperatorInfo SwitchE = new OperatorInfo("SwitchE", "out: E<T>", "a B<E>");
        public static OperatorInfo SnapshotE = new OperatorInfo("SnapshotE", "out: E<T>", "a: E<T>", "b: B<T>");
        public static OperatorInfo FilterE = new OperatorInfo("FilterE", "out: E<T>", "a: E<T>");
        public static OperatorInfo Hold = new OperatorInfo("Hold", "out: B<T>", "a: E<T>");
        public static OperatorInfo ConstantB = new OperatorInfo("ConstantB", "out: B<T>");
        public static OperatorInfo LiftB = new OperatorInfo("LiftB", "out: B<T>", "a: B<T>");
        public static OperatorInfo Lift2B = new OperatorInfo("Lift2B", "out: B<T>", "a: B<T>", "a: B<T>");
        public static OperatorInfo SwitchB = new OperatorInfo("SwitchB", "out: B<T>", "B<B>");
        public static OperatorInfo EndE = new OperatorInfo("EndE", "None", "E<T>");
        public static OperatorInfo EndB = new OperatorInfo("EndB" ,"None", "E<T>");
        public static OperatorInfo InputB = new OperatorInfo("InputB", "out: B<T>");
        public static OperatorInfo DomInputB = new OperatorInfo("DomInputB", "out: B<T>");
        public static OperatorInfo DomOutputB = new OperatorInfo("DomOutputB", "None", "B<T>");
        public static OperatorInfo DomEnableB = new OperatorInfo("DomEnableB", "None", "B<T>");
        public static OperatorInfo DebugB = new OperatorInfo("DebugB", "None", "B<T>");
        public static OperatorInfo LabelB = new OperatorInfo("LabelB", "None", "text: B<String>");
        public static OperatorInfo FieldB = new OperatorInfo("FieldB", "text: B<String>");
        public static OperatorInfo DisableableFieldB = new OperatorInfo("DisableableFieldB", "text: B<String>", "enable: B<bool>");
        public static OperatorInfo ButtonE = new OperatorInfo("ButtonE", "clicked: E<Unit>");
    }
}
