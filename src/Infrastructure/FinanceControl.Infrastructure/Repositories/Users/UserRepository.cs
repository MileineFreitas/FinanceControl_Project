using System;
using System.Collections.Generic;
using System.Linq;
using FinanceControl.Domain.Entities.Users;
using FinanceControl.Domain.Interfaces.Repositories.Users;
using FinanceControl.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Infrastructure.Repositories.Users;

public class UserRepository : IUserRepository
{
    private readonly FinanceDbContext _context;

    public UserRepository(FinanceDbContext context)
    {
        _context = context;
    }

    public IEnumerable<User> GetAllUsers()
    {
        return _context.Users.AsNoTracking().ToList();
    }

    public User GetUserById(int id)
    {
        var user = _context.Users.AsNoTracking().FirstOrDefault(c => c.UserId == id);
        if (user == null)
            throw new KeyNotFoundException($"Usuario com id:{id} não encontrado");

        return user;
    }

    public User CreateUser(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
        return user;
    }

    public User UpdateUser(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        _context.Entry(user).State = EntityState.Modified;
        _context.SaveChanges();
        return user;
    }

    public User DeleteUser(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        _context.Users.Remove(user);
        _context.SaveChanges();
        return user;
    }

    public User? GetUserByEmailAndPassword(string email, string password)
    {

        return _context.Users
            .FirstOrDefault(u => u.UserEmail == email && u.Password == password);
    }
}
