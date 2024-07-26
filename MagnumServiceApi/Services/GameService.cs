using MagnumServiceApi.Data;
using MagnumServiceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MagnumServiceApi.Services
{
    public class GameService(AppDbContext context, IRoundService roundService) : IGameService
    {
        private readonly AppDbContext _context = context;
        private readonly IRoundService _roundService = roundService;

        public async Task<Game> StartGameSessionAsync(int player1Id, int player2Id, int roundId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var gameSession = new Game
                {
                    Player1Id = player1Id,
                    Player2Id = player2Id,
                    CurrentRoundId = roundId,
                    Player1Wins = 0,
                    Player2Wins = 0,
                    WinningScore = 3
                };
                _context.Game.Add(gameSession);
                await _context.SaveChangesAsync();

                var round = await _context.Rounds.FirstOrDefaultAsync(r => r.Id == roundId);
                if (round == null)
                {
                    throw new ArgumentException("Current round not found.");
                }

                round.GameSessionId = gameSession.Id;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return gameSession;
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException(
                    "An error occurred while updating the database.",
                    ex
                );
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException("An unexpected error occurred.", ex);
            }
        }

        public async Task<(int player1Id, int player2Id, int roundId)> RegisterPlayersAsync(
            string player1Name,
            string player2Name
        )
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
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
                    PlayerId = player1.Id,
                    MoveType = MoveType.NoMove,
                    RoundId = round.Id
                };

                var move2 = new Move
                {
                    PlayerId = player2.Id,
                    MoveType = MoveType.NoMove,
                    RoundId = round.Id
                };

                _context.Moves.Add(move1);
                _context.Moves.Add(move2);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return (player1.Id, player2.Id, round.Id);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException(
                    "An error occurred while updating the database.",
                    ex
                );
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException("An unexpected error occurred.", ex);
            }
        }

        public async Task<(bool FinishedGame, int? WinnerId)> CheckGameResultAsync(
            int gameSessionId
        )
        {
            try
            {
                var gameSession =
                    await _context.Game.FirstOrDefaultAsync(gs => gs.Id == gameSessionId)
                    ?? throw new ArgumentException("Game session not found.");

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
            catch (Exception ex)
            {
                throw new ApplicationException(
                    "An error occurred while checking the game result.",
                    ex
                );
            }
        }

        public async Task<(
            Game game,
            List<Move> moves,
            Round round,
            int RoundsPlayed,
            List<Round> Rounds
        )> GetGameById(int gameSessionId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var gameFound =
                    await _context.Game.FirstOrDefaultAsync(g => g.Id == gameSessionId)
                    ?? throw new ArgumentException("Game session not found.");

                var moves = await _context
                    .Moves.Where(m => m.RoundId == gameFound.CurrentRoundId)
                    .ToListAsync();
                var round =
                    await _context.Rounds.FindAsync(gameFound.CurrentRoundId)
                    ?? throw new ArgumentException("Round not found.");
                var roundsPlayed = await _context
                    .Rounds.Where(r => r.GameSessionId == gameFound.Id)
                    .ToListAsync();

                await transaction.CommitAsync();
                return (gameFound, moves, round, roundsPlayed.Count, roundsPlayed);
            }
            catch (ArgumentException)
            {
                await transaction.RollbackAsync();
                throw;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException("An unexpected error occurred.", ex);
            }
        }

        public async Task<(
            Game game,
            List<Move> moves,
            Round round,
            int RoundsPlayed,
            List<Round> Rounds
        )> StartNewRound(int gameSessionId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var gameFound =
                    await _context.Game.FirstOrDefaultAsync(g => g.Id == gameSessionId)
                    ?? throw new ArgumentException("Game session not found.");

                var moves = await _context
                    .Moves.Where(m => m.RoundId == gameFound.CurrentRoundId)
                    .ToListAsync();
                if (
                    gameFound.Player1Wins == gameFound.WinningScore
                    || gameFound.Player2Wins == gameFound.WinningScore
                )
                {
                    await transaction.RollbackAsync();
                    throw new ApplicationException(
                        "Cannot create a new round due to limit of rounds won: 3"
                    );
                }
                var roundCreated = new Round { WinnerId = 0, GameSessionId = gameSessionId };
                _context.Rounds.Add(roundCreated);
                await _context.SaveChangesAsync();

                gameFound.CurrentRoundId = roundCreated.Id;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                var roundsPlayed = await _context
                    .Rounds.Where(r => r.GameSessionId == gameFound.Id)
                    .ToListAsync();

                return (gameFound, moves, roundCreated, roundsPlayed.Count, roundsPlayed);
            }
            catch (ArgumentException)
            {
                await transaction.RollbackAsync();
                throw;
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException(
                    "An error occurred while updating the database.",
                    ex
                );
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApplicationException("An unexpected error occurred.", ex);
            }
        }
    }
}
