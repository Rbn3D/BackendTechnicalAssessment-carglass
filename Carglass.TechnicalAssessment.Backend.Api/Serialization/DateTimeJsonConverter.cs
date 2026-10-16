using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Carglass.TechnicalAssessment.Backend.Api.Serialization;

public sealed class DateTimeJsonConverter : JsonConverter<DateTime>
{
    public const string Format = "yyyy-MM-dd HH:mm:ss";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TryGetDateTime(out var isoDate))
            return isoDate;

        var value = reader.GetString();
        if (DateTime.TryParseExact(value, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            return date;

        throw new JsonException($"Fecha no válida: '{value}'. Formato esperado: '{Format}'.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
    }
}
