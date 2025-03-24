using Serilog;
using SurveyBasket;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration);

builder.Services.AddDistributedMemoryCache();


builder.Host.UseSerilog((context, configuration) =>
{
    //configuration.MinimumLevel.Information().WriteTo.Console();

    configuration.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors(); // Use default CORS policy

app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler(); // Use the global exception handler

app.Run();
