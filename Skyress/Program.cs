using Skyress.API.Extenstions;
using Serilog;
using Skyress.API.Endpoints.Items;
using Skyress.API.Endpoints.Customers;
using Skyress.API.Endpoints.Invoices;
using Skyress.API.Endpoints.Payments;
using Skyress.API.Endpoints.Tags;
using Skyress.API.Endpoints.Todos;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDependencies(builder.Configuration);
builder.Host.UseSerilog((context, configuration) => 
    configuration.ReadFrom.Configuration(context.Configuration));


var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.MapItemsApi();
app.MapCustomersApi();
app.MapInvoicesApi();
app.MapPaymentsApi();
app.MapTagsApi();
app.MapTodosApi();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();
        foreach (var description in descriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
        options.RoutePrefix = string.Empty;
    });
}

app.Run();