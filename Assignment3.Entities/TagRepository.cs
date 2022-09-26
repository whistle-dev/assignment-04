namespace Assignment3.Entities;

public class TagRepository : ITagRepository
{
    (Response Response, int TagId) Create(TagCreateDTO tag)
    {
        return (Response.Created, tag.Name)
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
        return null;
    }
    Response Delete(int tagId, bool force = false)
    {
        return null;
    }


}
