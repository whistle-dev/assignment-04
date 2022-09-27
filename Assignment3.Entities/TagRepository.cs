namespace Assignment3.Entities;

public class TagRepository : ITagRepository
{
    private readonly KanbanContext _context;

    public TagRepository(KanbanContext context)
    {
        _context = context;
    }
    (Response response, int TagId) Create(TagCreateDTO tag)
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
    IReadOnlyCollection<TagDTO> ReadAll()
    {
        return null;
    }
    TagDTO Read(int tagId)
    {
        return null;
    }
    Response Update(TagUpdateDTO tag)
    {
        var entity = _context.tags.Find(tag.Id);
        Response response;
    }
    Response Delete(int tagId, bool force = false)
    {
        return null;
    }


}
