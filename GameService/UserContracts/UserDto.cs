namespace GameService.UserContracts
{
    public record UserDto(
        int UserId,
        string Username,
        string UserEmail,
        string UserPassword,
        string Role
        );
}
