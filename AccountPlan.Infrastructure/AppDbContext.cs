using Microsoft.EntityFrameworkCore;
using AccountPlan.Domain.Entities;

namespace uCondo.AccountPlan.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AccountPlanEntity> AccountPlans => Set<AccountPlanEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountPlanEntity>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Code).IsUnique();
            builder.HasMany(x => x.Children).WithOne(x => x.Parent).HasForeignKey(x => x.ParentId);
        });
    }
}