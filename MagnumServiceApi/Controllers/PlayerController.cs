using Microsoft.AspNetCore.Mvc;
using MagnumServiceApi.Services;
using MagnumServiceApi.Models;
using System;
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

            try
            {
                var (player1Id, player2Id, roundId) = await _gameService.RegisterPlayersAsync(request.Player1Name, request.Player2Name);
                var gameSessionId = await _gameService.StartGameSessionAsync(player1Id, player2Id, roundId);

                return Ok(new 
                { 
                    Player1 = new PlayerInfo { Id = player1Id, Name = request.Player1Name },
                    Player2 = new PlayerInfo { Id = player2Id, Name = request.Player2Name },
                    RoundId = roundId, 
                    GameSessionId = gameSessionId.Id
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Bad request: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("battle/{gameId}")]
        public async Task<IActionResult> GetBattleDetailsAsync(int gameId)
        {
            try
            {
                var (game, moves, round, roundsPlayed) = await _gameService.GetGameById(gameId);

                return Ok(new 
                {
                    game,
                    moves,
                    round,
                    RoundsPlayed = roundsPlayed
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound($"Not found: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("new-round/{gameId}")]
        public async Task<IActionResult> StartNewRound(int gameId)
        {
            try
            {
                var (game, moves, round, roundsPlayed) = await _gameService.StartNewRound(gameId);

                return Ok(new 
                {
                    game,
                    moves,
                    round,
                    RoundsPlayed = roundsPlayed
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound($"Not found: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
    }
}
