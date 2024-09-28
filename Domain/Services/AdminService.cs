using minimal_api.Infrastructure.Db;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;

namespace minimal_api.Domain.Services;

public class AdminService : IAdminService
{
  private readonly DataBaseContext  _context;
  public AdminService(DataBaseContext dbContext)
  {
    _context = dbContext;
  }
  public Admin Login(LoginDTO loginDTO)
  {
    return _context.Admins.Where(admin => admin.Email == loginDTO.Email && admin.Password == loginDTO.Password).FirstOrDefault();
  }
}