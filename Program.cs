using simulador_de_banco.Application.Interface.IInfrastructure.Persistence;
using simulador_de_banco.Application.Interface.IInfrastructure.Repository;
using simulador_de_banco.Application.Interface.IServices;
using simulador_de_banco.Application.Services;
using simulador_de_banco.Infrastructure.Persistence;
using simulador_de_banco.Infrastructure.Repostory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IContaCorrenteService,ContaCorrenteService>();
builder.Services.AddScoped<ITransacoesRepository, TransacoesRepository>();

builder.Services.AddScoped<SqlUnitOfWork>();

builder.Services.AddScoped<ISqlUnitOfWork>(provider => provider.GetRequiredService<SqlUnitOfWork>());
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
