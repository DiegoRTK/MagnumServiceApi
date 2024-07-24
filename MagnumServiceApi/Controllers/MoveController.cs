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

        public MoveController(IMoveService moveService)
        {
            _moveService = moveService;
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
                var (hasFinishedRound, FinishedMatch, WinnerPlayerId, game) = await _moveService.RegisterMoveAsync(request.playerId, request);
                return Ok(new
                {
                    HasFinishedRound = hasFinishedRound,
                    FinishedMatch,
                    WinnerPlayerId,
                    GameId = game?.Id
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
    }
}
