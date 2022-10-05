namespace Assignment4.Core;

public interface IWorkItemRepository
{
    (Response Response, int WorkItemId) Create(WorkItemCreateDTO workitem);
    IReadOnlyCollection<WorkItemDTO> ReadAll();
    IReadOnlyCollection<WorkItemDTO> ReadAllRemoved();
    IReadOnlyCollection<WorkItemDTO> ReadAllByTag(string tag);
    IReadOnlyCollection<WorkItemDTO> ReadAllByUser(int userId);
    IReadOnlyCollection<WorkItemDTO> ReadAllByState(State state);
    WorkItemDetailsDTO Read(int workitemId);
    Response Update(WorkItemUpdateDTO workitem);
    Response Delete(int workitemId);
}
