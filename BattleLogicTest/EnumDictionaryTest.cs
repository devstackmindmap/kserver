using NUnit.Framework;
using System.Collections.Generic;
using NUnit.Framework.Internal;

namespace BattleLogicTest
{

    enum TestType
    {
        Aka,
        AkaBlue,
        KnightRun
    }

    public class TestTypeComparer : IEqualityComparer<TestType>
    {
        public static TestTypeComparer Comparer = new TestTypeComparer();
        bool IEqualityComparer<TestType>.Equals(TestType x, TestType y)
        {
            return x == y;
        }

        int IEqualityComparer<TestType>.GetHashCode(TestType obj)
        {
            return ((int)obj).GetHashCode();
        }
    }

    [TestFixture]
    public class EnumDictionaryTest
    {
        Dictionary<TestType, int> enumDictionary = new Dictionary<TestType, int>(TestTypeComparer.Comparer);
        Dictionary<TestType, int> enumDictionary2 = new Dictionary<TestType, int>();
        [Test]
        public void Run()
        {
            enumDictionary.Add(TestType.Aka, 1);
            enumDictionary.Add(TestType.AkaBlue, 2);
            enumDictionary.Add(TestType.KnightRun, 3);

            int value = 0;
            enumDictionary.ContainsKey(TestType.Aka);
            enumDictionary.ContainsKey(TestType.AkaBlue);
            enumDictionary.ContainsKey(TestType.KnightRun);

            value = enumDictionary[TestType.Aka];
            value = enumDictionary[TestType.AkaBlue];
            value = enumDictionary[TestType.KnightRun];

            enumDictionary.TryGetValue(TestType.Aka, out value);
            enumDictionary.TryGetValue(TestType.AkaBlue, out value);
            enumDictionary.TryGetValue(TestType.KnightRun, out value);
        }


        [Test]
        public void Run2()
        {
            enumDictionary2.Add(TestType.Aka, 1);
            enumDictionary2.Add(TestType.AkaBlue, 2);
            enumDictionary2.Add(TestType.KnightRun, 3);

            int value = 0;
            enumDictionary2.ContainsKey(TestType.Aka);
            enumDictionary2.ContainsKey(TestType.AkaBlue);
            enumDictionary2.ContainsKey(TestType.KnightRun);

            value = enumDictionary2[TestType.Aka];
            value = enumDictionary2[TestType.AkaBlue];
            value = enumDictionary2[TestType.KnightRun];

            enumDictionary2.TryGetValue(TestType.Aka, out value);
            enumDictionary2.TryGetValue(TestType.AkaBlue, out value);
            enumDictionary2.TryGetValue(TestType.KnightRun, out value);
        }
    }

}
