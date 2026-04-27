public record MdmItem(
    string Id,
    string Varenummer,
    string Varebetegnelse,
    string VarebetegnelseLang,
    string Varenavn,
    string NasjonalKategori,
    string Varegruppenr,
    string Varegruppe,
    string Mellomgruppenr,
    string Mellomgruppe,
    string Varetype,
    string Varekategori,
    int Pakningsstorrelse,
    string EnhetForKvantum,
    string Primaerenhet,
    string Salgsenhet,
    int Omregning,
    bool SvartTrekant,
    string? MvaSats,
    bool Parallellimport,
    string MFDatoDetaljist,
    string Varestatus,
    bool Fmd,
    bool InngarIFest,
    bool Modelartikkel,
    string Lagerstyringsprofil,
    string? Inntektskonto,
    string? TransportbetingelseKode,
    string? TransportbetingelseBeskrivelse,
    string? LegemiddelformNavn,
    string? LegemiddelformKode,
    string? OppbevaringsbetingelseDetaljist,
    string? Leverandor,
    string? NavnFormStyrke,
    string? Planleggingsmetode,
    decimal? DoseDDD,
    decimal? Statistikkfaktor,
    int? Holdbarhet,
    string? UtgaattDato,
    Pakningsinfo? Pakningsinfo
);

public record Pakningsinfo(int Multippel, string EnhetPakning, int Sortering);

public record AgreementItem(
    string Id,
    string Varenummer,
    string LeverandorensInterneVarenummer,
    int Prioritet,
    decimal PrisSalgsenhet,
    decimal PrisPrimaerenhet,
    string Avtaleenhet,
    int? Detaljistpakningsstorrelse,
    decimal AvtaleAip,
    decimal? AnbefaltAup,
    string Organisasjonsnummer,
    string Avtalenummer,
    string Avtaleniva,
    string Avtalenavn,
    string Avtalebeskrivelse,
    bool LISAvtale,
    string Avtaletype,
    string ValutaKode,
    string AvtaleStartdato,
    string AvtaleSluttdato,
    string AvtalelinjeStartdato,
    string AvtalelinjeSluttdato,
    int? Ledetid,
    string? FargekodeSortimentsstatus,
    string? FargekodeSortiment,
    int? FargekodeByttegruppeprioritet,
    List<Kampanje> Kampanjer,
    List<string> BildeLinker
);

public record Kampanje(
    string Kampanjenavn,
    string Kampanjetype,
    string KampanjeStartDato,
    string KampanjeSluttDato
);

public record ItemGroupItem(
    string Code,
    string Name_no,
    string? Description_no,
    string? Name_nn,
    string? Description_nn,
    string? Name_en,
    string? Description_en
);

public record AtcCodeItem(
    string Code,
    string Name_no,
    string? Description_no,
    string? Name_nn,
    string? Description_nn,
    string? Name_en,
    string? Description_en
);

public record ColourCodeItem(
    string Id,
    string Varenummer,
    string Avtaleenhet,
    string Avtalenummer,
    string? FargekodeSortimentsstatus,
    string? FargekodeSortiment,
    int? FargekodeByttegruppeprioritet
);
