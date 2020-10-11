using NewFrpGraph.Operators;
using NUnit.Framework;

namespace Tests
{
    public class OperatorInfoTest
    {
        [Test]
        public void EqualityTest1()
        {
            var infos = typeof(OperatorInfoObjects).GetFields();
            foreach (var fieldInfo in infos)
            {
                var operatorInfo = (OperatorInfo)fieldInfo.GetValue(null);
                Assert.AreEqual(operatorInfo.OperatorName.GetType(), typeof(string));
            }
        }
    }
}
