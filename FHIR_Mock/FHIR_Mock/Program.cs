using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(o =>
{
    o.SerializerOptions.WriteIndented = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title       = "MDM FHIR API",
        Version     = "v1",
        Description = "FHIR PackagedProductDefinition endpoints for MDM articles"
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MDM FHIR API v1");
    c.RoutePrefix = "swagger";
});

app.MapMetadata();
app.MapArticles();
app.MapAgreements();
app.MapItemGroups();
app.MapAtcCodes();
app.MapColourCodes();

app.Run();