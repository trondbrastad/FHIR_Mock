public static class Metadata
{
    public static void MapMetadata(this WebApplication app)
    {
        app.MapGet("/", () => Results.Redirect("/swagger"));

        app.MapGet("/fhir/metadata", () => Results.Json(new
        {
            resourceType = "CapabilityStatement",
            status       = "active",
            date         = DateTimeOffset.Now,
            kind         = "instance",
            fhirVersion  = "5.0.0",
            format       = new[] { "json" },
            rest         = new[]
            {
                new
                {
                    mode     = "server",
                    resource = new[]
                    {
                        new
                        {
                            type        = "PackagedProductDefinition",
                            interaction = new[]
                            {
                                new { code = "read" },
                                new { code = "search-type" }
                            }
                        }
                    }
                }
            }
        }));
    }
}