using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;

namespace minimal_api.Domain.Interfaces;

public interface IAdminService
{
    void Create (Admin admin);
    List<Admin> ReadAll();
    Admin? Read(int id);
    void Update (Admin admin);
    void Delete (Admin admin);
    Admin? Login(LoginDTO loginDTO);
}
