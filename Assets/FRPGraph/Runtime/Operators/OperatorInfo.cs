using System;
using System.Collections.Generic;
using System.Linq;
using Mono.CSharp;

namespace FRPGraph.Runtime
{
    [Serializable]
    public class OperatorInfo
    {
        public string OperatorName;
        public string OperatorReturnName;
        public List<string> OperatorArgumentNames;
        public EventType OperatorReturnType;

        public OperatorInfo(string operatorName, string operatorReturnName, EventType operatorReturnType, params string[] operatorArgumentNames)
        {
            OperatorName = operatorName;
            OperatorReturnName = operatorReturnName;
            OperatorReturnType = operatorReturnType;
            OperatorArgumentNames = operatorArgumentNames.ToList();
        }
    }
    
    public enum EventType {Event, Behavior}

    public static class OperatorInfoObjects
    {
        public static OperatorInfo OneE = new OperatorInfo("OneE", "out: E<T>", EventType.Event);
        public static OperatorInfo MapE = new OperatorInfo("MapE", "out: E<T>", EventType.Event, "a: E<T>");
        public static OperatorInfo MergeE = new OperatorInfo("MergeE", "out: E<T>", EventType.Event,"a: E<T>", "b: E<T>");
        public static OperatorInfo SwitchE = new OperatorInfo("SwitchE", "out: E<T>", EventType.Event,"a B<E>");
        public static OperatorInfo SnapshotE = new OperatorInfo("SnapshotE", "out: E<T>", EventType.Event,"a: E<T>", "b: B<T>");
        public static OperatorInfo FilterE = new OperatorInfo("FilterE", "out: E<T>", EventType.Event,"a: E<T>");
        public static OperatorInfo Hold = new OperatorInfo("Hold", "out: B<T>", EventType.Behavior,"a: E<T>");
        public static OperatorInfo ConstantB = new OperatorInfo("ConstantB", "out: B<T>", EventType.Behavior);
        public static OperatorInfo LiftB = new OperatorInfo("LiftB", "out: B<T>", EventType.Behavior,"a: B<T>");
        public static OperatorInfo Lift2B = new OperatorInfo("Lift2B", "out: B<T>", EventType.Behavior,"a: B<T>", "b: B<T>");
        public static OperatorInfo SwitchB = new OperatorInfo("SwitchB", "out: B<T>", EventType.Behavior,"B<B>");
        public static OperatorInfo EndE = new OperatorInfo("EndE", "None", EventType.Event,"E<T>");
        public static OperatorInfo EndB = new OperatorInfo("EndB" ,"None", EventType.Behavior,"B<T>");
        public static OperatorInfo InputB = new OperatorInfo("InputB", "out: B<T>", EventType.Behavior);
        public static OperatorInfo DomInputB = new OperatorInfo("DomInputB", "out: B<T>", EventType.Behavior);
        public static OperatorInfo DomInputE = new OperatorInfo("DomInputE", "out: E<T>", EventType.Event);
        public static OperatorInfo DomOutputB = new OperatorInfo("DomOutputB", "None", EventType.Behavior,"B<T>");
        public static OperatorInfo DomOutputE = new OperatorInfo("DomOutputE", "None", EventType.Behavior,"E<T>");
        public static OperatorInfo DomEnableB = new OperatorInfo("DomEnableB", "None", EventType.Behavior,"B<T>");
        public static OperatorInfo DebugB = new OperatorInfo("DebugB", "None", EventType.Behavior,"B<T>");
        public static OperatorInfo LabelB = new OperatorInfo("LabelB", "None", EventType.Behavior,"text: B<String>");
        public static OperatorInfo FieldB = new OperatorInfo("FieldB", "text: B<String>", EventType.Behavior);
        public static OperatorInfo DisableableFieldB = new OperatorInfo("DisableableFieldB", "text: B<String>", EventType.Behavior,"enable: B<bool>");
        public static OperatorInfo ButtonE = new OperatorInfo("ButtonE", "clicked: E<Unit>", EventType.Event);
    }
}
