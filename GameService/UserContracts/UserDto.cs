namespace GameService.UserContracts
{
    public record UserDto(
        int GameId,
        string Username,
        string UserEmail,
        string UserPassword,
        string Role
        );
}
