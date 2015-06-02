using RealTimeDataSample.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RealTimeDataSample
{
    public sealed class PerformanceGenerator : IDisposable
    {
		private const int RandomMinValue = 250;
		private const int RandomMaxValue = 2500000;

		private readonly Random _random = new Random();
		private readonly CancellationTokenSource _cts = new CancellationTokenSource();
		private readonly long[] _ioPerSecond = new long[60];
		private readonly Task _performanceGenerationTask;
		private long _ioPerMinute;
		private int _secondIndex;

		public long IoPerMinute { get { return Volatile.Read(ref _ioPerMinute); } }
		public long IoPerSecond { get { return Volatile.Read(ref _ioPerSecond[Volatile.Read(ref _secondIndex)]); } }

		private IPerformanceBroadcaster Broadcaster { get; }

		public PerformanceGenerator(IPerformanceBroadcaster broadcaster)
		{
			Broadcaster = broadcaster;
			Initialize();
			_performanceGenerationTask = Task.Run(GeneratePerformance);
		}

		public void Dispose()
		{
			_cts.Cancel();
			try { _performanceGenerationTask.Wait(); }
			catch (OperationCanceledException) { }
		}

		private long GetRandomTick()
		{
			return _random.Next(RandomMinValue, RandomMaxValue + 1);
		}

		private void Initialize()
		{
			for (int i = 0; i < 60; i++)
			{
				_ioPerMinute += _ioPerSecond[i] = GetRandomTick();
            }
		}

		private async Task GeneratePerformance()
		{
			while (true)
			{
				PushPerformanceData(GetRandomTick());
				await Task.Delay(1000, _cts.Token).ConfigureAwait(false);
			}
		}

		private void PushPerformanceData(long data)
		{
			int i = _secondIndex + 1;

			if (i == 60) i = 0;

			long ioPerMinuteDelta = data - _ioPerSecond[i];

			Volatile.Write(ref _ioPerSecond[i], data);
			Volatile.Write(ref _ioPerMinute, _ioPerMinute + ioPerMinuteDelta);
			Volatile.Write(ref _secondIndex, i);

			Broadcaster.BroadcastIoPerSecond(IoPerSecond);
			Broadcaster.BroadcastIoPerMinute(IoPerMinute);
		}
	}
}
