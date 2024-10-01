using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Enums;
using minimal_api.Domain.Interfaces;
using minimal_api.Domain.ModelViews;
using minimal_api.Domain.Services;
using minimal_api.Infrastructure.Db;

#region Builder
  var builder = WebApplication.CreateBuilder(args);
  var key = builder.Configuration.GetSection("Jwt").ToString();
  if (string.IsNullOrEmpty(key)) key = "123456";

  builder.Services.AddAuthentication(option => {
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
  }).AddJwtBearer(option => {
    option.TokenValidationParameters = new TokenValidationParameters {
      ValidateLifetime = true,
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
      ValidateIssuer = false,
      ValidateAudience = false
    };
  });

  builder.Services.AddAuthorization();

  builder.Services.AddEndpointsApiExplorer();
  builder.Services.AddSwaggerGen();
  builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme 
    {
      Name = "Authorization",
      Type = SecuritySchemeType.Http,
      Scheme = "bearer",
      BearerFormat = "JWT",
      In = ParameterLocation.Header,
      Description = "Put JWT Token here:"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement 
    {
      {
        new OpenApiSecurityScheme {
          Reference =  new OpenApiReference 
          {
            Type =  ReferenceType.SecurityScheme,
            Id = "Bearer"
          }
        },
        new string[] {}
      }
    });
  });

  builder.Services.AddScoped<IAdminService, AdminService>();
  builder.Services.AddScoped<IVehicleService, VehicleService>();

  builder.Services.AddDbContext<DataBaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StandardConnection")));

  var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
#endregion

#region Admin

  string CreatesJWT(Admin admin) {
    if(string.IsNullOrEmpty(key)) return string.Empty; 
    
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)); 
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>(){
      new Claim("Email", admin.Email),
      new Claim("Profile", admin.Profile),
    };

    var token = new JwtSecurityToken (
      claims: claims,
      expires: DateTime.Now.AddDays(1),
      signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }

  static ValidationErrors validatesAdminDTO (AdminDTO adminDTO) {
    var validation = new ValidationErrors {
      Messages = new List<string>()
    };

    if (string.IsNullOrEmpty(adminDTO.Email))
      validation.Messages.Add("Email must be filled.");

    if (string.IsNullOrEmpty(adminDTO.Password))
      validation.Messages.Add("Password must be filled.");

    if (string.IsNullOrEmpty(adminDTO.Profile.ToString()))
      validation.Messages.Add("Profile must be filled.");

    return validation;
  }

  app.MapPost("/admin", ([FromBody] AdminDTO adminDTO, IAdminService adminService) => {

    var validation = validatesAdminDTO(adminDTO);
    if (validation.Messages.Count > 0) return Results.BadRequest(validation);

    var adminCreated = new Admin{
      Email = adminDTO.Email,
      Password = adminDTO.Password,
      Profile = adminDTO.Profile.ToString() ?? Profile.editor.ToString()
    };

    adminService.Create(adminCreated);

    return Results.Created($"/vehicle/{adminCreated.Id}", new AdminMV{
      Id = adminCreated.Id,
      Email = adminCreated.Email,
      Profile = adminCreated.Profile,
    });

  }).RequireAuthorization().WithTags("Admins");

  app.MapGet("/admins", (IAdminService adminService) => {
    var adminsView = new List<AdminMV>();
    var admins = adminService.ReadAll();

    foreach (var adm in admins)
    {
      adminsView.Add(new AdminMV{
        Id = adm.Id,
        Email = adm.Email,
        Profile = adm.Profile,
      });
    }

    return Results.Ok(adminsView);

  }).RequireAuthorization().WithTags("Admins");

  app.MapGet("/admin/{id}", ([FromRoute] int id, IAdminService adminService) => {
    var admin = adminService.Read(id);
    if (admin == null) return Results.NotFound();
  
    return Results.Ok(new AdminMV{
      Id = admin.Id,
      Email = admin.Email,
      Profile = admin.Profile,
    });
  }).RequireAuthorization().WithTags("Admins");

  app.MapPut("/admin/{id}", ([FromRoute] int id, AdminDTO adminDTO, IAdminService adminService) => {
    var admin = adminService.Read(id);
    if (admin == null) return Results.NotFound();

    var validation = validatesAdminDTO(adminDTO);
    if (validation.Messages.Count > 0) return Results.BadRequest(validation);

    admin.Email = adminDTO.Email;
    admin.Password = adminDTO.Password;
    admin.Profile = adminDTO.Profile.ToString() ?? Profile.editor.ToString();

    adminService.Update(admin);

    return Results.Ok(new AdminMV{
      Id = admin.Id,
      Email = admin.Email,
      Profile = admin.Profile,
    });
  }).RequireAuthorization().WithTags("Admins");

  app.MapDelete("/admin/{id}", ([FromRoute] int id, IAdminService adminService) => {
    var admin = adminService.Read(id);
    if (admin == null) return Results.NotFound();

    adminService.Delete(admin);

    return Results.NoContent();
  }).RequireAuthorization().WithTags("Admins");

  app.MapPost("/admin/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) => {
    var admin = adminService.Login(loginDTO);
    if (admin != null) {
      string token = CreatesJWT(admin);
      return Results.Ok(new AdminLoged {
        Email = admin.Email,
        Profile = admin.Profile,
        Token = token
      });
    } 
    else 
      return Results.Unauthorized();

  }).AllowAnonymous().WithTags("Admins");
#endregion

#region Vehicle

  static ValidationErrors validatesVehicleDTO (VehicleDTO vehicleDTO) {
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

    var validation = validatesVehicleDTO(vehicleDTO);
    if (validation.Messages.Count > 0) return Results.BadRequest(validation);

    var vehicleCreated = new Vehicle{
      Name = vehicleDTO.Name,
      Brand = vehicleDTO.Brand,
      Year = vehicleDTO.Year
    };

    vehicleService.Create(vehicleCreated);

    return Results.Created($"/vehicle/{vehicleCreated.Id}", vehicleCreated);

  }).RequireAuthorization().WithTags("Vehicles");

  app.MapGet("/vehicles", (IVehicleService vehicleService) => {
    var vehicles = vehicleService.ReadAll();

    return Results.Ok(vehicles);

  }).RequireAuthorization().WithTags("Vehicles");

  app.MapGet("/vehicle/{id}", ([FromRoute] int id, IVehicleService vehicleService) => {
    var vehicle = vehicleService.Read(id);
    if (vehicle == null) return Results.NotFound();

    return Results.Ok(vehicle);
  }).RequireAuthorization().WithTags("Vehicles");

  app.MapPut("/vehicle/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) => {
    var vehicle = vehicleService.Read(id);
    if (vehicle == null) return Results.NotFound();

    var validation = validatesVehicleDTO(vehicleDTO);
    if (validation.Messages.Count > 0) return Results.BadRequest(validation);

    vehicle.Name = vehicleDTO.Name;
    vehicle.Brand = vehicleDTO.Brand;
    vehicle.Year = vehicleDTO.Year;

    vehicleService.Update(vehicle);

    return Results.Ok(vehicle);
  }).RequireAuthorization().WithTags("Vehicles");

  app.MapDelete("/vehicle/{id}", ([FromRoute] int id, IVehicleService vehicleService) => {
    var vehicle = vehicleService.Read(id);
    if (vehicle == null) return Results.NotFound();

    vehicleService.Delete(vehicle);

    return Results.NoContent();
  }).RequireAuthorization().WithTags("Vehicles");

#endregion

#region Swagger
app.UseSwagger();
app.UseSwaggerUI();
#endregion

#region JWT
  app.UseAuthentication();
  app.UseAuthorization();
#endregion

app.Run();
