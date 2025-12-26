using System;
using System.Diagnostics;
using System.Text;
using NUnit.Framework;

namespace StructBenchmarking
{
    public class StringBuilderTask : ITask
    {
        public void Run()
        {
            var builder = new StringBuilder();
            for (int i = 0; i < 10000; i++)
            {
                builder.Append('a');
            }
            var _ = builder.ToString();
        }
    }

    public class StringConstructorTask : ITask
    {
        public void Run()
        {
            var _ = new string('a', 10000);
        }
    }

    public class Benchmark : IBenchmark
    {
        public double MeasureDurationInMs(ITask task, int repetitionCount)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.WaitForFullGCComplete(); 

            task.Run();

            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < repetitionCount; i++)
            {
                task.Run();
            }
            stopwatch.Stop();

            return stopwatch.Elapsed.TotalMilliseconds / repetitionCount;
        }
    }

    [TestFixture]
    public class RealBenchmarkUsageSample
    {
        [Test]
        public void StringConstructorFasterThanStringBuilder()
        {
            var benchmark = new Benchmark();
            var stringConstructorTask = new StringConstructorTask();
            var stringBuilderTask = new StringBuilderTask();
            int repetitionCount = 1000;
            var stringConstructorTime = benchmark.MeasureDurationInMs(stringConstructorTask, repetitionCount);
            var stringBuilderTime = benchmark.MeasureDurationInMs(stringBuilderTask, repetitionCount);
            
            Assert.That(stringConstructorTime, Is.LessThan(stringBuilderTime),
                "Ожидалось, что создание строки с помощью конструктора будет быстрее, чем с использованием StringBuilder.");
        }
    }
}
