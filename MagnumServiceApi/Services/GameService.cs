using MagnumServiceApi.Models;
using MagnumServiceApi.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MagnumServiceApi.Services
{
    public class GameService : IGameService
    {
        private readonly AppDbContext _context;
        private readonly IRoundService _roundService;

        public GameService(AppDbContext context, IRoundService roundService)
        {
            _context = context;
            _roundService = roundService;
        }

    public async Task<int> StartGameSessionAsync(int player1Id, int player2Id, int roundId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                await _context.SaveChangesAsync();

                var gameSession = new Game
                {
                    Player1Id = player1Id,
                    Player2Id = player2Id,
                    CurrentRoundId = roundId, // Indica que aún no se ha creado una ronda
                    Player1Wins = 0,
                    Player2Wins = 0,
                    WinningScore = 3 // Cambiar según tu lógica de puntuación
                };

                _context.Game.Add(gameSession);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return gameSession.Id;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

   public async Task<(int player1Id, int player2Id, int roundId)> RegisterPlayersAsync(string player1Name, string player2Name)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var player1 = new Player { Name = player1Name };
                    var player2 = new Player { Name = player2Name };

                    _context.Players.Add(player1);
                    _context.Players.Add(player2);
                    await _context.SaveChangesAsync();

                    var round = await _roundService.CreateRoundAsync();
                    await _context.SaveChangesAsync();

                    var move1 = new Move
                    {
                        playerId = player1.Id,
                        MoveType = MoveType.NoMove,
                        RoundId = round.Id
                    };

                    var move2 = new Move
                    {
                        playerId = player2.Id,
                        MoveType = MoveType.NoMove,
                        RoundId = round.Id
                    };

                    _context.Moves.Add(move1);
                    _context.Moves.Add(move2);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return (player1.Id, player2.Id, round.Id);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
        public async Task<(bool FinishedGame, int? WinnerId)> CheckGameResultAsync(int gameSessionId)
        {
            var gameSession = await _context.Game
                .FirstOrDefaultAsync(gs => gs.Id == gameSessionId) ?? throw new ArgumentException("Game session not found.");
            if (gameSession.Player1Wins == gameSession.WinningScore)
            {
                return (true, gameSession.Player1Id);
            }

            if (gameSession.Player2Wins == gameSession.WinningScore)
            {
                return (true, gameSession.Player2Id);
            }

            return (false, null);
        }
    }
}
