namespace Assignment4.Entities;

public class TaskRepository : ITaskRepository
{
    private readonly KanbanContext _context;

    public TaskRepository(KanbanContext context)
    {
        _context = context;
    }

    public (Response Response, int TaskId) Create(TaskCreateDTO task)
    {
        var entity = _context.tasks.FirstOrDefault(t => t.Title == task.Title);

        Response response;



        if (entity is null)
        {
            entity = new Task(task.Title);

            entity.State = State.New;
            entity.Created = DateTime.UtcNow;
            entity.StateUpdated = DateTime.UtcNow;

            _context.tasks.Add(entity);
            _context.SaveChanges();

            response = Response.Created;
        }
        else
        {
            response = Response.Conflict;
        }

        if (task.AssignedToId == null)
        {
            response = Response.BadRequest;
        }

        return (response, entity.Id);
    }
    public IReadOnlyCollection<TaskDTO> ReadAll()
    {

        var tasks = from t in _context.tasks
                    orderby t.Title
                    select new TaskDTO(t.Id, (t.Title != null ? t.Title : ""), t.AssignedToName, t.Tags.Select(n => n.Name != null ? n.Name : "").ToList(), t.State);

        return tasks.ToArray();

    }

    public TaskDetailsDTO Read(int taskId)
    {
        var entity = _context.tasks.FirstOrDefault(t => t.Id == taskId);

        if (entity is null)
        {
            return null;
        }

        return new TaskDetailsDTO(entity.Id, (entity.Title != null ? entity.Title : ""), (entity.Description != null ? entity.Description : ""), entity.Created, entity.AssignedToName, entity.Tags.Select(n => n.Name != null ? n.Name : "").ToList(), entity.State, entity.StateUpdated);
    }

    public IReadOnlyCollection<TaskDTO> ReadAllRemoved()
    {
        return (from t in _context.tasks
                where t.State == State.Removed
                orderby t.Title
                select new TaskDTO(t.Id, (t.Title != null ? t.Title : ""), t.AssignedToName, t.Tags.Select(n => n.Name != null ? n.Name : "").ToList(), t.State)).ToArray();

    }
    public IReadOnlyCollection<TaskDTO> ReadAllByTag(string tagName)
    {
        Tag tag = (Tag)(from t in _context.tags
                        where t.Name != null && t.Name == tagName
                        select t);


        return (from t in _context.tasks
                where t.Tags.Contains(tag)
                orderby t.Title
                select new TaskDTO(t.Id, (t.Title != null ? t.Title : ""), t.AssignedToName, t.Tags.Select(n => n.Name != null ? n.Name : "").ToList(), t.State)).ToArray();
    }

    public IReadOnlyCollection<TaskDTO> ReadAllByUser(int userId)
    {
        return (from t in _context.tasks
                where t.Id == userId
                orderby t.Title
                select new TaskDTO(t.Id, (t.Title != null ? t.Title : ""), t.AssignedToName, t.Tags.Select(n => n.Name != null ? n.Name : "").ToList(), t.State)).ToArray();
    }

    public IReadOnlyCollection<TaskDTO> ReadAllByState(State state)
    {
        return (from t in _context.tasks
                where t.State == state
                orderby t.Title
                select new TaskDTO(t.Id, (t.Title != null ? t.Title : ""), t.AssignedToName, t.Tags.Select(n => n.Name != null ? n.Name : "").ToList(), t.State)).ToArray();


    }

    public Response Update(TaskUpdateDTO task)
    {
        var entity = _context.tasks.FirstOrDefault(t => t.Id == task.Id);
        Response response;

        if (entity is null)
        {
            response = NotFound;
        }
        else if (_context.users.Find(task.AssignedToId) == null)
        {
            response = BadRequest;
        }
        else if (_context.tasks.FirstOrDefault(t => t.Title == task.Title && t.Id != task.Id) != null)
        {
            response = Conflict;
        }
        else
        {
            entity.Title = task.Title;
            entity.StateUpdated = DateTime.UtcNow;
            _context.SaveChanges();

            response = Updated;
        }
        return response;
    }
    public Response Delete(int taskId)
    {
        var task = _context.tasks.Find(taskId);
        Response response = NotFound;

        if (task is null)
        {
            response = NotFound;
        }
        else if (task.State == State.New)
        {

            _context.tasks.Remove(task);
            response = Deleted;

        }
        else if (task.State == State.Active)
        {

            task.State = State.Removed;
            response = Deleted;

        }
        else if (task.State == State.Resolved || task.State == State.Closed || task.State == State.Removed)
        {

            response = Conflict;
        }
        _context.SaveChanges();
        return response;
    }

}
