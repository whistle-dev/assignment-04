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
        context.tasks.Add(new Task { Id = 2, Title = "Task 2", Description = "Description 2", Created = DateTime.Now, State = State.Resolved });
        context.tasks.Add(new Task { Id = 3, Title = "Task 3", Description = "Description 3", Created = DateTime.Now, State = State.Active });
        context.tasks.Add(new Task { Id = 4, Title = "Task 4", Description = "Description 4", Created = DateTime.Now, State = State.Closed });
        context.tasks.Add(new Task { Id = 5, Title = "Task 5", Description = "Description 5", Created = DateTime.Now, State = State.Removed });
        var user = new User {Id = 1, Name = "Jakob Storskov", Email = "jakst@itu.dk"};
        context.tasks.Add(new Task { Id = 6, Title = "Task 6", Description = "Description 6", Created = DateTime.Now, AssignedToName = user.Name, State = State.New });
        context.tasks.Add(new Task { Id = 7, Title = "Task 7", Description = "Description 7", Created = DateTime.Now, State = State.Active });
        context.users.Add(user);
        context.SaveChanges();

        _context = context;
        _repository = new TaskRepository(_context);
    }

    [Fact]
    public void Task_With_State_Equalto_New_Should_Be_Deletable()
    {
        var response = _repository.Delete(1);
        response.Should().Be(Response.Deleted);
    }
    [Fact]
    public void Task_With_State_Equalto_Resolved_Should_Return_Conflicted_Response_When_Deleted()
    {
        var response = _repository.Delete(2);
        response.Should().Be(Response.Conflict);
    }
    [Fact]
    public void Task_With_State_Equalto_Closed_Should_Return_Conflicted_Response_When_Deleted()
    {
        var response = _repository.Delete(4);
        response.Should().Be(Response.Conflict);
    }
    [Fact]
    public void Task_With_State_Equalto_Removed_Should_Return_Conflicted_Response_When_Deleted()
    {
        var response = _repository.Delete(5);
        response.Should().Be(Response.Conflict);
    }
    [Fact]
    public void Deleteing_Task_With_Active_State_Should_Set_State_To_Removed()
    {
        _repository.Delete(7);
        var task = _repository.Read(7);
        task.State.Should().Be(State.Removed);
    }
    [Fact]
    public void Assigning_Active_User_To_Task_Should_Return_BadRequest_Response()
    {
        var task = _repository.Read(7);
        var response = _repository.Update(new TaskUpdateDTO(task.Id, task.Title, 17, task.Description, new string[] { }, task.State));
        response.Should().Be(Response.BadRequest);
    }
    [Fact]
    public void Newly_Created_Task_Should_Have_State_Equal_To_New()
    {
        var response = _repository.Create(new TaskCreateDTO("Newly created task", null, null, new List<string>()));
        var task = _repository.Read(response.TaskId);
        task.State.Should().Be(State.New);
    }
    [Fact]
    public void Newly_Created_Task_Should_Have_AlmostEqual_Stateupdated_and_Created_Times()
    {
        var response = _repository.Create(new TaskCreateDTO("Newly created task 2", null, null, new List<string>()));
        var task = _repository.Read(response.TaskId);
        var expected = DateTime.UtcNow;
        task.Created.Should().BeCloseTo(expected, precision: TimeSpan.FromSeconds(5));
        task.StateUpdated.Should().BeCloseTo(expected, precision: TimeSpan.FromSeconds(5));
    }
    [Fact]
    public void Updating_Task_Should_Set_Stateupdated_To_CurrentTime()
    {
        var task = _repository.Read(7);
        _repository.Update(new TaskUpdateDTO(task.Id, task.Title, 1, task.Description, new string[] {}, task.State)); //Assigning task 7 to user id 1
        var updatedTask = _repository.Read(task.Id);
        var expectedTime = DateTime.UtcNow;
        updatedTask.StateUpdated.Should().BeCloseTo(expectedTime, precision: TimeSpan.FromSeconds(5));
    }
}
