using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace BookstoreAPI.Listeners
{
	/// <summary>
	/// Base class implementing queue consumer pattern of processing.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	internal abstract class BaseRequestProcessor<T> : IDisposable
		where T : class
	{
		private const int NumerOfConsumerThreads = 3;

		private ConcurrentQueue<T> requestQueue;
		private AutoResetEvent processingGate;
		private CancellationTokenSource cts;
		private CancellationToken ct;
		private Task[] consumers;

		/// <summary>
		/// Initializes new instance of <see cref="BaseRequestProcessor"/>
		/// </summary>
		public BaseRequestProcessor()
		{
			requestQueue = new ConcurrentQueue<T>();
			processingGate = new AutoResetEvent(false);
		}

		/// <summary>
		/// Starts processing.
		/// </summary>
		public void Start()
		{
			cts = new CancellationTokenSource();
			ct = cts.Token;

			consumers = new Task[NumerOfConsumerThreads];
			for (int i = 0; i < consumers.Length; i++)
			{
				consumers[i] = Task.Factory.StartNew(() =>
				{
					while (!ct.IsCancellationRequested)
					{
						ct.ThrowIfCancellationRequested();

						processingGate.WaitOne();

						if (!requestQueue.TryDequeue(out T request))
						{
							continue;
						}

						HandleRequest(request);
					}

				}, ct)
				.ContinueWith(t =>
				{
					t.Exception?.Handle(ex =>
					{
						return true;
					});
				}, TaskContinuationOptions.NotOnRanToCompletion);
			}
		}

		/// <summary>
		/// Stops processing.
		/// </summary>
		public void Stop()
		{
			cts.Cancel();
		}

		/// <summary>
		/// Enqueues <paramref name="request"/> for execution.
		/// </summary>
		/// <param name="request">Request to process.</param>
		public void EnqueueForProcessing(T request)
		{
			requestQueue.Enqueue(request);
			processingGate.Set();
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			Stop();
			DrainRequestQueue();
		}

		/// <summary>
		/// Handles request of type <typeparamref name="T"/>.
		/// </summary>
		/// <param name="request">Request to handle.</param>
		protected abstract void HandleRequest(T request);

		/// <summary>
		/// Drains request queue.
		/// </summary>
		private void DrainRequestQueue()
		{
			while (requestQueue.TryDequeue(out _)) ;
		}
	}
}
