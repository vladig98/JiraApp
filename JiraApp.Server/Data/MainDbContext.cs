namespace JiraApp.Server.Data;

public class MainDbContext : DbContext
{
    public DbSet<BoardModel> Boards { get; set; }
    public DbSet<ColumnModel> Columns { get; set; }
    public DbSet<TaskModel> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskModel>()
            .Property(p => p.Version)
            .IsRowVersion();
    }

    public override int SaveChanges()
    {
        UpdateAuditData();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        UpdateAuditData();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        UpdateAuditData();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditData();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditData()
    {
        IEnumerable<EntityEntry<BaseModel>> baseModels = ChangeTracker.Entries<BaseModel>();

        foreach (EntityEntry<BaseModel> entry in baseModels)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.Id = Guid.CreateVersion7();
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
