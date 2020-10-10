using FRPGraph.Runtime;
using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;

namespace Tests
{
    public class EventTypeTest
    {
        private const EventTypePrimitive _Behaviour = EventTypePrimitive.Behaviour;
        private const EventTypePrimitive _Event = EventTypePrimitive.Event;
        
        [Test]
        public void EqualityTest1()
        {
            Assert.AreEqual(
                new EventType("T"), 
                new EventType("T"));
            Assert.AreNotEqual(
                new EventType("T"), 
                new EventType("U"));
        }
        
        [Test]
        public void EqualityTest2()
        {
            Assert.AreEqual(
                new EventType("T", _Behaviour), 
                new EventType("T", _Behaviour));
            Assert.AreNotEqual(
                new EventType("T", _Behaviour), 
                new EventType("T", _Event));
        }

        [Test]
        public void EqualityTest3()
        {
            Assert.AreEqual(
                new EventType("T", _Behaviour, _Behaviour, _Event), 
                new EventType("T", _Behaviour, _Behaviour, _Event));
            Assert.AreNotEqual(
                new EventType("T", _Behaviour, _Behaviour, _Event), 
                new EventType("T", _Behaviour, _Behaviour));
            Assert.AreNotEqual(
                new EventType("T", _Behaviour, _Behaviour), 
                new EventType("T", _Behaviour, _Behaviour, _Event));
        }

        [Test]
        public void CloneTest()
        {
            var original = new EventType("T", _Behaviour, _Behaviour, _Event);
            var clone = original.Clone();
            Assert.IsFalse(ReferenceEquals(original, clone));
            Assert.AreEqual(original, clone);
        }
        
        [Test]
        public void WrapTest()
        {
            var lhs = new EventType("T", _Behaviour, _Behaviour, _Event);
            var rhs = new EventType("T", _Behaviour, _Behaviour);
            Assert.AreNotEqual(lhs, rhs);
            var newRhs = rhs.Wrap(_Event);
            Assert.AreEqual(lhs, newRhs);
            Assert.AreNotEqual(lhs, rhs);
            var newLhs = lhs.Unwrap();
            Assert.AreEqual(newLhs, rhs);
            Assert.AreNotEqual(lhs, rhs);
        }
        
        [Test]
        public void OuterTypeValidate()
        {
            var lhs = new EventType("T", _Behaviour, _Behaviour, _Event);
            var rhs = new EventType("T", _Behaviour, _Behaviour);
            Assert.AreEqual(lhs.OuterType, _Behaviour);
            Assert.AreEqual(rhs.OuterType, _Behaviour);
            Assert.AreEqual(new EventType("U").OuterType, null);
        }
    }
}
