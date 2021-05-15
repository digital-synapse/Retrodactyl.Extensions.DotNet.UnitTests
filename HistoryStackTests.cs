using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace Retrodactyl.Extensions.DotNet.UnitTests
{
    public class HistoryStackTests
    {
        public HistoryStackTests()
        {
            primitiveStack = new HistoryStack<int>(3);
            structStack = new HistoryStack<TestStruct>(3);
            objectStack = new HistoryStack<TestClass>(3);
        }
        private HistoryStack<int> primitiveStack;
        private HistoryStack<TestStruct> structStack;
        private HistoryStack<TestClass> objectStack;

        [Fact]
        public void ShouldAllowPushPopPrimitives()
        {
            primitiveStack.Push(1);
            primitiveStack.Push(2);
            primitiveStack.Push(3);
            primitiveStack.Pop().Should().Be(3);
            primitiveStack.Pop().Should().Be(2);
            primitiveStack.Pop().Should().Be(1);
            primitiveStack.Pop().Should().Be(default(int));
            primitiveStack.Pop().Should().Be(default(int));
            primitiveStack.Pop().Should().Be(default(int));
        }

        [Fact]
        public void ShouldAllowPushPopStructs()
        {
            var a = new TestStruct() { x = 1, y = "a" };
            var b = new TestStruct() { x = 2, y = "b" };
            var c = new TestStruct() { x = 3, y = "c" };

            structStack.Push(a);
            structStack.Push(b);
            structStack.Push(c);
            structStack.Pop().Should().Be(c);
            structStack.Pop().Should().Be(b);
            structStack.Pop().Should().Be(a);
            structStack.Pop().Should().Be(default(TestStruct));
            structStack.Pop().Should().Be(default(TestStruct));
            structStack.Pop().Should().Be(default(TestStruct));
        }

        [Fact]
        public void ShouldAllowPushPopObjects()
        {
            var a = new TestClass() { x = 1, y = "a" };
            var b = new TestClass() { x = 2, y = "b" };
            var c = new TestClass() { x = 3, y = "c" };

            objectStack.Push(a);
            objectStack.Push(b);
            objectStack.Push(c);
            objectStack.Pop().Should().Be(c).And.BeSameAs(c);
            objectStack.Pop().Should().Be(b).And.BeSameAs(b);
            objectStack.Pop().Should().Be(a).And.BeSameAs(a);
            objectStack.Pop().Should().Be(default(TestClass)).And.BeNull();
            objectStack.Pop().Should().Be(default(TestClass)).And.BeNull();
            objectStack.Pop().Should().Be(default(TestClass)).And.BeNull();
        }

        [Fact]
        public void ShouldBehaveLikeRingBuffer()
        {
            // stack declared with a capacity / buffer size of 3            
            // pushing more than 3 should be allowed 
            primitiveStack.Push(1);
            primitiveStack.Push(2);
            primitiveStack.Push(3);
            primitiveStack.Push(4);
            primitiveStack.Push(5);

            // pop returns the last three
            primitiveStack.Pop().Should().Be(5);
            primitiveStack.Pop().Should().Be(4);
            primitiveStack.Pop().Should().Be(3);

            // end of buffer
            primitiveStack.Pop().Should().Be(default(int));
            primitiveStack.Pop().Should().Be(default(int));
            primitiveStack.Pop().Should().Be(default(int));
            primitiveStack.Pop().Should().Be(default(int));
            primitiveStack.Pop().Should().Be(default(int));
        }

        [Fact]
        public void ShouldAllowPeek()
        {
            // stack declared with a capacity / buffer size of 3            
            // pushing more than 3 should be allowed 
            primitiveStack.Push(1);
            primitiveStack.Push(2);
            primitiveStack.Push(3);
            primitiveStack.Push(4);
            primitiveStack.Push(5);

            // peek
            primitiveStack.Peek().Should().Be(5);
            primitiveStack.Peek().Should().Be(5);
            primitiveStack.Peek().Should().Be(5);

            // pop - still 5
            primitiveStack.Pop().Should().Be(5);

            // peek more
            primitiveStack.Peek(2).Should().HaveCount(2);
            primitiveStack.Peek(2).First().Should().Be(4);
            primitiveStack.Peek(2).Skip(1).Take(1).First().Should().Be(3);

            // peek beyond end of buffer
            primitiveStack.Peek(100).Should().HaveCount(100);
            primitiveStack.Peek(100).First().Should().Be(4);
            primitiveStack.Peek(100).Skip(1).Take(1).First().Should().Be(3);
        }

        public struct TestStruct
        {
            public int x;
            public string y;
        }

        public class TestClass
        {
            public int x;
            public string y;
        }
    }
}
