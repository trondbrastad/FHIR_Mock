using System.Globalization;

public static class Helpers
{
    public static decimal? TryParseDecimalNo(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        var normalized = s.Trim().Replace(",", ".");
        return decimal.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : null;
    }

    public static string Slug(string s)
    {
        var chars = s.ToLowerInvariant()
            .Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_')
            .ToArray();
        return new string(chars);
    }

    // Flat, FHIR-compliant simple extensions — { "url": "...", "valueString": "foo" }
    public static Dictionary<string, object?> ExtString(string name, string? value)
        => Ext(name, "valueString", value);

    public static Dictionary<string, object?> ExtBool(string name, bool value)
        => Ext(name, "valueBoolean", (object)value);

    public static Dictionary<string, object?> ExtInt(string name, int? value)
        => Ext(name, "valueInteger", (object?)value);

    public static Dictionary<string, object?> ExtDecimal(string name, decimal? value)
        => Ext(name, "valueDecimal", (object?)value);

    public static Dictionary<string, object?> ExtDate(string name, string? value)
        => Ext(name, "valueDate", value);

    public static Dictionary<string, object?> ExtDateTime(string name, string? value)
        => Ext(name, "valueDateTime", value);

    public static Dictionary<string, object?> ExtUri(string name, string? value)
        => Ext(name, "valueUri", value);

    // Complex extension (nested, no value) — { "url": "...", "extension": [ ... ] }
    public static Dictionary<string, object?> ExtComplex(string name, IEnumerable<object> nested)
        => new()
        {
            ["url"]       = $"https://sykehuspartner.no/fhir/StructureDefinition/sa-mdm-{name}",
            ["extension"] = nested.ToList()
        };

    private static Dictionary<string, object?> Ext(string name, string valueKey, object? value)
        => new()
        {
            ["url"]    = $"https://sykehuspartner.no/fhir/StructureDefinition/sa-mdm-{name}",
            [valueKey] = value
        };

    private const string BaseUrl = "http://www.sykehuspartner.no";

    public static IEnumerable<object> BuildLinks(string path, int pageSize, int pageNo, int total, int skip)
    {
        yield return new { relation = "self", url = $"{BaseUrl}{path}?_count={pageSize}&page={pageNo}" };
        if (skip + pageSize < total)
            yield return new { relation = "next", url = $"{BaseUrl}{path}?_count={pageSize}&page={pageNo + 1}" };
    }

    public static object FhirNotFound(string resource, string id) => new
    {
        resourceType = "OperationOutcome",
        issue = new[] { new { severity = "error", code = "not-found", diagnostics = $"{resource}/{id} not found" } }
    };
}
