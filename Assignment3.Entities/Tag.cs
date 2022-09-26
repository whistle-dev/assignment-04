namespace Assignment3.Entities;

public class Tag
{   
    public int id {get; set;}
    [StringLength(50)]
    [Required]
    public string? name {get; set;}
    public virtual ICollection<Task> tasks {get; set;}
    
    
    public Tag()
    {
        this.tasks = new HashSet<Task>();
    }
    

}
