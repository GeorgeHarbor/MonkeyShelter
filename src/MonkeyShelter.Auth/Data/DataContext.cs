using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using MonkeyShelter.Auth.Models;

namespace MonkeyShelter.Auth.Data;

public class DataContext(DbContextOptions options) : IdentityDbContext<User, IdentityRole, string>(options)
{
}