using System;

namespace ProjectX.Common
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts unix milliseconds to DateTime value
        /// </summary>
        public static DateTime FromUnixMillisecondsToUTCDateTime(this long time) 
            =>  DateTime.SpecifyKind(DateTimeOffset.FromUnixTimeMilliseconds(time).UtcDateTime, DateTimeKind.Utc);

        /// <summary>
        /// Converts unix milliseconds to DateTime value
        /// </summary>
        public static DateTime? FromUnixMillisecondsToUTCDateTime(this long? time)
            => time.HasValue
                ? new DateTime?(DateTime.SpecifyKind(DateTimeOffset.FromUnixTimeMilliseconds(time.Value).DateTime, DateTimeKind.Utc)) 
                : default;

        /// <summary>
        /// Convert DateTime to unix miliseconds
        /// </summary>
        public static long ToUnixMilliseconds(this DateTime time) 
            => time.ToUnixTime(false);

        /// <summary>
        /// Convert nullable DateTime to unix miliseconds
        /// </summary>
        public static long? ToUnixMilliseconds(this DateTime? time)
        {
            if (time.HasValue)
                return time.Value.ToUnixTime(false);

            return null;
        }

        public static long ToUnixTime(this DateTime dt, bool isSeconds = false)
        {
            var time = dt - DateTime.UnixEpoch;

            return (isSeconds ? (Int64)time.TotalSeconds : (Int64)time.TotalMilliseconds);
        }
    }
}
