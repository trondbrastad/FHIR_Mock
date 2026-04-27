using System.Xml.Linq;

public static class Articles
{
    private static readonly List<MdmItem> _items = LoadItems();

    public static void MapArticles(this WebApplication app)
    {
        app.MapGet("/fhir/GetArticles", (HttpRequest request, int? _count, int? page, string? _format) =>
        {
            var pageSize = Math.Clamp(_count ?? 50, 1, 500);
            var pageNo   = Math.Max(page ?? 1, 1);
            var total    = _items.Count;
            var skip     = (pageNo - 1) * pageSize;

            var bundle = new
            {
                resourceType = "Bundle",
                type         = "searchset",
                total        = total,
                link         = Helpers.BuildLinks("/fhir/GetArticles", pageSize, pageNo, total, skip),
                entry        = _items.Skip(skip).Take(pageSize).Select(i => new
                {
                    fullUrl  = $"http://www.sykehuspartner.no/fhir/GetArticles/{i.Varenummer}",
                    resource = MapToResource(i)
                })
            };
            return FhirResponse.Ok(bundle, _format, request);
        })
        .WithName("GetArticles")
        .WithSummary("Returns all articles")
        .Produces(200, contentType: "application/fhir+json")
        .Produces(200, contentType: "application/fhir+xml");

        app.MapGet("/fhir/GetArticle/{varenummer}", (HttpRequest request, string varenummer, string? _format) =>
        {
            var item = _items.FirstOrDefault(x => x.Varenummer == varenummer);
            if (item is null)
                return FhirResponse.NotFound(Helpers.FhirNotFound("PackagedProductDefinition", varenummer), _format, request);
            return FhirResponse.Ok(MapToResource(item), _format, request);
        })
        .WithName("GetArticleById")
        .WithSummary("Returns a single article by Varenummer")
        .Produces(200, contentType: "application/fhir+json")
        .Produces(200, contentType: "application/fhir+xml");
    }

    private static List<MdmItem> LoadItems()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "data", "articles.xml");
        var ns   = XNamespace.Get("http://www.sykehuspartner.no/sa/erp/soa/xsd/G228_1_ItemsFromMDM/Products/v1.0");
        var doc  = XDocument.Load(path);

        return doc.Root!.Elements(ns + "Article").Select(x => new MdmItem(
            Id:                              x.Element(ns + "Id")?.Value ?? "",
            Varenummer:                      x.Element(ns + "Varenummer")?.Value ?? "",
            Varebetegnelse:                  x.Element(ns + "Varebetegnelse")?.Value ?? "",
            VarebetegnelseLang:              x.Element(ns + "VarebetegnelseLang")?.Value ?? "",
            Varenavn:                        x.Element(ns + "Varenavn")?.Value ?? "",
            NasjonalKategori:                x.Element(ns + "NasjonalKategori")?.Value ?? "",
            Varegruppenr:                    x.Element(ns + "Varegruppenr")?.Value ?? "",
            Varegruppe:                      x.Element(ns + "Varegruppe")?.Value ?? "",
            Mellomgruppenr:                  x.Element(ns + "Mellomgruppenr")?.Value ?? "",
            Mellomgruppe:                    x.Element(ns + "Mellomgruppe")?.Value ?? "",
            Varetype:                        x.Element(ns + "Varetype")?.Value ?? "",
            Varekategori:                    x.Element(ns + "Varekategori")?.Value ?? "",
            Pakningsstorrelse:               int.Parse(x.Element(ns + "Pakningsstorrelse")?.Value ?? "1"),
            EnhetForKvantum:                 x.Element(ns + "EnhetForKvantum")?.Value ?? "",
            Primaerenhet:                    x.Element(ns + "Primaerenhet")?.Value ?? "",
            Salgsenhet:                      x.Element(ns + "Salgsenhet")?.Value ?? "",
            Omregning:                       int.Parse(x.Element(ns + "Omregning")?.Value ?? "1"),
            SvartTrekant:                    bool.Parse(x.Element(ns + "SvartTrekant")?.Value ?? "false"),
            MvaSats:                         x.Element(ns + "MvaSats")?.Value,
            Parallellimport:                 bool.Parse(x.Element(ns + "Parallellimport")?.Value ?? "false"),
            MFDatoDetaljist:                 x.Element(ns + "MFDatoDetaljist")?.Value ?? "",
            Varestatus:                      x.Element(ns + "Varestatus")?.Value ?? "",
            Fmd:                             bool.Parse(x.Element(ns + "Fmd")?.Value ?? "false"),
            InngarIFest:                     bool.Parse(x.Element(ns + "InngarIFest")?.Value ?? "false"),
            Modelartikkel:                   bool.Parse(x.Element(ns + "Modelartikkel")?.Value ?? "false"),
            Lagerstyringsprofil:             x.Element(ns + "Lagerstyringsprofil")?.Value ?? "",
            Inntektskonto:                   x.Element(ns + "Inntektskonto")?.Value,
            TransportbetingelseKode:         x.Element(ns + "TransportbetingelseKode")?.Value,
            TransportbetingelseBeskrivelse:  x.Element(ns + "TransportbetingelseBeskrivelse")?.Value,
            LegemiddelformNavn:              x.Element(ns + "LegemiddelformNavn")?.Value,
            LegemiddelformKode:              x.Element(ns + "LegemiddelformKode")?.Value,
            OppbevaringsbetingelseDetaljist: x.Element(ns + "OppbevaringsbetingelseDetaljist")?.Value,
            Leverandor:                      x.Element(ns + "Leverandor")?.Value,
            NavnFormStyrke:                  x.Element(ns + "NavnFormStyrke")?.Value,
            Planleggingsmetode:              x.Element(ns + "Planleggingsmetode")?.Value,
            DoseDDD:                         decimal.TryParse(x.Element(ns + "DoseDDD")?.Value, out var d) ? d : null,
            Statistikkfaktor:                decimal.TryParse(x.Element(ns + "Statistikkfaktor")?.Value, out var s) ? s : null,
            Holdbarhet:                      int.TryParse(x.Element(ns + "Holdbarhet")?.Value, out var h) ? h : null,
            UtgaattDato:                     x.Element(ns + "UtgaattDato")?.Value,
            Pakningsinfo:                    null
        )).ToList();
    }

    private static object MapToResource(MdmItem i)
    {
        decimal? mva = Helpers.TryParseDecimalNo(i.MvaSats);

        var ext = new List<object>
        {
            Helpers.ExtString("id",                  i.Id),
            Helpers.ExtString("varebetegnelse",      i.Varebetegnelse),
            Helpers.ExtString("nasjonalKategori",    i.NasjonalKategori),
            Helpers.ExtString("varegruppenr",        i.Varegruppenr),
            Helpers.ExtString("varegruppe",          i.Varegruppe),
            Helpers.ExtString("mellomgruppenr",      i.Mellomgruppenr),
            Helpers.ExtString("mellomgruppe",        i.Mellomgruppe),
            Helpers.ExtString("varetype",            i.Varetype),
            Helpers.ExtString("varekategori",        i.Varekategori),
            Helpers.ExtString("primaerenhet",        i.Primaerenhet),
            Helpers.ExtString("salgsenhet",          i.Salgsenhet),
            Helpers.ExtInt   ("omregning",           i.Omregning),
            Helpers.ExtBool  ("svartTrekant",        i.SvartTrekant),
            Helpers.ExtBool  ("parallellimport",     i.Parallellimport),
            Helpers.ExtString("mfDatoDetaljist",     i.MFDatoDetaljist),
            Helpers.ExtString("varestatusTekst",     i.Varestatus),
            Helpers.ExtBool  ("fmd",                 i.Fmd),
            Helpers.ExtBool  ("inngarIFest",         i.InngarIFest),
            Helpers.ExtBool  ("modelartikkel",       i.Modelartikkel),
            Helpers.ExtString("lagerstyringsprofil", i.Lagerstyringsprofil),
        };

        if (mva is not null)                                              ext.Add(Helpers.ExtDecimal("mvaSats",                        mva));
        if (!string.IsNullOrWhiteSpace(i.Inntektskonto))                  ext.Add(Helpers.ExtString ("inntektskonto",                   i.Inntektskonto));
        if (!string.IsNullOrWhiteSpace(i.TransportbetingelseKode))        ext.Add(Helpers.ExtString ("transportbetingelseKode",         i.TransportbetingelseKode));
        if (!string.IsNullOrWhiteSpace(i.TransportbetingelseBeskrivelse)) ext.Add(Helpers.ExtString ("transportbetingelseBeskrivelse",  i.TransportbetingelseBeskrivelse));
        if (!string.IsNullOrWhiteSpace(i.LegemiddelformNavn))             ext.Add(Helpers.ExtString ("legemiddelformNavn",              i.LegemiddelformNavn));
        if (!string.IsNullOrWhiteSpace(i.LegemiddelformKode))             ext.Add(Helpers.ExtString ("legemiddelformKode",              i.LegemiddelformKode));
        if (!string.IsNullOrWhiteSpace(i.OppbevaringsbetingelseDetaljist))ext.Add(Helpers.ExtString ("oppbevaringsbetingelseDetaljist", i.OppbevaringsbetingelseDetaljist));
        if (!string.IsNullOrWhiteSpace(i.NavnFormStyrke))                 ext.Add(Helpers.ExtString ("navnFormStyrke",                 i.NavnFormStyrke));
        if (!string.IsNullOrWhiteSpace(i.Planleggingsmetode))             ext.Add(Helpers.ExtString ("planleggingsmetode",             i.Planleggingsmetode));

        var ppd = new Dictionary<string, object?>
        {
            ["resourceType"]          = "PackagedProductDefinition",
            ["id"]                    = i.Varenummer,
            ["identifier"]            = new[] { new { system = "https://sykehuspartner.no/varenummer", value = i.Varenummer } },
            ["name"]                  = i.Varenavn,
            ["description"]           = i.VarebetegnelseLang,
            ["containedItemQuantity"] = new[] { new { value = i.Pakningsstorrelse, unit = i.EnhetForKvantum } },
            ["status"]                = new { coding = new[] { new { system = "http://hl7.org/fhir/publication-status", code = "active" } } },
            ["extension"]             = ext
        };

        if (!string.IsNullOrWhiteSpace(i.Leverandor))
            ppd["manufacturer"] = new[] { new { reference = $"Organization/{Helpers.Slug(i.Leverandor)}", display = i.Leverandor } };

        if (!string.IsNullOrWhiteSpace(i.UtgaattDato))
            ppd["marketingStatus"] = new[]
            {
                new {
                    status    = new { coding = new[] { new { system = "http://terminology.hl7.org/CodeSystem/marketing-status", code = "inactive" } } },
                    dateRange = new { end = i.UtgaattDato }
                }
            };

        if (i.Pakningsinfo is not null)
            ppd["packaging"] = new
            {
                quantity  = 1,
                extension = new[]
                {
                    Helpers.ExtComplex("pakningsinfo", new object[]
                    {
                        Helpers.ExtInt   ("multippel",    i.Pakningsinfo.Multippel),
                        Helpers.ExtString("enhetPakning", i.Pakningsinfo.EnhetPakning),
                        Helpers.ExtInt   ("sortering",    i.Pakningsinfo.Sortering)
                    })
                }
            };

        return ppd;
    }
}
