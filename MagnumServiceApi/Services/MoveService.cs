using MagnumServiceApi.Data;
using MagnumServiceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MagnumServiceApi.Services
{
    public class MoveService(
        AppDbContext context,
        IRoundService roundService,
        IGameService gameService
        ) : IMoveService
    {
        private readonly AppDbContext _context = context;
        private readonly IRoundService _roundService = roundService;
        private readonly IGameService _gameService = gameService;

        public async Task<(
            bool hasFinishedRound,
            bool FinishedMatch,
            int? WinnerPlayerId,
            int? RoundCount
        )> RegisterMoveAsync(int PlayerId, MoveRequest moveRequest)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var move = await _context.Moves.FirstOrDefaultAsync(m =>
                    m.PlayerId == PlayerId && moveRequest.RoundId == m.RoundId
                );

                if (move == null)
                {
                    move = new Move
                    {
                        PlayerId = PlayerId,
                        MoveType = moveRequest.MoveType,
                        RoundId = moveRequest.RoundId
                    };

                    _context.Moves.Add(move);
                }
                else
                {
                    if (move.MoveType > 0)
                    {
                        throw new InvalidOperationException(
                            "Player cannot perform an extra movement."
                        );
                    }
                    else
                    {
                        move.MoveType = moveRequest.MoveType;
                        _context.Moves.Update(move);
                    }
                }

                await _context.SaveChangesAsync();

                var rounds = await _context.Rounds.CountAsync(r =>
                    r.GameSessionId == moveRequest.GameId
                );
                var (hasFinishedRound, FinishedMatch, WinnerPlayerId) = await ValidateRoundAsync(
                    move.RoundId
                );

                await transaction.CommitAsync();

                return (hasFinishedRound, FinishedMatch, WinnerPlayerId, rounds);
            }
            catch (InvalidOperationException ex)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException(ex.Message, ex);
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

        public async Task<(
            bool hasFinishedRound,
            bool FinishedMatch,
            int? WinnerPlayerId
        )> ValidateRoundAsync(int roundId)
        {
            try
            {
                var moves = await _context.Moves.Where(m => m.RoundId == roundId).ToListAsync();
                if (moves.Count != 2)
                {
                    return (false, false, null);
                }

                var move1 = moves[0];
                var move2 = moves[1];

                if (move1.MoveType > 0 && move2.MoveType > 0)
                {
                    var (hasFinishedRound, winnerId, game) = await _roundService.DetermineWinner(
                        move1,
                        move2
                    );

                    if (hasFinishedRound)
                    {
                        var (FinishedMatch, WinnerPlayerId) =
                            await _gameService.CheckGameResultAsync(game.Id);
                        return (true, FinishedMatch, winnerId);
                    }
                    else
                    {
                        return (false, false, winnerId);
                    }
                }

                return (false, false, null);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while validating the round.", ex);
            }
        }
    }
}
