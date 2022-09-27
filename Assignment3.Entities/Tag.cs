namespace Assignment3.Entities;

public class Tag
{   
    public int Id {get; set;}
    [StringLength(50)]
    [Required]
    public string? Name {get; set;}
    public virtual ICollection<Task> Tasks {get; set;}
    
    
    public Tag(String name)
    {
        this.Tasks = new HashSet<Task>();
        Name = name;
    }
    

}
