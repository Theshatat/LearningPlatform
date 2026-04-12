using LearningPlatform.Interfaces;
using System.Threading.Channels;

namespace LearningPlatform.InterfaceImplementation
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<Func<CancellationToken, Task>> _queue;
        public BackgroundTaskQueue()
        {
            _queue = Channel.CreateUnbounded<Func<CancellationToken, Task>>(); // Create an unbounded channel for storing tasks
        }
        // Dequeue a task to be processed in the background so that the background service can execute it
        public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            return await _queue.Reader.ReadAsync(cancellationToken);
        }
        // Enqueue a task to be processed in the background that will be executed by the background service
        public void QueueTask(Func<CancellationToken, Task> task)
        {
            _queue.Writer.TryWrite(task);
        }
    }
}
