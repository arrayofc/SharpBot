using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Discord.WebSocket;

namespace SharpBot.Utility {
    public static class TimeUtil {

        /// <summary>
        /// Date format used to convert time to a human-readable detailed form.
        /// </summary>
        private const string DATE_FORMAT_NICE = "dddd, dd MMMM yyyy HH:mm:ss";
        
        /// <summary>
        /// Date format used to convert time to a human-readable less detailed form.
        /// </summary>
        private const string DATE_FORMAT_SMALLER = "ddd, dd MMM yyy HH:mm:ss";
        
        /// <summary>
        /// Date format used to convert time to a human-readable compact form.
        /// </summary>
        private const string DATE_FORMAT_COMPACT = "MM/dd/yyyy HH:mm:ss";

        /// <summary>
        /// Converts the <para>millis</para> to a human-readable time lapse.
        /// </summary>
        /// <param name="millis">The time in millis to convert.</param>
        public static string Convert(long millis) {
            var span = TimeSpan.FromMilliseconds(millis);
            var parts = $"{span.Days:D2}d:{span.Hours:D2}h:{span.Minutes:D2}m:{span.Seconds:D2}s:{span.Milliseconds:D3}ms"
                .Split(':')
                .SkipWhile(s => Regex.Match(s, @"^00\w").Success) // skip zero-valued components
                .ToArray();
            return string.Join(" ", parts);
        }

        public static string NowCompact() {
            return DateTime.Now.ToString(DATE_FORMAT_COMPACT);
        }
        
        public static string NowDetailed() {
            return DateTime.Now.ToString(DATE_FORMAT_NICE);
        }
        
        public static string NowSmall() {
            return DateTime.Now.ToString(DATE_FORMAT_SMALLER);
        }

        public static string WhenCompact(long millis) {
            return DateTimeOffset.FromUnixTimeMilliseconds(millis).ToString(DATE_FORMAT_COMPACT);
        }
        
        public static string WhenDetailed(long millis) {
            return DateTimeOffset.FromUnixTimeMilliseconds(millis).ToString(DATE_FORMAT_NICE);
        }
        
        public static string WhenSmall(long millis) {
            return DateTimeOffset.FromUnixTimeMilliseconds(millis).ToString(DATE_FORMAT_SMALLER);
        }
    }
}