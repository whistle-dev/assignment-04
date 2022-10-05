namespace Assignment4.Entities;

public class WorkItemRepository : IWorkItemRepository
{
    private readonly KanbanContext _context;

    public WorkItemRepository(KanbanContext context)
    {
        _context = context;
    }

    public (Response Response, int WorkItemId) Create(WorkItemCreateDTO workitem)
    {
        var entity = _context.workitems.FirstOrDefault(t => t.Title == workitem.Title);

        Response response;



        if (entity is null)
        {
            entity = new WorkItem(workitem.Title);

            entity.State = State.New;
            entity.Created = DateTime.UtcNow;
            entity.StateUpdated = DateTime.UtcNow;

            _context.workitems.Add(entity);
            _context.SaveChanges();

            response = Response.Created;
        }
        else
        {
            response = Response.Conflict;
        }

        if (workitem.AssignedToId == null)
        {
            response = Response.BadRequest;
        }

        return (response, entity.Id);
    }
    public IReadOnlyCollection<WorkItemDTO> ReadAll()
    {

        var workitems = from t in _context.workitems
                        orderby t.Title
                        select new WorkItemDTO(t.Id, (t.Title != null ? t.Title : ""), t.AssignedToName, t.Tags.Select(n => n.Name != null ? n.Name : "").ToList(), t.State);

        return workitems.ToArray();

    }

    public WorkItemDetailsDTO Read(int workitemId)
    {
        var entity = _context.workitems.FirstOrDefault(t => t.Id == workitemId);

        if (entity is null)
        {
            return null;
        }

        return new WorkItemDetailsDTO(entity.Id, (entity.Title != null ? entity.Title : ""), (entity.Description != null ? entity.Description : ""), entity.Created, entity.AssignedToName, entity.Tags.Select(n => n.Name != null ? n.Name : "").ToList(), entity.State, entity.StateUpdated);
    }

    public IReadOnlyCollection<WorkItemDTO> ReadAllRemoved()
    {
        return (from t in _context.workitems
                where t.State == State.Removed
                orderby t.Title
                select new WorkItemDTO(t.Id, (t.Title != null ? t.Title : ""), t.AssignedToName, t.Tags.Select(n => n.Name != null ? n.Name : "").ToList(), t.State)).ToArray();

    }
    public IReadOnlyCollection<WorkItemDTO> ReadAllByTag(string tagName)
    {
        Tag tag = (Tag)(from t in _context.tags
                        where t.Name != null && t.Name == tagName
                        select t);


        return (from t in _context.workitems
                where t.Tags.Contains(tag)
                orderby t.Title
                select new WorkItemDTO(t.Id, (t.Title != null ? t.Title : ""), t.AssignedToName, t.Tags.Select(n => n.Name != null ? n.Name : "").ToList(), t.State)).ToArray();
    }

    public IReadOnlyCollection<WorkItemDTO> ReadAllByUser(int userId)
    {
        return (from t in _context.workitems
                where t.Id == userId
                orderby t.Title
                select new WorkItemDTO(t.Id, (t.Title != null ? t.Title : ""), t.AssignedToName, t.Tags.Select(n => n.Name != null ? n.Name : "").ToList(), t.State)).ToArray();
    }

    public IReadOnlyCollection<WorkItemDTO> ReadAllByState(State state)
    {
        return (from t in _context.workitems
                where t.State == state
                orderby t.Title
                select new WorkItemDTO(t.Id, (t.Title != null ? t.Title : ""), t.AssignedToName, t.Tags.Select(n => n.Name != null ? n.Name : "").ToList(), t.State)).ToArray();


    }

    public Response Update(WorkItemUpdateDTO workitem)
    {
        var entity = _context.workitems.FirstOrDefault(t => t.Id == workitem.Id);
        Response response;

        if (entity is null)
        {
            response = NotFound;
        }
        else if (_context.users.Find(workitem.AssignedToId) == null)
        {
            response = BadRequest;
        }
        else if (_context.workitems.FirstOrDefault(t => t.Title == workitem.Title && t.Id != workitem.Id) != null)
        {
            response = Conflict;
        }
        else
        {
            entity.Title = workitem.Title;
            entity.StateUpdated = DateTime.UtcNow;
            _context.SaveChanges();

            response = Updated;
        }
        return response;
    }
    public Response Delete(int workitemId)
    {
        var workitem = _context.workitems.Find(workitemId);
        Response response = NotFound;

        if (workitem is null)
        {
            response = NotFound;
        }
        else if (workitem.State == State.New)
        {

            _context.workitems.Remove(workitem);
            response = Deleted;

        }
        else if (workitem.State == State.Active)
        {

            workitem.State = State.Removed;
            response = Deleted;

        }
        else if (workitem.State == State.Resolved || workitem.State == State.Closed || workitem.State == State.Removed)
        {

            response = Conflict;
        }
        _context.SaveChanges();
        return response;
    }

}
