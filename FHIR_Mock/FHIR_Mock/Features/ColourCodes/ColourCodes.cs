using System.Xml.Linq;

public static class ColourCodes
{
    public static void MapColourCodes(this WebApplication app)
    {
        app.MapGet("/fhir/ColourCodes", (HttpRequest request, int? _count, int? page, string? _format) =>
        {
            var data     = LoadColourCodes();
            var pageSize = Math.Clamp(_count ?? 50, 1, 500);
            var pageNo   = Math.Max(page ?? 1, 1);
            var total    = data.Count;
            var skip     = (pageNo - 1) * pageSize;

            var bundle = new
            {
                resourceType = "Bundle",
                type         = "searchset",
                total        = total,
                link         = Helpers.BuildLinks("/fhir/ColourCodes", pageSize, pageNo, total, skip),
                entry        = data.Skip(skip).Take(pageSize).Select(c => new
                {
                    fullUrl  = $"http://www.sykehuspartner.no/fhir/ColourCodes/{c.Id}",
                    resource = MapToResource(c)
                })
            };
            return FhirResponse.Ok(bundle, _format, request);
        })
        .WithName("GetColourCodes")
        .WithSummary("Returns all colour codes (Fargekode) from G228_7_Fargekoder XML");

        app.MapGet("/fhir/ColourCodes/{id}", (HttpRequest request, string id, string? _format) =>
        {
            var item = LoadColourCodes().FirstOrDefault(c => c.Id == id);
            if (item is null)
                return FhirResponse.NotFound(Helpers.FhirNotFound("ColourCodes", id), _format, request);
            return FhirResponse.Ok(MapToResource(item), _format, request);
        })
        .WithName("GetColourCodeById")
        .WithSummary("Returns a single colour code by Id");
    }

    private static List<ColourCodeItem> LoadColourCodes()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "data", "colourcodes.xml");
        var ns   = XNamespace.Get("http://www.sykehuspartner.no/sa/erp/soa/xsd/G228_7_Fargekoder/Fargekoder/v1.0");
        var doc  = XDocument.Load(path);

        return doc.Root!.Elements(ns + "Fargekode").Select(x => new ColourCodeItem(
            Id:                            x.Element(ns + "Id")?.Value ?? "",
            Varenummer:                    x.Element(ns + "Varenummer")?.Value ?? "",
            Avtaleenhet:                   x.Element(ns + "Avtaleenhet")?.Value ?? "",
            Avtalenummer:                  x.Element(ns + "Avtalenummer")?.Value ?? "",
            FargekodeSortimentsstatus:     x.Element(ns + "FargekodeSortimentsstatus")?.Value,
            FargekodeSortiment:            x.Element(ns + "FargekodeSortiment")?.Value,
            FargekodeByttegruppeprioritet: int.TryParse(x.Element(ns + "FargekodeByttegruppeprioritet")?.Value, out var v) ? v : null
        )).ToList();
    }

    private static object MapToResource(ColourCodeItem c)
    {
        var ext = new List<object>
        {
            Helpers.ExtString("varenummer",   c.Varenummer),
            Helpers.ExtString("avtaleenhet",  c.Avtaleenhet),
            Helpers.ExtString("avtalenummer", c.Avtalenummer),
        };
        if (c.FargekodeSortimentsstatus     != null) ext.Add(Helpers.ExtString("fargekodeSortimentsstatus",     c.FargekodeSortimentsstatus));
        if (c.FargekodeSortiment            != null) ext.Add(Helpers.ExtString("fargekodeSortiment",            c.FargekodeSortiment));
        if (c.FargekodeByttegruppeprioritet.HasValue) ext.Add(Helpers.ExtInt  ("fargekodeByttegruppeprioritet", c.FargekodeByttegruppeprioritet));

        return new
        {
            resourceType = "Basic",
            id           = c.Id,
            identifier   = new[] { new { system = "https://sykehuspartner.no/fargekode", value = c.Id } },
            subject      = new { reference = $"PackagedProductDefinition/{c.Varenummer}" },
            code         = new { coding = new[] { new { system = "https://sykehuspartner.no/fhir/CodeSystem/fargekode", code = "Fargekode" } } },
            extension    = ext
        };
    }
}
