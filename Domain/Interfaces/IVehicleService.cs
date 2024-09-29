using minimal_api.Domain.Entities;

namespace minimal_api.Domain.Interfaces;

public interface IVehicleService
{
    void Create (Vehicle vehicle);
    List<Vehicle> ReadAll();
    Vehicle? Read(int id);
    void Update (Vehicle vehicle);
    void Delete (Vehicle vehicle);
}
