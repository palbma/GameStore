namespace GameService.UserContracts
{
    public record CreateUserRequest(string UserName, string UserEmail, string UserPassword, string Role);
}
