using System;
using System.Diagnostics;
namespace Assignment4.Entities.Tests;

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
        context.tags.AddRange(new Tag("High") { Id = 1 }, new Tag("Low") { Id = 2 });
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


        Assert.Equal(Response.Created, response);
        Assert.Equal(3, tagId);
    }

    [Fact]
    public void Create_given_existing_Tag_returns_Conflict()
    {
        var (response, tagId) = _repository.Create(new TagCreateDTO("High"));
        Assert.Equal(Response.Conflict, response);

    }

    [Fact]
    public void Find_given_non_existing_id_returns_null()
    {
        var tag = _repository.Read(42);
        Assert.Null(tag);
    }

    [Fact]
    public void Read_given_existing_id_returns_Tag()
    {
        var tag = _repository.Read(1);
        Assert.Equal("High", tag.Name);
    }

    [Fact]
    public void ReadAll_given_existing_id_returns_Tag()
    {
        var tags = _repository.ReadAll();
        Assert.Equal(2, tags.Count());
    }

}
