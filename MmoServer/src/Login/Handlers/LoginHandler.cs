using MmoServer.Connection;
using MmoServer.Connection.Domain;
using MmoServer.Logging;
using MmoServer.Messages.Handler;
using MmoServer.PlayerCharacters;
using MmoServer.Players;
using MmoServer.Players.Factory;
using MmoServer.World;
using MmoShared.Messages.Login;

namespace MmoServer.Login.Handlers;

public sealed class LoginHandler : MessageHandler<LoginNotify>
{
    private readonly LoginService _loginService;
    private readonly PlayerCharacterService _playerCharacterService;
    private readonly WorldService _worldService;
    private readonly IPlayerFactory _playerFactory;

    public override ConnectionState AllowedState => ConnectionState.Login;
    
    public LoginHandler(LoginService loginService, PlayerCharacterService playerCharacterService, WorldService worldService, IPlayerFactory playerFactory)
    {
        _loginService = loginService;
        _playerCharacterService = playerCharacterService;
        _worldService = worldService;
        _playerFactory = playerFactory;
    }

    protected override async Task HandleAsync(
        ClientConnection connection,
        LoginNotify message)
    {
        var result = await _loginService.Login(message.Username, message.Password);

        if (result.resultCode != LoginResultCode.Success || result.user == null)
        {
            connection.AddMessage(new LoginResultSync
            {
                ResultCode = result.resultCode
            });
            
            return;
        }
        
        var characters = await _playerCharacterService.LoadCharacters(result.user.Id);

        if (characters.Length == 0)
        {
            MmoLogger.Error("User has no characters: " + result.user.Id);
            connection.AddMessage(new LoginResultSync
            {
                ResultCode = LoginResultCode.InternalServerError
            });
            return;
        }
            
        // Current character limit is 1
        var selectedCharacter = characters[0];
        var player = _playerFactory.CreatePlayer(selectedCharacter, connection);
        
        connection.Authenticate(player);

        _worldService.EnqueuePlayer(connection.Player!);
        
        connection.AddMessage(new LoginResultSync
        {
            ResultCode = result.resultCode,
            PlayerDataDto = selectedCharacter.ToDto()
        });
    }
}