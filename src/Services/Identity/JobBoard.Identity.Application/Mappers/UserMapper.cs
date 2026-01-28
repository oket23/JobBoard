using JobBoard.Identity.Domain.Models.Users;
using JobBoard.Identity.Domain.Response.Users;

namespace JobBoard.Identity.Application.Mappers;

public static class UserMapper
{
    public static UserResponse ToResponse(this User user)
    {
        if (user == null)
        {
            return null!;
        }

        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Gender =  user.Gender,
            DateOfBirth = user.DateOfBirth,
            Role = user.Role
        };
    }

    public static IEnumerable<UserResponse> ToResponseList(this IEnumerable<User> users)
    {
        return users.Select(u => u.ToResponse());
    }
}