using System.Xml.Linq;

public static class ItemGroups
{
    public static void MapItemGroups(this WebApplication app)
    {
        app.MapGet("/fhir/ItemGroups", (HttpRequest request, int? _count, int? page, string? _format) =>
        {
            var data     = LoadItemGroups();
            var pageSize = Math.Clamp(_count ?? 50, 1, 500);
            var pageNo   = Math.Max(page ?? 1, 1);
            var total    = data.Count;
            var skip     = (pageNo - 1) * pageSize;

            var bundle = new
            {
                resourceType = "Bundle",
                type         = "searchset",
                total        = total,
                link         = Helpers.BuildLinks("/fhir/ItemGroups", pageSize, pageNo, total, skip),
                entry        = data.Skip(skip).Take(pageSize).Select(g => new
                {
                    fullUrl  = $"http://www.sykehuspartner.no/fhir/ItemGroups/{Uri.EscapeDataString(g.Code)}",
                    resource = MapToResource(g)
                })
            };
            return FhirResponse.Ok(bundle, _format, request);
        })
        .WithName("GetItemGroups")
        .WithSummary("Returns all item groups (Varegruppe) from G228_6_Varegruppe XML");

        app.MapGet("/fhir/ItemGroups/{code}", (HttpRequest request, string code, string? _format) =>
        {
            var item = LoadItemGroups().FirstOrDefault(g => g.Code == code);
            if (item is null)
                return FhirResponse.NotFound(Helpers.FhirNotFound("ItemGroups", code), _format, request);
            return FhirResponse.Ok(MapToResource(item), _format, request);
        })
        .WithName("GetItemGroupByCode")
        .WithSummary("Returns a single item group by Code");
    }

    private static List<ItemGroupItem> LoadItemGroups()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "data", "itemgroups.xml");
        var ns   = XNamespace.Get("http://www.sykehuspartner.no/sa/erp/soa/xsd/G228_6_Varegruppe/v1.0");
        var doc  = XDocument.Load(path);

        return doc.Root!.Elements(ns + "Varegruppe").Select(x => new ItemGroupItem(
            Code:           x.Element(ns + "Code")?.Value ?? "",
            Name_no:        x.Element(ns + "Name_no")?.Value ?? "",
            Description_no: x.Element(ns + "Description_no")?.Value,
            Name_nn:        x.Element(ns + "Name_nn")?.Value,
            Description_nn: x.Element(ns + "Description_nn")?.Value,
            Name_en:        x.Element(ns + "Name_en")?.Value,
            Description_en: x.Element(ns + "Description_en")?.Value
        )).ToList();
    }

    private static object MapToResource(ItemGroupItem g)
    {
        var ext = new List<object>();
        if (g.Name_nn        != null) ext.Add(Helpers.ExtString("name_nn",        g.Name_nn));
        if (g.Description_nn != null) ext.Add(Helpers.ExtString("description_nn", g.Description_nn));
        if (g.Name_en        != null) ext.Add(Helpers.ExtString("name_en",        g.Name_en));
        if (g.Description_en != null) ext.Add(Helpers.ExtString("description_en", g.Description_en));

        return new
        {
            resourceType = "CodeSystem",
            id           = g.Code,
            url          = $"https://sykehuspartner.no/fhir/CodeSystem/varegruppe/{g.Code}",
            identifier   = new[] { new { system = "https://sykehuspartner.no/varegruppe", value = g.Code } },
            content      = "complete",
            name         = g.Name_no,
            description  = g.Description_no,
            status       = "active",
            extension    = ext
        };
    }
}
