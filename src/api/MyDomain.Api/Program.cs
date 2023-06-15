using MyDomain.Api;
using MyDomain.Application;
using MyDomain.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false);
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure();
    builder.Services.AddProblemDetails();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<LogUnhandledExceptionsMiddleware>();
app.MapControllers();
app.Run();
