namespace Assignment3.Core;

public interface ITagRepository
{
    (Response response, int TagId) Create(TagCreateDTO tag);
    IReadOnlyCollection<TagDTO> ReadAll();
    TagDTO Read(int tagId);
    Response Update(TagUpdateDTO tag);
    Response Delete(int tagId, bool force = false);
}