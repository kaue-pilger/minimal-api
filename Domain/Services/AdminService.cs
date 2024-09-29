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

  public void Create(Admin admin)
  {
    _context.Admins.Add(admin);
    _context.SaveChanges();
  }

  public void Delete(Admin admin)
  {
    _context.Admins.Remove(admin);
    _context.SaveChanges();
  }

  public Admin? Read(int id)
  {
    return _context.Admins.Find(id);
  }

  public List<Admin> ReadAll()
  {
    return _context.Admins.ToList();
  }

  public void Update(Admin admin)
  {
    _context.Admins.Update(admin);
    _context.SaveChanges();
  }

  public Admin Login(LoginDTO loginDTO)
  {
    return _context.Admins.Where(admin => admin.Email == loginDTO.Email && admin.Password == loginDTO.Password).FirstOrDefault();
  }
}