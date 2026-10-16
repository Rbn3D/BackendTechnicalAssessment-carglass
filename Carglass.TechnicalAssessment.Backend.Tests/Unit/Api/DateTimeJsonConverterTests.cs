using Carglass.TechnicalAssessment.Backend.Api.Serialization;
using System.Globalization;
using System.Text.Json;

namespace Carglass.TechnicalAssessment.Backend.Tests.Unit.Api;

public class DateTimeJsonConverterTests
{
    private readonly JsonSerializerOptions _options = new() { Converters = { new DateTimeJsonConverter() } };

    [Theory]
    [InlineData("\"2019-01-09 14:26:17\"")]   // formato del README
    [InlineData("\"2019-01-09T14:26:17\"")]   // ISO 8601 (ejemplo de Swagger)
    public void Reads_supported_formats(string json)
    {
        var result = JsonSerializer.Deserialize<DateTime>(json, _options);

        Assert.Equal(new DateTime(2019, 1, 9, 14, 26, 17), result);
    }

    [Theory]
    [InlineData("\"09/01/2019 14:26:17\"")]
    [InlineData("\"2019-13-01 14:26:17\"")]
    [InlineData("\"\"")]
    public void Rejects_unsupported_formats(string json)
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<DateTime>(json, _options));
    }

    [Fact]
    public void Writes_readme_format()
    {
        var json = JsonSerializer.Serialize(new DateTime(2019, 1, 9, 14, 26, 17), _options);

        Assert.Equal("\"2019-01-09 14:26:17\"", json);
    }

    [Fact]
    public void Writes_same_format_regardless_of_current_culture()
    {
        var original = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("ar-SA");   // calendario y dígitos distintos
            var json = JsonSerializer.Serialize(new DateTime(2019, 1, 9, 14, 26, 17), _options);

            Assert.Equal("\"2019-01-09 14:26:17\"", json);
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }
}
