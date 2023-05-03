using System;
using Newtonsoft.Json;

namespace Acme.IdentityServer.WebApi.Json {
    public class IsoTimeSpanConverter : JsonConverter {
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer) {

            if (value == null) {
                writer.WriteNull();
                return;
            }

            var isValueTimeSpan = value is TimeSpan;

            if (!isValueTimeSpan) {
                throw new JsonSerializationException("Expected TimeSpan object value.");
            }

            var timeSpan = (TimeSpan)value;

            if (timeSpan == TimeSpan.MaxValue) {
                var text = "P99Y12M31DT23H59M59S";
                writer.WriteValue(text);
            } else {
                var totalDays = Math.Floor(timeSpan.TotalDays);
                var years = Math.Truncate(totalDays / 365);
                var months = Math.Truncate((totalDays % 365) / 30);
                var days = Math.Truncate((totalDays % 365) % 30);
                var text = $"P{years}Y{months}M{days}DT{timeSpan.Hours}H{timeSpan.Minutes}M{timeSpan.Seconds}S";
                writer.WriteValue(text);
            }
        }

        private bool IsNullableType(Type objectType) {
            return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing property value of the JSON that is being converted.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer) {
            bool isNullable = IsNullableType(objectType);

            if (reader.TokenType == JsonToken.Null) {
                if (!isNullable) {
                    throw new JsonSerializationException(string.Format("Cannot convert null value to {0}.", objectType));
                }

                return null;
            }

            Type t = isNullable ? Nullable.GetUnderlyingType(objectType) : objectType;


            if (reader.TokenType == JsonToken.String) {
                var timeSpanText = reader.Value.ToString();
                var span = System.Xml.XmlConvert.ToTimeSpan(timeSpanText);
                return span;
            }


            throw new JsonSerializationException(string.Format("Unexpected token {0} when parsing TimeSpan.", reader.TokenType));
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType) {

            Type t = IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType;

            return typeof(TimeSpan) == t;
        }
    }
}
