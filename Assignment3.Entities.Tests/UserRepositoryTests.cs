namespace Assignment3.Entities.Tests;


public class UserRepositoryTests
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
        context.Database.EnsureCreated();
        context.users.AddRange(new User() { Id = 1, Name = "Test1", Email = "test@test.dk"},
                               new User() { Id = 2, Name = "Test2", Email = "test2@test.dk"});
        context.SaveChanges();

        
        _context = context;
        _repository = new UserRepository(_context);

    }

    [Fact]
    public void Create_given_Tag_returns_Created_with_Tag()
    {
        
        var (response, userId) = _repository.Create(new UserCreateDTO("Test1", "test@test.dk"));
        
        
        Assert.Equal(Response.Created, response);
        Assert.Equal(2, userId);
    }

    [Fact]
    public void Create_given_existing_Tag_returns_Conflict()
    {
        var (response, userId) = _repository.Create(new UserCreateDTO("Test", "test@test.dk"));
        Assert.Equal(Response.Conflict, response);
        
    }

    [Fact]
    public void Find_given_non_existing_id_returns_null()
    {
        var user = _repository.Read(42);
        Assert.Null(user);
    }









}
