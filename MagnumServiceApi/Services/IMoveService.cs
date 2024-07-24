using MagnumServiceApi.Models;
using System.Threading.Tasks;

namespace MagnumServiceApi.Services
{
    public interface IMoveService
    {
        Task<(bool hasFinishedRound, bool FinishedMatch, int? WinnerPlayerId, Game? game)> RegisterMoveAsync(int playerId, MoveRequest moveRequest);
        Task<(bool hasFinishedRound, bool FinishedMatch, int? WinnerPlayerId, Game? game)> ValidateRoundAsync(int roundId);
    }
}
