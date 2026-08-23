using simulador_de_banco.Application.Interface.IInfrastructure.Integration.Antifraude;
using simulador_de_banco.Application.Interface.IInfrastructure.Integration.Email;
using simulador_de_banco.Application.Interface.IInfrastructure.Persistence;
using simulador_de_banco.Application.Interface.IInfrastructure.Repository;
using simulador_de_banco.Application.Interface.IInfrastructure.Storage;
using simulador_de_banco.Application.Interface.IServices;
using simulador_de_banco.Application.Services;
using simulador_de_banco.Infrastructure.Integrations.Antifraude;
using simulador_de_banco.Infrastructure.Integrations.email;
using simulador_de_banco.Infrastructure.Persistence;
using simulador_de_banco.Infrastructure.Repostory;
using simulador_de_banco.Infrastructure.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IContaCorrenteService,ContaCorrenteService>();
builder.Services.AddScoped<ITransacoesServices, TransacoesRepository>();
builder.Services.AddScoped<IAntifraudeServices, AntifraudeClient>();
builder.Services.AddScoped<IEmailServices, EmailClient>();
builder.Services.AddScoped<IExtratoServices, ExtratoFile>();

builder.Services.AddScoped<SqlUnitOfWork>();

builder.Services.AddScoped<ISqlUnitOfWorkServices>(provider => provider.GetRequiredService<SqlUnitOfWork>());
builder.Services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<SqlUnitOfWork>());

builder.Services.AddScoped<HttpClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
