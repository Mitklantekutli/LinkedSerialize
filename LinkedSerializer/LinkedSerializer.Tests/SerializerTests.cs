using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinkedSerializer.Tests
{
    [TestClass]
    public class SerializerTests
    {
        //FS must be mocked, but there were no testing tasks - so its for debug only
        [TestMethod]
        public void TestMethod1()
        {
            var path = "file.txt";
            var list = new ListRand();

            var node1 = new ListNode() { Data = "Elem1" };
            var node2 = new ListNode() { Data = "Elem2", Rand = node1};
            var node3 = new ListNode() { Data = "Elem3" };
            list.Add(node1);
            list.Add(node2);
            list.Add(node3);

            using (var fs = new FileStream(path, FileMode.Create))
            {
                list.Serialize(fs);
            }

            using (var fs = new FileStream(path, FileMode.Open))
            {
                fs.Position = 0;
                var l = new ListRand();
                l.Deserialize(fs);

                Assert.IsTrue(l.Count==list.Count);
                Assert.IsTrue(l.Head.Data == list.Head.Data);
                Assert.IsTrue(l.Tail.Data==list.Tail.Data);
                Assert.IsTrue(l.Head.Next.Data==list.Head.Next.Data);
                Assert.IsTrue(l.Head.Next.Rand.Data==list.Head.Next.Rand.Data);
            }
        }
    }
}
