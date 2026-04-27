using System.Text.Json;
using System.Xml.Linq;

/// <summary>
/// Converts any object to a FHIR-compatible HTTP response in JSON or XML format.
/// Format is determined by the _format query parameter or Accept header.
/// Swagger UI can use ?_format=xml or ?_format=json.
/// </summary>
public static class FhirResponse
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public static IResult Ok(object resource, string? format, HttpRequest request)
        => WantsXml(format, request)
            ? XmlResult(resource, 200)
            : Results.Json(resource, JsonOptions);

    public static IResult NotFound(object resource, string? format, HttpRequest request)
        => WantsXml(format, request)
            ? XmlResult(resource, 404)
            : Results.Json(resource, JsonOptions, statusCode: 404);

    private static bool WantsXml(string? format, HttpRequest request)
    {
        if (!string.IsNullOrWhiteSpace(format))
            return format.ToLowerInvariant() is "xml" or "application/fhir+xml";

        var accept = request.Headers.Accept.ToString();
        return accept.Contains("xml", StringComparison.OrdinalIgnoreCase);
    }

    private static IResult XmlResult(object resource, int statusCode)
    {
        var json = JsonSerializer.Serialize(resource, JsonOptions);
        var doc  = JsonDocument.Parse(json);
        var xml  = ConvertToFhirXml(doc.RootElement);
        return Results.Content(xml, "application/fhir+xml; charset=utf-8", System.Text.Encoding.UTF8, statusCode);
    }

    // Converts a JSON element to FHIR XML.
    // FHIR XML rules:
    //   - Root element = resourceType value, with xmlns
    //   - Arrays become repeated elements (except "extension" array items use url as attribute)
    //   - Primitives become <elementName value="..."/>
    //   - Objects become nested elements
    private static readonly XNamespace FhirNs = "http://hl7.org/fhir";

    private static string ConvertToFhirXml(JsonElement root)
    {
        var resourceType = root.TryGetProperty("resourceType", out var rt) ? rt.GetString() ?? "Resource" : "Resource";
        var rootEl = new XElement(FhirNs + resourceType);

        foreach (var prop in root.EnumerateObject())
        {
            if (prop.Name == "resourceType") continue;
            AppendElement(rootEl, prop.Name, prop.Value);
        }

        var xmlDoc = new XDocument(new XDeclaration("1.0", "utf-8", null), rootEl);
        return xmlDoc.ToString();
    }

    private static void AppendElement(XElement parent, string name, JsonElement value)
    {
        switch (value.ValueKind)
        {
            case JsonValueKind.Object:
                // Check if it's a nested resource with resourceType (e.g. entry.resource)
                if (value.TryGetProperty("resourceType", out var nestedRt))
                {
                    var nestedEl = new XElement(FhirNs + name);
                    var resourceEl = new XElement(FhirNs + nestedRt.GetString()!);
                    foreach (var p in value.EnumerateObject())
                    {
                        if (p.Name == "resourceType") continue;
                        AppendElement(resourceEl, p.Name, p.Value);
                    }
                    nestedEl.Add(resourceEl);
                    parent.Add(nestedEl);
                }
                else
                {
                    var el = new XElement(FhirNs + name);
                    foreach (var p in value.EnumerateObject())
                        AppendElement(el, p.Name, p.Value);
                    parent.Add(el);
                }
                break;

            case JsonValueKind.Array:
                // Extensions: each item gets url as attribute
                if (name == "extension" || name == "link" || name == "identifier" ||
                    name == "coding" || name == "issue" || name == "entry" ||
                    name == "containedItemQuantity" || name == "manufacturer" ||
                    name == "subject" || name == "marketingStatus")
                {
                    foreach (var item in value.EnumerateArray())
                    {
                        var el = new XElement(FhirNs + name);
                        if (item.ValueKind == JsonValueKind.Object)
                        {
                            // url attribute for extensions
                            if (name == "extension" && item.TryGetProperty("url", out var urlProp))
                            {
                                el.Add(new XAttribute("url", urlProp.GetString()!));
                                foreach (var p in item.EnumerateObject())
                                {
                                    if (p.Name == "url") continue;
                                    AppendElement(el, p.Name, p.Value);
                                }
                            }
                            else
                            {
                                foreach (var p in item.EnumerateObject())
                                    AppendElement(el, p.Name, p.Value);
                            }
                        }
                        else
                        {
                            el.Add(new XAttribute("value", GetPrimitiveValue(item)));
                        }
                        parent.Add(el);
                    }
                }
                else
                {
                    // Generic array — repeat element
                    foreach (var item in value.EnumerateArray())
                    {
                        var el = new XElement(FhirNs + name);
                        if (item.ValueKind == JsonValueKind.Object)
                            foreach (var p in item.EnumerateObject())
                                AppendElement(el, p.Name, p.Value);
                        else
                            el.Add(new XAttribute("value", GetPrimitiveValue(item)));
                        parent.Add(el);
                    }
                }
                break;

            default:
                // Primitive — FHIR XML uses <element value="..."/>
                parent.Add(new XElement(FhirNs + name, new XAttribute("value", GetPrimitiveValue(value))));
                break;
        }
    }

    private static string GetPrimitiveValue(JsonElement el) => el.ValueKind switch
    {
        JsonValueKind.String  => el.GetString() ?? "",
        JsonValueKind.Number  => el.GetRawText(),
        JsonValueKind.True    => "true",
        JsonValueKind.False   => "false",
        JsonValueKind.Null    => "",
        _                     => el.GetRawText()
    };
}
