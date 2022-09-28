namespace Assignment3.Entities.Tests;

public class TaskRepositoryTests : IDisposable
{
    private readonly KanbanContext _context;
    private readonly TaskRepository _repository;

    public TaskRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.tasks.Add(new Task { Id = 1, Title = "Test", State = State.Active });
        context.SaveChanges();
        context.Database.EnsureCreated();
        _context = context;
        _repository = new TaskRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
