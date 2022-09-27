namespace Assignment3.Entities;

public class User
{
    public int Id { get; set; }
    [StringLength(100)]
    [Required]
    public string? Name { get; set; }
    [StringLength(100)]
    [Required]
    public string? Email { get; set; }

    public Task[] Tasks;

    public User()
    {
        Tasks = new Task[0];
    }
}
