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
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Admin
app.MapPost("/admin/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) => {

  if (adminService.Login(loginDTO) != null) 
    return Results.Ok("Success");
  else 
    return Results.Unauthorized();

}).WithTags("Admins");
#endregion

#region Vehicle

  static ValidationErrors validatesDTO (VehicleDTO vehicleDTO) {
    var validation = new ValidationErrors {
      Messages = new List<string>()
    };

    if (string.IsNullOrEmpty(vehicleDTO.Name))
      validation.Messages.Add("Name must be filled.");

    if (string.IsNullOrEmpty(vehicleDTO.Brand))
      validation.Messages.Add("Brand must be filled.");

    if (vehicleDTO.Year < 1950)
      validation.Messages.Add("Year must be higher.");

    return validation;
  }

  app.MapPost("/vehicle", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) => {

    var validation = validatesDTO(vehicleDTO);
    if (validation.Messages.Count > 0) return Results.BadRequest(validation);

    var vehicleCreated = new Vehicle{
      Name = vehicleDTO.Name,
      Brand = vehicleDTO.Brand,
      Year = vehicleDTO.Year
    };

    vehicleService.Create(vehicleCreated);

    return Results.Created($"/vehicle/{vehicleCreated.Id}", vehicleCreated);

  }).WithTags("Vehicles");

  app.MapGet("/vehicles", (IVehicleService vehicleService) => {
    var vehicles = vehicleService.ReadAll();

    return Results.Ok(vehicles);

  }).WithTags("Vehicles");

  app.MapGet("/vehicle/{id}", ([FromRoute] int id, IVehicleService vehicleService) => {
    var vehicle = vehicleService.Read(id);
    if (vehicle == null) return Results.NotFound();

    return Results.Ok(vehicle);
  }).WithTags("Vehicles");

  app.MapPut("/vehicle/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) => {
    var vehicle = vehicleService.Read(id);
    if (vehicle == null) return Results.NotFound();

    var validation = validatesDTO(vehicleDTO);
    if (validation.Messages.Count > 0) return Results.BadRequest(validation);

    vehicle.Name = vehicleDTO.Name;
    vehicle.Brand = vehicleDTO.Brand;
    vehicle.Year = vehicleDTO.Year;

    vehicleService.Update(vehicle);

    return Results.Ok(vehicle);
  }).WithTags("Vehicles");

  app.MapDelete("/vehicle/{id}", ([FromRoute] int id, IVehicleService vehicleService) => {
    var vehicle = vehicleService.Read(id);
    if (vehicle == null) return Results.NotFound();

    vehicleService.Delete(vehicle);

    return Results.NoContent();
  }).WithTags("Vehicles");

#endregion

#region Swagger
app.UseSwagger();
app.UseSwaggerUI();
#endregion

app.Run();
