using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Interfaces;
using minimal_api.Domain.Services;
using minimal_api.Infrastructure.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdminService, AdminService>();

builder.Services.AddDbContext<DataBaseContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("StandardConnection")));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) => {

  if (adminService.Login(loginDTO) != null) 
    return Results.Ok("Success");
  else 
    return Results.Unauthorized();

} );

app.Run();
