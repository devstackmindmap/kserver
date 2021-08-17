using AkaUtility;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UtilityTest
{
    class SampleClass2
    {
        public string name;
        public string comment;
    }
    class SampleClass
    {
        public int a;
        public string b;
        public List<int> c;
        public List<SampleClass2> d;
        public Dictionary<string, SampleClass2> e;
    }

    class CopyObjectTest
    {
        [Test]
        public void CopyFromSkill()
        {
            var sample2 = new SampleClass2 { name = "joy", comment = " blabla" };
            var sample3 = new SampleClass2 { name = "joy2", comment = " blabla" };
            var sampleContainer = new SampleClass
            {
                a = 10,
                b = "sample",
                c = new List<int> { 1, 2, 3, },
                d = new List<SampleClass2> { sample2 },
                e = new Dictionary<string, SampleClass2> { { "test", sample3 } }
            };

            var clone = AkaUtility.Utility.CopyFrom(sampleContainer);
            Assert.That(() =>
            {
                return
                sampleContainer.a == 10
                && sampleContainer.b == "sample"
                && sampleContainer.c.Count == 3
                && sampleContainer.d[0].name == "joy"
                && sampleContainer.e["test"].name == "joy2";
            }, Is.EqualTo(true));
        }

        [Test]
        public void TryGetValueDictionary()
        {
            var dic = new Dictionary<int, int> { { 1, 10 }, { 2, 20 } };
            //var a = dic[3];

            int worker, iocp;
            ThreadPool.GetMaxThreads(out worker, out iocp);


        }

        [Test]
        public void TimerTest()
        {
            var t = new AkaTimer.Timer(100, true, delegate
            {
                System.Console.WriteLine("ThreadId: " + System.Threading.Thread.CurrentThread.ManagedThreadId);
                //     throw new System.Exception("test");
            });

            t.Start();
            Task.Delay(3000).Wait();
            t.Dispose();
            t.Pause();

        }


        [Test]
        public void RandomListToSubListTest()
        {
            int[] sourceList = { 1, 2, 3, 4, 5, 6, 7 };
            int resultCount = 3;
            var result = AkaRandom.Random.ChooseIndexRanddomlyInSourceByCount(sourceList, resultCount).ToList();

            Assert.That(() => result.Count == 3, Is.EqualTo(true));
        }
    }
}
