using LearningPlatform.Interfaces;

namespace LearningPlatform.Services
{
    public class BackgroundEnrollmentService : BackgroundService
    {
        private readonly IBackgroundTaskQueue _queue;

        public BackgroundEnrollmentService(IBackgroundTaskQueue queue)
        {
            _queue = queue;
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var task = await _queue.DequeueAsync(stoppingToken);

                try
                {
                    await task(stoppingToken);

                }
                catch (Exception ex)
                {
                    // Log the exception or handle it as needed
                    Console.WriteLine($"Error processing background task: {ex.Message}");
                }
            }
        }
    }
}
