using MediatR;
using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Queries.Users;

public record GetAllUsersQuery : IRequest<IEnumerable<UserResponseModel>>;
