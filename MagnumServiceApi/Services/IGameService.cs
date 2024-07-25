using MagnumServiceApi.Models;
using System.Threading.Tasks;

namespace MagnumServiceApi.Services
{
    public interface IGameService
    {
        Task<Game> StartGameSessionAsync(int player1Id, int player2Id, int roundId);
        Task<(bool FinishedGame, int? WinnerId)> CheckGameResultAsync(int gameSessionId);
        Task<(int player1Id, int player2Id, int roundId)> RegisterPlayersAsync(string player1Name, string player2Name);

        Task<(Game game, List<Move> moves, Round round, int RoundsPlayed, List<Round> Rounds)> GetGameById(int gameSessionId);

        Task<(Game game, List<Move> moves, Round round, int RoundsPlayed, List<Round> Rounds)> StartNewRound(int gameId);
    }
}
