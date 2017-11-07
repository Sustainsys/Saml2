using System;
using System.Xml;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Helper methods for DateTime formatting.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Format a datetime for inclusion in SAML messages.
        /// </summary>
        /// <param name="dateTime">Datetime to format.</param>
        /// <returns>Formatted value.</returns>
        public static string ToSaml2DateTimeString(this DateTime dateTime)
        {
            return XmlConvert.ToString(dateTime.AddTicks(-(dateTime.Ticks % TimeSpan.TicksPerSecond)),
                XmlDateTimeSerializationMode.Utc);
        }
    }
}
