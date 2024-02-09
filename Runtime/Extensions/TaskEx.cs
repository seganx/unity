using System.Threading.Tasks;

namespace SeganX
{
    public static class TaskEx
    {
        public static async Task Yield(int frames = 1)
        {
            await Task.Yield();
        }

        public static async Task DelayInFrames(int frames = 1)
        {
            while (--frames > 0)
                await Task.Yield();
        }

        public static async Task DelayInSeconds(float seconds)
        {
            var stopwatch = Utils.StopWatch.Create();
            while (stopwatch.RealtimeTimer < seconds)
                await Task.Yield();
        }
    }
}