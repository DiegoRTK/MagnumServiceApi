using MagnumServiceApi.Models;
using System.Threading.Tasks;

namespace MagnumServiceApi.Services
{
    public interface IMoveService
    {
        Task<(bool hasFinishedRound, bool FinishedMatch, int? WinnerPlayerId, int? RoundCount)> RegisterMoveAsync(int playerId, MoveRequest moveRequest);
        Task<(bool hasFinishedRound, bool FinishedMatch, int? WinnerPlayerId)> ValidateRoundAsync(int roundId);
    }
}
