using System.Buffers.Binary;
using System.IO.Pipelines;

using BenchmarkDotNet.Attributes;

using Syroot.BinaryData;

namespace RlUpk.Core.Benchmark;

public class IOBenchmark
{
    private Stream input;
    private PipeReader pipeReader;
    private int _iterations = 1000000;

    [GlobalSetup]
    public void Setup()
    {
        input = File.OpenRead("TestData/TAGame.upk");
    }

    [IterationSetup]
    public void IterationSetup()
    {
        input.Seek(0, SeekOrigin.Begin);
        pipeReader = PipeReader.Create(input);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        input.Dispose();
    }

    [Benchmark(Baseline = true)]
    public int ReadInt32_Syroot()
    {
        var s = 0;
        for (int i = 0; i < _iterations; i++)
        {
            s += input.ReadInt32();
            s += input.ReadInt32();
            s += input.ReadInt32();
            s += input.ReadInt32();
        }

        return s;
    }

    [Benchmark]
    public int ReadInt32_BitConverter_StackAllocBuffer()
    {
        var s = 0;
        Span<byte> buffer = stackalloc byte[4 * sizeof(int)];
        for (int i = 0; i < _iterations; i++)
        {
            input.ReadExactly(buffer);
            s += BitConverter.ToInt32(buffer.Slice(0, 4));
            s += BitConverter.ToInt32(buffer.Slice(4, 4));
            s += BitConverter.ToInt32(buffer.Slice(8, 4));
            s += BitConverter.ToInt32(buffer.Slice(12, 4));
        }

        return s;
    }

    [Benchmark]
    public int ReadInt32_BinaryPrimitives_StackAllocBuffer()
    {
        Span<byte> buffer = stackalloc byte[4 * sizeof(int)];
        var s = 0;
        for (int i = 0; i < _iterations; i++)
        {
            input.ReadExactly(buffer);
            s += BinaryPrimitives.ReadInt32LittleEndian(buffer.Slice(0, 4));
            s += BinaryPrimitives.ReadInt32LittleEndian(buffer.Slice(4, 4));
            s += BinaryPrimitives.ReadInt32LittleEndian(buffer.Slice(8, 4));
            s += BinaryPrimitives.ReadInt32LittleEndian(buffer.Slice(12, 4));
        }

        return s;
    }

    static readonly byte[] buffer1 = new byte[4 * sizeof(int)];

    [Benchmark]
    public int ReadInt32_BinaryPrimitives_StaticBuffer()
    {
        var s = 0;
        for (int i = 0; i < _iterations; i++)
        {
            input.ReadExactly(buffer1);
            s += BinaryPrimitives.ReadInt32LittleEndian(buffer1.AsSpan(0, 4));
            s += BinaryPrimitives.ReadInt32LittleEndian(buffer1.AsSpan(4, 4));
            s += BinaryPrimitives.ReadInt32LittleEndian(buffer1.AsSpan(8, 4));
            s += BinaryPrimitives.ReadInt32LittleEndian(buffer1.AsSpan(12, 4));
        }

        return s;
    }

    [Benchmark]
    public async Task<int> ReadInt32_PipelineReader()
    {
        var s = 0;
        for (int i = 0; i < _iterations; i++)
        {
            var memTask = pipeReader.ReadAtLeastAsync(sizeof(int) * 4);
            if (memTask.IsCompleted)
            {
                s += ReadFourInts(memTask.Result.Buffer.FirstSpan);
                continue;
            }
            ReadFourInts((await memTask).Buffer.FirstSpan);
        }

        return s;

        int ReadFourInts(ReadOnlySpan<byte> memSpan)
        {
            s += BinaryPrimitives.ReadInt32LittleEndian(memSpan.Slice(0, 4));
            s += BinaryPrimitives.ReadInt32LittleEndian(memSpan.Slice(4, 4));
            s += BinaryPrimitives.ReadInt32LittleEndian(memSpan.Slice(8, 4));
            s += BinaryPrimitives.ReadInt32LittleEndian(memSpan.Slice(12, 4));
            return s;
        }
    }
}