namespace Assignment3.Entities;

public class TaskRepository : ITaskRepository
{
    private readonly KanbanContext _context;

    public TaskRepository(KanbanContext context)
    {
        _context = context;
    }

    public (Response Response, int TaskId) Create(TaskCreateDTO task) {
        var entity = _context.tasks.FirstOrDefault(t => t.Title == task.Title);
        
        Response response;

       if (entity is null) {
            entity = new Task(task.Title);

            _context.tasks.Add(entity);
            _context.SaveChanges();

            response = Response.Created;
       } else {
            response = Response.Conflict;
        } 

        var created = new TaskDTO(entity.Id, entity.Title, entity.AssignedToName, entity.Tags, entity.State);
       
       return (response, created);
    }
    public IReadOnlyCollection<TaskDTO> ReadAll() {
       //return _context.tasks.Select(u => new TaskDTO(u.Id, u.Title, u.AssignedToName, u.Tags, u.State)).ToList();

        var tasks = from t in _context.tasks
                   orderby t.Title
                   select new TaskDTO(t.Id, t.Title, t.AssignedToName, t.Tags, t.State);

        return tasks.ToArray();
        
    }

    public TaskDetailsDTO Read(int taskId) {
        var entity = _context.tasks.FirstOrDefault(t => t.Id == taskId);

        if (entity is null) {
            return null;
        }

        return new TaskDetailsDTO(entity.Id, entity.Title, entity.Description, entity.AssignedToName, entity.Tags, entity.State, entity.Created, entity.Updated);
    }

    public IReadOnlyCollection<TaskDTO> ReadAllRemoved() {
        return from t in _context.tasks
                   where t.State == State.Removed
                   orderby t.Title
                   select new TaskDTO(t.Id, t.Title, t.AssignedToName, t.Tags, t.State);
        
        //return _context.tasks.Where(t => t.State == State.Removed).Select(u => new TaskDTO(u.Id, u.Title, u.AssignedToName, u.Tags, u.State)).ToList();
    }
    public IReadOnlyCollection<TaskDTO> ReadAllByTag(string tag) {
        return from t in _context.tasks
                    where t.Tags.Contains(tag)
                    orderby t.Title
                    select new TaskDTO(t.Id, t.Title, t.AssignedToName, t.Tags, t.State);
    }

    public IReadOnlyCollection<TaskDTO> ReadAllByUser(int userId) {
       return from t in _context.tasks
                    where t.Id == userId
                    orderby t.Title
                    select new TaskDTO(t.Id, t.Title, t.AssignedToName, t.Tags, t.State);
    }
    public IReadOnlyCollection<TaskDTO> ReadAllByState(State state) {
       return from t in _context.tasks
                    where t.State == state
                    orderby t.Title
                    select new TaskDTO(t.Id, t.Title, t.AssignedToName, t.Tags, t.State);
        
    }
    
    public Response Update(TaskUpdateDTO task) {
        var entity = _context.tasks.FirstOrDefault(t => t.Id == task.Id);
        Response response;

        if (entity is null) {
            response = NotFound;
        }
        else if (_context.tasks.FirstOrDefault(t => t.Title == task.Title && t.Id != task.Id ) != null) {
            response = Conflict;
        }
        else {
            entity.Title = task.Title;
            _context.SaveChanges();

            response = Updated;
        }
        return response;
    }
    public Response Delete(int taskId) {
        var task = _context.tasks.Include(t => t.AssignedToName).FirstOrDefault(t => t.Id == taskId);

        Response response;

        if (task is null) {
            response = NotFound;
        }
        else if (task.AssignedToName.Any()) {
            response = Conflict;
        }
        else {
            _context.tasks.Remove(task);
            _context.SaveChanges();

            response = Deleted;
        }
        return response;
    }

}
