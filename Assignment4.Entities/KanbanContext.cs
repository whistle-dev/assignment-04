namespace Assignment4.Entities;
using Microsoft.EntityFrameworkCore;

public class KanbanContext : DbContext
{
    public KanbanContext(DbContextOptions<KanbanContext> options) : base(options)
    {

    }

    public virtual DbSet<Tag> tags => Set<Tag>();
    public virtual DbSet<User> users => Set<User>();
    public virtual DbSet<Task> tasks => Set<Task>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Task>().Property(t => t.State).HasConversion(
                v => v.ToString(),
                v => (State)Enum.Parse(typeof(State), v));

    }

}
