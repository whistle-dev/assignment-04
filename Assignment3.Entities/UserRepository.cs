namespace Assignment3.Entities;

public sealed class UserRepository : IUserRepository
{

    private readonly KanbanContext _context;

    public UserRepository(KanbanContext context)
    {
        _context = context;
    }

    public (Response Response, int UserId) Create(UserCreateDTO user)
    {
        var entity = _context.users.FirstOrDefault(u => u.Name == user.Name);
        Response response;

        if (entity is null)
        {
            entity = new User();

            _context.users.Add(entity);
            _context.SaveChanges();

            response = Response.Created;
        }
        else
        {
            response = Response.Conflict;
        }

        return (response, entity.Id);

    }

    public IReadOnlyCollection<UserDTO> ReadAll()
    {
        return _context.users.Select(u => new UserDTO(u.Id, u.Name, u.Email)).ToList();
    }

    public UserDTO Read(int userId)
    {
        var entity = _context.users.FirstOrDefault(u => u.Id == userId);

        if (entity is null)
        {
            return null;
        }

        return new UserDTO(entity.Id, entity.Name, entity.Email);
    }

    public Response Update(UserUpdateDTO user)
    {
        var entity = _context.users.FirstOrDefault(u => u.Id == user.Id);

        if (entity is null)
        {
            return Response.NotFound;
        }

        entity.Name = user.Name;
        entity.Email = user.Email;

        _context.SaveChanges();

        return Response.Updated;
    }

    public Response Delete(int userId, bool force = false)
    {
        var entity = _context.users.FirstOrDefault(u => u.Id == userId);

        if (entity is null)
        {
            return Response.NotFound;
        }

        if (force)
        {
            _context.users.Remove(entity);
        }

        _context.SaveChanges();

        return Response.Deleted;
    }


}
