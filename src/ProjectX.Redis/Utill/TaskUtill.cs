using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Redis.Utill
{
	internal static class TaskUtils
	{
		public static Task Delay(TimeSpan timeSpan)
		{
			return Delay((int)timeSpan.TotalMilliseconds);
		}

		public static Task Delay(TimeSpan timeSpan, CancellationToken cancellationToken)
		{
			return Delay((int)timeSpan.TotalMilliseconds, cancellationToken);
		}

		public static Task Delay(int delayMs)
		{
			return Task.Delay(delayMs);
		}

		public static Task Delay(int delayMs, CancellationToken cancellationToken)
		{
			return Task.Delay(delayMs, cancellationToken);
		}

		public static Task<T[]> WhenAll<T>(IEnumerable<Task<T>> tasks)
		{
			return Task.WhenAll(tasks);
		}
	}
}
