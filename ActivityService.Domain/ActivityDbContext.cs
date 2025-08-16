using ActivityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ActivityService.Domain;

public class ActivityDbContext : DbContext
{
    public ActivityDbContext(DbContextOptions<ActivityDbContext> options)
        : base(options) { }

    public DbSet<Activity> Activities => Set<Activity>();
}