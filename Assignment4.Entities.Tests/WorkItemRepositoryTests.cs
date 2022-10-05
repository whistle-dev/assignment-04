namespace Assignment4.Entities.Tests;

public class WorkItemRepositoryTests
{
    private readonly KanbanContext _context;
    private readonly WorkItemRepository _repository;

    public WorkItemRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<KanbanContext>();
        builder.UseSqlite(connection);
        var context = new KanbanContext(builder.Options);
        context.Database.EnsureCreated();
        context.workitems.Add(new WorkItem { Id = 1, Title = "WorkItem 1", Description = "Description 1", Created = DateTime.Now, State = State.New });
        context.workitems.Add(new WorkItem { Id = 2, Title = "WorkItem 2", Description = "Description 2", Created = DateTime.Now, State = State.Resolved });
        context.workitems.Add(new WorkItem { Id = 3, Title = "WorkItem 3", Description = "Description 3", Created = DateTime.Now, State = State.Active });
        context.workitems.Add(new WorkItem { Id = 4, Title = "WorkItem 4", Description = "Description 4", Created = DateTime.Now, State = State.Closed });
        context.workitems.Add(new WorkItem { Id = 5, Title = "WorkItem 5", Description = "Description 5", Created = DateTime.Now, State = State.Removed });
        var user = new User { Id = 1, Name = "Jakob Storskov", Email = "jakst@itu.dk" };
        context.workitems.Add(new WorkItem { Id = 6, Title = "WorkItem 6", Description = "Description 6", Created = DateTime.Now, AssignedToName = user.Name, State = State.New });
        context.workitems.Add(new WorkItem { Id = 7, Title = "WorkItem 7", Description = "Description 7", Created = DateTime.Now, State = State.Active });
        context.users.Add(user);
        context.SaveChanges();

        _context = context;
        _repository = new WorkItemRepository(_context);
    }

    [Fact]
    public void WorkItem_With_State_Equalto_New_Should_Be_Deletable()
    {
        var response = _repository.Delete(1);
        response.Should().Be(Response.Deleted);
    }
    [Fact]
    public void WorkItem_With_State_Equalto_Resolved_Should_Return_Conflicted_Response_When_Deleted()
    {
        var response = _repository.Delete(2);
        response.Should().Be(Response.Conflict);
    }
    [Fact]
    public void WorkItem_With_State_Equalto_Closed_Should_Return_Conflicted_Response_When_Deleted()
    {
        var response = _repository.Delete(4);
        response.Should().Be(Response.Conflict);
    }
    [Fact]
    public void WorkItem_With_State_Equalto_Removed_Should_Return_Conflicted_Response_When_Deleted()
    {
        var response = _repository.Delete(5);
        response.Should().Be(Response.Conflict);
    }
    [Fact]
    public void Deleteing_WorkItem_With_Active_State_Should_Set_State_To_Removed()
    {
        _repository.Delete(7);
        var workitem = _repository.Read(7);
        workitem.State.Should().Be(State.Removed);
    }
    [Fact]
    public void Assigning_Active_User_To_WorkItem_Should_Return_BadRequest_Response()
    {
        var workitem = _repository.Read(7);
        var response = _repository.Update(new WorkItemUpdateDTO(workitem.Id, workitem.Title, 17, workitem.Description, new string[] { }, workitem.State));
        response.Should().Be(Response.BadRequest);
    }
    [Fact]
    public void Newly_Created_WorkItem_Should_Have_State_Equal_To_New()
    {
        var response = _repository.Create(new WorkItemCreateDTO("Newly created workitem", null, null, new List<string>()));
        var workitem = _repository.Read(response.WorkItemId);
        workitem.State.Should().Be(State.New);
    }
    [Fact]
    public void Newly_Created_WorkItem_Should_Have_AlmostEqual_Stateupdated_and_Created_Times()
    {
        var response = _repository.Create(new WorkItemCreateDTO("Newly created workitem 2", null, null, new List<string>()));
        var workitem = _repository.Read(response.WorkItemId);
        var expected = DateTime.UtcNow;
        workitem.Created.Should().BeCloseTo(expected, precision: TimeSpan.FromSeconds(5));
        workitem.StateUpdated.Should().BeCloseTo(expected, precision: TimeSpan.FromSeconds(5));
    }
    [Fact]
    public void Updating_WorkItem_Should_Set_Stateupdated_To_CurrentTime()
    {
        var workitem = _repository.Read(7);
        _repository.Update(new WorkItemUpdateDTO(workitem.Id, workitem.Title, 1, workitem.Description, new string[] { }, workitem.State)); //Assigning workitem 7 to user id 1
        var updatedWorkItem = _repository.Read(workitem.Id);
        var expectedTime = DateTime.UtcNow;
        updatedWorkItem.StateUpdated.Should().BeCloseTo(expectedTime, precision: TimeSpan.FromSeconds(5));
    }
}
