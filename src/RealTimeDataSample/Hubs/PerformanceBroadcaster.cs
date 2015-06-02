using Microsoft.AspNet.SignalR;
using RealTimeDataSample.Services;

namespace RealTimeDataSample.Hubs
{
	public sealed class PerformanceBroadcaster : IPerformanceBroadcaster
	{
		private IHubContext<PerformanceHub> HubContext { get; }

		public PerformanceBroadcaster(IHubContext<PerformanceHub> hubContext)
		{
			HubContext = hubContext;
		}

		public void BroadcastIoPerSecond(long value)
		{
			HubContext.Clients.All.ioPerSecond(value: value);
		}

		public void BroadcastIoPerMinute(long value)
		{
			HubContext.Clients.All.ioPerMinute(value: value);
		}
	}
}
