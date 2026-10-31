using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using StargateAPI.Middleware;
using StargateAPI.PipelineBehaviors;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) => config
    .WriteTo.Console()
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.File(
        Path.Combine(builder.Environment.ContentRootPath, "logs", "stargate-.log"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: null,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<StargateContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("StarbaseApiDatabase")));

builder.Services.AddMediatR(cfg =>
{
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(RequestPreProcessorBehavior<,>));
    cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly);
});
builder.Services.AddTransient<IRequestPreProcessor<CreatePerson>, CreatePersonPreProcessor>();
builder.Services.AddTransient<IRequestPreProcessor<CreateAstronautDuty>, CreateAstronautDutyPreProcessor>();
builder.Services.AddTransient<IRequestPreProcessor<UpdatePerson>, UpdatePersonPreProcessor>();

var app = builder.Build();
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


