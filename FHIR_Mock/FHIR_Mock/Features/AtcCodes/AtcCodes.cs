using System.Xml.Linq;

public static class AtcCodes
{
    public static void MapAtcCodes(this WebApplication app)
    {
        app.MapGet("/fhir/AtcCodes", (HttpRequest request, int? _count, int? page, string? _format) =>
        {
            var data     = LoadAtcCodes();
            var pageSize = Math.Clamp(_count ?? 50, 1, 500);
            var pageNo   = Math.Max(page ?? 1, 1);
            var total    = data.Count;
            var skip     = (pageNo - 1) * pageSize;

            var bundle = new
            {
                resourceType = "Bundle",
                type         = "searchset",
                total        = total,
                link         = Helpers.BuildLinks("/fhir/AtcCodes", pageSize, pageNo, total, skip),
                entry        = data.Skip(skip).Take(pageSize).Select(a => new
                {
                    fullUrl  = $"http://www.sykehuspartner.no/fhir/AtcCodes/{Uri.EscapeDataString(a.Code)}",
                    resource = MapToResource(a)
                })
            };
            return FhirResponse.Ok(bundle, _format, request);
        })
        .WithName("GetAtcCodes")
        .WithSummary("Returns all ATC codes from G228_5_ATC XML");

        app.MapGet("/fhir/AtcCodes/{code}", (HttpRequest request, string code, string? _format) =>
        {
            var item = LoadAtcCodes().FirstOrDefault(a => a.Code == code);
            if (item is null)
                return FhirResponse.NotFound(Helpers.FhirNotFound("AtcCodes", code), _format, request);
            return FhirResponse.Ok(MapToResource(item), _format, request);
        })
        .WithName("GetAtcCodeByCode")
        .WithSummary("Returns a single ATC code entry by Code");
    }

    private static List<AtcCodeItem> LoadAtcCodes()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "data", "atccodes.xml");
        var ns   = XNamespace.Get("http://www.sykehuspartner.no/sa/erp/soa/xsd/G228_5_ATC/v1.0");
        var doc  = XDocument.Load(path);

        return doc.Root!.Elements(ns + "ATC").Select(x => new AtcCodeItem(
            Code:           x.Element(ns + "Code")?.Value ?? "",
            Name_no:        x.Element(ns + "Name_no")?.Value ?? "",
            Description_no: x.Element(ns + "Description_no")?.Value,
            Name_nn:        x.Element(ns + "Name_nn")?.Value,
            Description_nn: x.Element(ns + "Description_nn")?.Value,
            Name_en:        x.Element(ns + "Name_en")?.Value,
            Description_en: x.Element(ns + "Description_en")?.Value
        )).ToList();
    }

    private static object MapToResource(AtcCodeItem a)
    {
        var ext = new List<object>();
        if (a.Name_nn        != null) ext.Add(Helpers.ExtString("name_nn",        a.Name_nn));
        if (a.Description_nn != null) ext.Add(Helpers.ExtString("description_nn", a.Description_nn));
        if (a.Name_en        != null) ext.Add(Helpers.ExtString("name_en",        a.Name_en));
        if (a.Description_en != null) ext.Add(Helpers.ExtString("description_en", a.Description_en));

        return new
        {
            resourceType = "CodeSystem",
            id           = a.Code,
            url          = $"https://sykehuspartner.no/fhir/CodeSystem/atc/{a.Code}",
            identifier   = new[] { new { system = "https://sykehuspartner.no/atc", value = a.Code } },
            content      = "complete",
            name         = a.Name_no,
            description  = a.Description_no,
            status       = "active",
            extension    = ext
        };
    }
}
