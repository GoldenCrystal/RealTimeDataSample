namespace RealTimeDataSample.Services
{
	public interface IPerformanceBroadcaster
	{
		void BroadcastIoPerSecond(long value);
		void BroadcastIoPerMinute(long value);
	}
}