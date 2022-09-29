namespace Assignment3.Entities;


public class Task
{
    public int Id {get; set; }
    public DateTime Created;

    public string AssignedToName {get; set;}
    public DateTime StateUpdated;
    [StringLength(100)]
    [Required]
    public string? Title{get; set;}
    [Required]
    public State State;
    [StringLength(int.MaxValue)]
    public string? Description{get; set;}
    public virtual ICollection<Tag> Tags {get; set;}

    public Task()
    {
        this.Tags = new HashSet<Tag>();
    }

    public Task(string title)
    {
        this.Tags = new HashSet<Tag>();
        Title = title;
    }
}



