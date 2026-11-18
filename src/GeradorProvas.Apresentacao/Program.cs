using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Aplicacao;
using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Apresentacao;
using Fsg.ProgramacaoOrientadaObjetos.GeradorProvas.Infraestrutura;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<TratadorDeExcecoes>();

// Repositórios Singleton para as listas sobreviverem entre requisições.
builder.Services.AddSingleton<IRepositorioQuestoes, RepositorioQuestoesEmMemoria>();
builder.Services.AddSingleton<IRepositorioProvas, RepositorioProvasEmMemoria>();
builder.Services.AddSingleton(Random.Shared);
builder.Services.AddScoped<IEmbaralhador, EmbaralhadorAleatorio>();
builder.Services.AddScoped<ServicoCadastroQuestao>();
builder.Services.AddScoped<ServicoConsultaQuestao>();
builder.Services.AddScoped<ServicoGeradorProva>();
builder.Services.AddScoped<ServicoConsultaProva>();

var app = builder.Build();

app.UseExceptionHandler();
app.MapControllers();

app.Run();
