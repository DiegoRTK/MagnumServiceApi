using MagnumServiceApi.Models;
using MagnumServiceApi.Data;
using Microsoft.EntityFrameworkCore;

namespace MagnumServiceApi.Services
{
    public class MoveService : IMoveService
    {
        private readonly AppDbContext _context;
        private readonly IRoundService _roundService;

        private readonly IGameService _gameService;

        public MoveService(AppDbContext context, IRoundService roundService, IGameService gameService)
        {
            _context = context;
            _roundService = roundService;
            _gameService = gameService;
        }

        public async Task<(bool hasFinishedRound, bool FinishedMatch, int? WinnerPlayerId, Game? game)> RegisterMoveAsync(int playerId, MoveRequest moveRequest)
        {
            var move = await _context.Moves
                .FirstOrDefaultAsync(m => m.playerId == playerId && moveRequest.RoundId == m.RoundId);

            if (move == null)
            {
                move = new Move
                {
                    playerId = playerId,
                    MoveType = moveRequest.MoveType,
                    RoundId = moveRequest.RoundId
                };

                _context.Moves.Add(move);
            }
            else
            {
                if (move.MoveType > 0)
                {
                    throw new ArgumentException("Player cannot perform an extra movement.");
                } else {
                    move.MoveType = moveRequest.MoveType;
                    _context.Moves.Update(move);
                }  
            }
            await _context.SaveChangesAsync();
            var (hasFinishedRound, FinishedMatch, WinnerPlayerId, game) = await ValidateRoundAsync(move.RoundId);
            return (hasFinishedRound, FinishedMatch, WinnerPlayerId, game);
        }

        public async Task<(bool hasFinishedRound, bool FinishedMatch, int? WinnerPlayerId, Game? game)> ValidateRoundAsync(int roundId)
        {
            var moves = await _context.Moves
                .Where(m => m.RoundId == roundId)
                .ToListAsync();
            if (moves.Count == 2)
            {
                var move1 = moves[0];
                var move2 = moves[1];
                if (move1.MoveType > 0 && move2.MoveType > 0) {
                var (hasFinishedRound, winnerId, game) = await _roundService.DetermineWinner(move1, move2);
                    if (hasFinishedRound)
                    {   
                        var (FinishedMatch, WinnerPlayerId) = await _gameService.CheckGameResultAsync(game.Id);
                        return (true, FinishedMatch, winnerId, game);
                    }
                    else
                    {
                        return (false, false, winnerId, game);
                    }
                }
            }
            var gameFound = _context.Game.FirstOrDefault(g => g.CurrentRoundId == moves[0].RoundId);
            return (false, false, null, null);
        }
    }
}
