using Microsoft.AspNetCore.Mvc;
using MagnumServiceApi.Models;
using MagnumServiceApi.Services;
using System.Threading.Tasks;

namespace MagnumServiceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoveController : ControllerBase
    {
        private readonly IMoveService _moveService;

        private readonly IGameService _gameService;

        public MoveController(IMoveService moveService, IGameService gameService)
        {
            _moveService = moveService;
            _gameService = gameService;
        }

    [HttpPost("register")]
        public async Task<IActionResult> RegisterMoveAsync(MoveRequest request)
        {
            if (request == null)
            {
                return BadRequest("Move request is null");
            }

            try
            {
                var (hasFinishedRound, FinishedMatch, WinnerPlayerId, rounds) = await _moveService.RegisterMoveAsync(request.PlayerId, request);
                return Ok(new
                {
                    HasFinishedRound = hasFinishedRound,
                    FinishedMatch,
                    WinnerPlayerId,
                    RoundsPlayed = rounds
                    // GameId = currentGame
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    // [HttpGet('game')]
    }
}
