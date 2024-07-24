using Microsoft.AspNetCore.Mvc;
using MagnumServiceApi.Services;
using System.Threading.Tasks;

namespace MagnumServiceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly IGameService _gameService;

        public PlayersController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("start-battle")]
        public async Task<IActionResult> RegisterPlayers([FromBody] RegisterPlayersRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Player1Name) || string.IsNullOrWhiteSpace(request.Player2Name))
            {
                return BadRequest("Player names cannot be empty.");
            }

            var (player1Id, player2Id, roundId) = await _gameService.RegisterPlayersAsync(request.Player1Name, request.Player2Name);
            var gameSessionId = await _gameService.StartGameSessionAsync(player1Id, player2Id, roundId);
            return Ok(new { Player1Id = player1Id, Player2Id = player2Id, RoundId = roundId, GameSessionId = gameSessionId });
        }
    }

    public class RegisterPlayersRequest
    {
        public required string Player1Name { get; set; }
        public required string Player2Name { get; set; }
    }
}
