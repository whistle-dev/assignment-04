namespace Assignment4.Entities;


public class WorkItem
{
    public int Id { get; set; }
    public DateTime Created;

    public string? AssignedToName { get; set; }
    public DateTime StateUpdated;
    [StringLength(100)]
    [Required]
    public string? Title { get; set; }
    [Required]
    public State State;
    [StringLength(int.MaxValue)]
    public string? Description { get; set; }
    public virtual ICollection<Tag> Tags { get; set; }

    public WorkItem()
    {
        this.Tags = new HashSet<Tag>();
    }

    public WorkItem(string title)
    {
        this.Tags = new HashSet<Tag>();
        Title = title;
    }
}



