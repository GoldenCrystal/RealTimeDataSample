using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using RealTimeDataSample.Services;

namespace RealTimeDataSample.Hubs
{
	[HubName("performanceHub")]
	public class PerformanceHub : Hub
    {
		private IPerformanceBroadcaster Broadcaster { get; }

		public PerformanceHub(IPerformanceBroadcaster broadcaster)
		{
			Broadcaster = broadcaster;
		}

		private void BroadcastIoPerSecond(long value)
		{
			Broadcaster.BroadcastIoPerSecond(value);
		}

		private void BroadcastIoPerMinute(long value)
		{
			Broadcaster.BroadcastIoPerMinute(value);
		}
	}
}
