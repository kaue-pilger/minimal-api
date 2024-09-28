using minimal_api.Infrastructure.Db;
using minimal_api.Domain.Interfaces;
using minimal_api.Domain.Entities;

namespace minimal_api.Domain.Services;

public class VehicleService : IVehicleService
{
  private readonly DataBaseContext  _context;
  public VehicleService(DataBaseContext dbContext)
  {
    _context = dbContext;
  }

    public void Create(Vehicle vehicle)
    {
      _context.Vehicles.Add(vehicle);
      _context.SaveChanges();
    }

    public void Delete(Vehicle vehicle)
    {
      _context.Vehicles.Remove(vehicle);
      _context.SaveChanges();
    }

    public Vehicle? Read(int id)
    {
      return _context.Vehicles.Find(id);
    }

    public List<Vehicle> ReadAll()
    {
      return _context.Vehicles.ToList();
    }

    public void Update(Vehicle vehicle)
    {
      _context.Vehicles.Update(vehicle);
      _context.SaveChanges();
    }
}