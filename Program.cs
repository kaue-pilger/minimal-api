using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;
using minimal_api.Domain.ModelViews;
using minimal_api.Domain.Services;
using minimal_api.Infrastructure.Db;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddDbContext<DataBaseContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("StandardConnection")));

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home()));
#endregion

#region Admin
app.MapPost("/admin/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) => {

  if (adminService.Login(loginDTO) != null) 
    return Results.Ok("Success");
  else 
    return Results.Unauthorized();

} );
#endregion

#region Vehicle
  app.MapGet("/vehicles", (IVehicleService vehicleService) => {
    var vehicles = vehicleService.ReadAll();

    return Results.Ok(vehicles);

  } );

  app.MapPost("/vehicle", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) => {

    var vehicleCreated = new Vehicle{
      Name = vehicleDTO.Name,
      Brand = vehicleDTO.Brand,
      Year = vehicleDTO.Year
    };

    vehicleService.Create(vehicleCreated);

    return Results.Created($"/vehicle/{vehicleCreated.Id}", vehicleCreated);

  } );
#endregion

#region Swagger
app.UseSwagger();
app.UseSwaggerUI();
#endregion

app.Run();
