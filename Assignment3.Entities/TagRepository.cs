namespace Assignment3.Entities;

public sealed class TagRepository : ITagRepository
{
    private readonly KanbanContext _context;

    public TagRepository(KanbanContext context)
    {
        _context = context;
    }

    public (Response response, int TagId) Create(TagCreateDTO tag)
    {
        var entity = _context.tags.FirstOrDefault(t => t.Name == tag.Name);
        Response response;

        if (entity is null)
        {
            entity = new Tag(tag.Name);

            _context.tags.Add(entity);
            _context.SaveChanges();

            response = Response.Created;
        }
        else
        {
            response = Response.Conflict;
        }

        return (response, entity.Id);
    }
    public IReadOnlyCollection<TagDTO> ReadAll()
    {
        var tags = from t in _context.tags
                   orderby t.Name
                   select new TagDTO(t.Id, t.Name);

        return tags.ToArray();
    }
    public TagDTO Read(int tagId)
    {
        if (tagId == null)
        {
            return null;
        }
        var tags = from t in _context.tags
                   where t.Id == tagId
                   select new TagDTO(t.Id, t.Name);

        return tags.FirstOrDefault();

    }
    public Response Update(TagUpdateDTO tag)
    {
        var entity = _context.tags.Find(tag.Id);
        Response response;

        if (entity is null)
        {
            response = Response.NotFound;
        }
        else if (_context.tags.FirstOrDefault(t => t.Id != tag.Id && t.Name == tag.Name) != null)
        {
            response = Response.Conflict;
        }
        else
        {
            entity.Name = tag.Name;
            _context.SaveChanges();
            response = Response.Updated;
        }

        return response;

    }
    public Response Delete(int tagId, bool force = false)
    {
        var tag = _context.tags.Include(t => t.Tasks).FirstOrDefault(t => t.Id == tagId);
        Response response;
        if (tag == null)
        {
            response = Response.NotFound;
        }
        else if (force && tag.Tasks.Any())
        {
            _context.tags.Remove(tag);
            _context.SaveChanges();

            response = Response.Deleted;
            
        }
        else if (!force && tag.Tasks.Any())
        {
            response = Response.Conflict;
        }
        else
        {
            _context.tags.Remove(tag);
            _context.SaveChanges();

            response = Response.Deleted;
        }

        return response;
    }


}
