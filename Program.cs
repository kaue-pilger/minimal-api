using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.DTOs;
using minimal_api.Infrastructure.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataBaseContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("StandardConnection")));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (LoginDTO loginDTO) => {

  if (loginDTO.Email == "adm@teste.com" && loginDTO.Senha == "123456") 
    return Results.Ok("Login com sucesso");
  else 
    return Results.Unauthorized();

} );

app.Run();
