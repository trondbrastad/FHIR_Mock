using System.Xml.Linq;

public static class DataLoaders
{
    public static List<MdmItem> LoadItems()
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

    public static List<AgreementItem> LoadAgreements()
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

    public static List<ItemGroupItem> LoadItemGroups()
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

    public static List<AtcCodeItem> LoadAtcCodes()
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

    public static List<ColourCodeItem> LoadColourCodes()
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
}
