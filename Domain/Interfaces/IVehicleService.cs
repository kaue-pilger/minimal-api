using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Domain.DTOs;
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
