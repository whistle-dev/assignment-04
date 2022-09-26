namespace Assignment3.Entities;


public class Task
{
    public int id {get; set; }
    [StringLength(100)]
    [Required]
    public string? title{get; set;}
    [Required]
    public State state;
    [StringLength(int.MaxValue)]
    public string? description{get; set;}
    public virtual ICollection<Tag> tags {get; set;}

    public Task()
    {
        this.tags = new HashSet<Tag>();
    }
}

public enum State
{
    New,
    Active,
    Resolved,
    Closed,
    Removed
}


