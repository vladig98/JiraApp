namespace JiraApp.Server.Data;

public class MainDbContext(DbContextOptions<MainDbContext> options) : DbContext(options)
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
}
