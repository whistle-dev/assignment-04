namespace Assignment4.Entities;

public class Tag
{
    public int Id { get; set; }
    [StringLength(50)]
    [Required]
    public string? Name { get; set; }
    public virtual ICollection<WorkItem> WorkItems { get; set; }


    public Tag(String name)
    {
        this.WorkItems = new HashSet<WorkItem>();
        Name = name;
    }


}
