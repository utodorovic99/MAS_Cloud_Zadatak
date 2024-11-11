using System;
using System.Threading.Tasks;

namespace CommunicationsSDK.PlatformExtensions
{
	/// <summary>
	/// Extension class for <see cref="TaskExtensions"/>
	/// </summary>
	public static class TaskExtensions
	{
		/// <summary>
		/// Allows throwing custom type of extension on task failure.
		/// </summary>
		/// <param name="task">Task whose exception throwing is configured.</param>
		/// <param name="exceptionCreationFunc">Function creating custom exception.</param>
		/// <returns>Original task.</returns>
		/// <remarks>
		/// Method is wrapper around <see cref="Task.ContinueWith(Action{Task}, TaskContinuationOptions)"/>.
		/// </remarks>
		public static Task ThrowOnFailure(this Task task, Func<Task, Exception> exceptionCreationFunc)
		{
			return task.ContinueWith(t =>
			{
				t.Exception?.Handle(e =>
				{
					throw exceptionCreationFunc(t);
				});
			}, TaskContinuationOptions.OnlyOnFaulted);
		}
	}
}
