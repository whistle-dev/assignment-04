namespace Assignment3.Entities.Tests;


public class UserRepositoryTests : IDisposable
{

    private readonly KanbanContext _context;
    private readonly UserRepository _repository;
     public UserRepositoryTests()
    {

        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.users.Add(new User { 
                                    Id = 1, 
                                    Name = "Test", 
                                    Email = "Test@test.dk" });
        context.SaveChanges();

        
        context.Database.EnsureCreated();
        _context = context;
        _repository = new UserRepository(_context);

    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    //test UserRepository.cs with fluent assertions
    [Fact]
    public void CreateUser()
    {
        var user = new UserCreateDTO("Test1", "test@test.dk");
        _repository.Create(user);
        _context.users.Should().Be(Response.Created);
    }








}
