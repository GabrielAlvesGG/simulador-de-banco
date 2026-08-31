using simulador_de_banco.Application.Interface.IInfrastructure.Persistence;
using simulador_de_banco.Application.Interface.IInfrastructure.Repository;
using simulador_de_banco.Application.Interface.IServices;
using simulador_de_banco.Application.Services;
using simulador_de_banco.Infrastructure.Integrations.AntifraudeClientIntegrations;
using simulador_de_banco.Infrastructure.Integrations.email;
using simulador_de_banco.Infrastructure.Persistence;
using simulador_de_banco.Infrastructure.Repostory;
using simulador_de_banco.Infrastructure.Storage;
using simulador_de_banco.Infrastructure.Persistence.Interface;
using simulador_de_banco.Application.Notification.Interface;
using simulador_de_banco.Application.Antifraude.Interface;
using simulador_de_banco.Application.Extrato.Interface;
using simulador_de_banco.Application.Mapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IContaCorrenteService,ContaCorrenteService>();
builder.Services.AddScoped<ITransacoesServices, TransacoesRepository>();
builder.Services.AddScoped<IAntifraudeServices, AntifraudeClientIntegrations>();
builder.Services.AddScoped<IEmailServices, EmailClient>();
builder.Services.AddScoped<IExtratoServices, ExtratoFile>();
builder.Services.AddScoped<IMapperRequests, MapperRequests>();

builder.Services.AddScoped<simulador_de_banco.Infrastructure.Persistence.UnitOfWork>();

builder.Services.AddScoped((Func<IServiceProvider, IUnitOfWorkServices>)(provider => provider.GetRequiredService<simulador_de_banco.Infrastructure.Persistence.UnitOfWork>()));
builder.Services.AddScoped((Func<IServiceProvider, simulador_de_banco.Infrastructure.Persistence.Interface.ISqlUnitOfWork>)(provider => (simulador_de_banco.Infrastructure.Persistence.Interface.ISqlUnitOfWork)provider.GetRequiredService<simulador_de_banco.Infrastructure.Persistence.UnitOfWork>()));

builder.Services.AddScoped<HttpClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
