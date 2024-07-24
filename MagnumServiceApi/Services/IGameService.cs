using MagnumServiceApi.Models;
using System.Threading.Tasks;

namespace MagnumServiceApi.Services
{
    public interface IGameService
    {
        Task<int> StartGameSessionAsync(int player1Id, int player2Id, int roundId);
        Task<(bool FinishedGame, int? WinnerId)> CheckGameResultAsync(int gameSessionId);
        Task<(int player1Id, int player2Id, int roundId)> RegisterPlayersAsync(string player1Name, string player2Name);

    }
}
