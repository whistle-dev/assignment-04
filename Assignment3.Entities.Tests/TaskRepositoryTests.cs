namespace Assignment3.Entities.Tests;

public class TaskRepositoryTests
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
        context.Database.EnsureCreated();
        context.tasks.Add(new Task { Id = 1, Title = "Task 1", Description = "Description 1", Created = DateTime.Now, State = State.New });
        context.SaveChanges();

        _context = context;
        _repository = new TaskRepository(_context);
    }

}
