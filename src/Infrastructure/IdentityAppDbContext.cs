using AspDotnetBoilerplate.src.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AspDotnetBoilerplate.src.Infrastructure;


public class IdentityAppDbContext(
    DbContextOptions<IdentityAppDbContext> options
    ) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    
}