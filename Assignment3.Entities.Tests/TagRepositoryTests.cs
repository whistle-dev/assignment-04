namespace Assignment3.Entities.Tests;

public sealed class TagRepositoryTests
{
    private readonly KanbanContext _context;
    private readonly TagRepository _repository;

    public TagRepositoryTests()
    {
        var connection = new SqliteConnection("filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();
        context.tags.AddRange(new Tag("High") { Id = 1}, new Tag("Low") { Id = 2});
        context.tasks.Add(new Task
                          {
                          Id = 1, 
                          Created = DateTime.Now, 
                          AssignedToName = "High", 
                          Title = "Make", 
                          State = State.New, 
                          Description = "Make a new program", 
                          Tags = context.tags.ToArray()
                          });
        context.SaveChanges();

        _context = context;
        _repository = new TagRepository(_context);

    }

    [Fact]
    public void Create_given_Tag_returns_Created_with_Tag()
    {
        var (response, tagId) = _repository.Create(new TagCreateDTO("Medium"));

        response.Should().Be(Response.Created);
        
        tagId.Should().Be(3);
    }

}
