using MagnumServiceApi.Models;
using System.Threading.Tasks;

namespace MagnumServiceApi.Services
{
    public interface IRoundService
    {
        Task<Round> CreateRoundAsync();
        Task<(bool hasFinishedRound, int? winnerId, Game game)> DetermineWinner(Move move1, Move move2);
    }
}
