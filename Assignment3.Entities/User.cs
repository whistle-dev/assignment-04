namespace Assignment3.Entities;

public class User
{
    public int id {get; set;}
    [StringLength(100)]
    [Required]
    public string? name {get; set;}
    [StringLength(100)]
    [Required]
    public string? email {get; set;}

    public Task[] tasks;

}
