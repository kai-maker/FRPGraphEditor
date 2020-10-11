using System.Collections.Generic;
using System.Linq;

namespace FRPGraph.Runtime
{
    public class EventType
    {
        private LinkedList<EventTypePrimitive> _list;
        private string _baseType;
        public string BaseType => _baseType;
        public EventTypePrimitive? OuterType => _list.Count > 0 ? (EventTypePrimitive?) _list.First.Value : null;

        public EventType(string baseType, params EventTypePrimitive[] eventTypes)
        {
            _baseType = baseType;
            _list = eventTypes.Length == 0
                ? new LinkedList<EventTypePrimitive>()
                : new LinkedList<EventTypePrimitive>(eventTypes);
        }

        public override bool Equals(object obj)
        {
            var lhs = this;
            var rhs = obj as EventType;
            if (rhs == null) return false;
            var isListEqual = lhs._list.SequenceEqual(rhs._list);
            var isBaseTypeEqual = lhs._baseType == rhs._baseType;
            return isListEqual && isBaseTypeEqual;
        }

        public EventType Clone()
        {
            return new EventType(_baseType, _list.ToArray());
        }

        public EventType Wrap(EventTypePrimitive primitive)
        {
            var returnVal = Clone();
            returnVal._list.AddLast(primitive);
            return returnVal;
        }

        public EventType Unwrap()
        {
            var returnVal = Clone();
            returnVal._list.RemoveLast();
            return returnVal;
        }
        
        public EventType Unwrap(EventTypePrimitive eventTypePrimitive)
        {
            if (OuterType != eventTypePrimitive)
                return null;
            var returnVal = Clone();
            returnVal._list.RemoveLast();
            return returnVal;
        }
    }

    public enum EventTypePrimitive
    {
        Event, Behaviour
    }
}
