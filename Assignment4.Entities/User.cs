namespace Assignment4.Entities;

public class User
{
    public int Id { get; set; }
    [StringLength(100)]
    [Required]
    public string? Name { get; set; }
    [StringLength(100)]
    [Required]
    public string? Email { get; set; }

    public WorkItem[] WorkItems;

    public User()
    {
        WorkItems = new WorkItem[0];
    }
}
