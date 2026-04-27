using System.Xml.Linq;

public static class Agreements
{
    public static void MapAgreements(this WebApplication app)
    {
        app.MapGet("/fhir/Agreements", (HttpRequest request, int? _count, int? page, string? _format) =>
        {
            var data     = LoadAgreements();
            var pageSize = Math.Clamp(_count ?? 50, 1, 500);
            var pageNo   = Math.Max(page ?? 1, 1);
            var total    = data.Count;
            var skip     = (pageNo - 1) * pageSize;

            var bundle = new
            {
                resourceType = "Bundle",
                type         = "searchset",
                total        = total,
                link         = Helpers.BuildLinks("/fhir/Agreements", pageSize, pageNo, total, skip),
                entry        = data.Skip(skip).Take(pageSize).Select(a => new
                {
                    fullUrl  = $"http://www.sykehuspartner.no/fhir/Agreements/{a.Id}",
                    resource = MapToResource(a)
                })
            };
            return FhirResponse.Ok(bundle, _format, request);
        })
        .WithName("GetAgreements")
        .WithSummary("Returns all agreement lines (Avtalelinje) from G228_2_Contracts XML");

        app.MapGet("/fhir/Agreements/{id}", (HttpRequest request, string id, string? _format) =>
        {
            var item = LoadAgreements().FirstOrDefault(a => a.Id == id);
            if (item is null)
                return FhirResponse.NotFound(Helpers.FhirNotFound("Agreements", id), _format, request);
            return FhirResponse.Ok(MapToResource(item), _format, request);
        })
        .WithName("GetAgreementById")
        .WithSummary("Returns a single agreement line by Id");
    }

    private static List<AgreementItem> LoadAgreements()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "data", "agreements.xml");
        var ns   = XNamespace.Get("http://www.sykehuspartner.no/sa/erp/soa/xsd/G228_2_AgreementsFromMDM/Agreements/v1.0");
        var doc  = XDocument.Load(path);

        return doc.Root!.Elements(ns + "Avtalelinje").Select(x => new AgreementItem(
            Id:                             x.Element(ns + "Id")?.Value ?? "",
            Varenummer:                     x.Element(ns + "Varenummer")?.Value ?? "",
            LeverandorensInterneVarenummer: x.Element(ns + "LeverandorensInterneVarenummer")?.Value ?? "",
            Prioritet:                      int.TryParse(x.Element(ns + "Prioritet")?.Value, out var p) ? p : 0,
            PrisSalgsenhet:                 decimal.TryParse(x.Element(ns + "PrisSalgsenhet")?.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var ps) ? ps : 0,
            PrisPrimaerenhet:               decimal.TryParse(x.Element(ns + "PrisPrimaerenhet")?.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var pp) ? pp : 0,
            Avtaleenhet:                    x.Element(ns + "Avtaleenhet")?.Value ?? "",
            Detaljistpakningsstorrelse:     int.TryParse(x.Element(ns + "Detaljistpakningsstorrelse")?.Value, out var dp) ? dp : null,
            AvtaleAip:                      decimal.TryParse(x.Element(ns + "AvtaleAip")?.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var aa) ? aa : 0,
            AnbefaltAup:                    decimal.TryParse(x.Element(ns + "AnbefaltAup")?.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var au) ? au : null,
            Organisasjonsnummer:            x.Element(ns + "Organisasjonsnummer")?.Value ?? "",
            Avtalenummer:                   x.Element(ns + "Avtalenummer")?.Value ?? "",
            Avtaleniva:                     x.Element(ns + "Avtaleniva")?.Value ?? "",
            Avtalenavn:                     x.Element(ns + "Avtalenavn")?.Value ?? "",
            Avtalebeskrivelse:              x.Element(ns + "Avtalebeskrivelse")?.Value ?? "",
            LISAvtale:                      bool.TryParse(x.Element(ns + "LISAvtale")?.Value, out var lis) && lis,
            Avtaletype:                     x.Element(ns + "Avtaletype")?.Value ?? "",
            ValutaKode:                     x.Element(ns + "ValutaKode")?.Value ?? "",
            AvtaleStartdato:                x.Element(ns + "AvtaleStartdato")?.Value ?? "",
            AvtaleSluttdato:                x.Element(ns + "AvtaleSluttdato")?.Value ?? "",
            AvtalelinjeStartdato:           x.Element(ns + "AvtalelinjeStartdato")?.Value ?? "",
            AvtalelinjeSluttdato:           x.Element(ns + "AvtalelinjeSluttdato")?.Value ?? "",
            Ledetid:                        int.TryParse(x.Element(ns + "Ledetid")?.Value, out var lt) ? lt : null,
            FargekodeSortimentsstatus:      x.Element(ns + "FargekodeSortimentsstatus")?.Value,
            FargekodeSortiment:             x.Element(ns + "FargekodeSortiment")?.Value,
            FargekodeByttegruppeprioritet:  int.TryParse(x.Element(ns + "FargekodeByttegruppeprioritet")?.Value, out var fb) ? fb : null,
            Kampanjer: x.Elements(ns + "Kampanje").Select(k => new Kampanje(
                Kampanjenavn:      k.Element(ns + "Kampanjenavn")?.Value ?? "",
                Kampanjetype:      k.Element(ns + "Kampanjetype")?.Value ?? "",
                KampanjeStartDato: k.Element(ns + "KampanjeStartDato")?.Value ?? "",
                KampanjeSluttDato: k.Element(ns + "KampanjeSluttDato")?.Value ?? ""
            )).ToList(),
            BildeLinker: x.Elements(ns + "BildeLinker")
                          .Select(b => b.Element(ns + "BildeLink")?.Value ?? "")
                          .Where(s => !string.IsNullOrEmpty(s))
                          .ToList()
        )).ToList();
    }

    private static object MapToResource(AgreementItem a)
    {
        var ext = new List<object>
        {
            Helpers.ExtString ("leverandorensInterneVarenummer", a.LeverandorensInterneVarenummer),
            Helpers.ExtInt    ("prioritet",            a.Prioritet),
            Helpers.ExtDecimal("prisSalgsenhet",       a.PrisSalgsenhet),
            Helpers.ExtDecimal("prisPrimaerenhet",     a.PrisPrimaerenhet),
            Helpers.ExtString ("avtaleenhet",          a.Avtaleenhet),
            Helpers.ExtDecimal("avtaleAip",            a.AvtaleAip),
            Helpers.ExtString ("organisasjonsnummer",  a.Organisasjonsnummer),
            Helpers.ExtString ("avtaleniva",           a.Avtaleniva),
            Helpers.ExtBool   ("lisAvtale",            a.LISAvtale),
            Helpers.ExtString ("avtaletype",           a.Avtaletype),
            Helpers.ExtString ("valutaKode",           a.ValutaKode),
            Helpers.ExtDate   ("avtaleStartdato",      a.AvtaleStartdato),
            Helpers.ExtDate   ("avtaleSluttdato",      a.AvtaleSluttdato),
            Helpers.ExtDate   ("avtalelinjeStartdato", a.AvtalelinjeStartdato),
            Helpers.ExtDate   ("avtalelinjeSluttdato", a.AvtalelinjeSluttdato),
        };

        if (a.Detaljistpakningsstorrelse.HasValue) ext.Add(Helpers.ExtInt    ("detaljistpakningsstorrelse",    a.Detaljistpakningsstorrelse));
        if (a.AnbefaltAup.HasValue)                ext.Add(Helpers.ExtDecimal("anbefaltAup",                  a.AnbefaltAup));
        if (a.Ledetid.HasValue)                    ext.Add(Helpers.ExtInt    ("ledetid",                      a.Ledetid));
        if (!string.IsNullOrEmpty(a.FargekodeSortimentsstatus))  ext.Add(Helpers.ExtString("fargekodeSortimentsstatus",    a.FargekodeSortimentsstatus));
        if (!string.IsNullOrEmpty(a.FargekodeSortiment))         ext.Add(Helpers.ExtString("fargekodeSortiment",           a.FargekodeSortiment));
        if (a.FargekodeByttegruppeprioritet.HasValue)             ext.Add(Helpers.ExtInt   ("fargekodeByttegruppeprioritet",a.FargekodeByttegruppeprioritet));

        foreach (var k in a.Kampanjer)
            ext.Add(Helpers.ExtComplex("kampanje", new object[]
            {
                Helpers.ExtString("kampanjenavn",      k.Kampanjenavn),
                Helpers.ExtString("kampanjetype",      k.Kampanjetype),
                Helpers.ExtDate  ("kampanjeStartDato", k.KampanjeStartDato),
                Helpers.ExtDate  ("kampanjeSluttDato", k.KampanjeSluttDato),
            }));

        foreach (var link in a.BildeLinker)
            ext.Add(Helpers.ExtUri("bildeLink", link));

        return new
        {
            resourceType = "Contract",
            id           = a.Id,
            identifier   = new[] { new { system = "https://sykehuspartner.no/avtalenummer", value = a.Avtalenummer } },
            subject      = new[] { new { reference = $"PackagedProductDefinition/{a.Varenummer}" } },
            title        = a.Avtalenavn,
            subtitle     = a.Avtalebeskrivelse,
            status       = "executed",
            extension    = ext
        };
    }
}
