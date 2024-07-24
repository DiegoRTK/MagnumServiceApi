using MagnumServiceApi.Models;
using System.Threading.Tasks;
using MagnumServiceApi.Data;
using Microsoft.EntityFrameworkCore;

namespace MagnumServiceApi.Services
{
    public class RoundService(AppDbContext context) : IRoundService
    {
        private readonly AppDbContext _context = context;

        public async Task<Round> CreateRoundAsync()
        {
            var round = new Round
            {
                WinnerId = 0
            };

            _context.Rounds.Add(round);
            await _context.SaveChangesAsync();

            return round;
        }

        public async Task<(bool hasFinishedRound, int? winnerId, Game game)> DetermineWinner(Move move1, Move move2)
        {
            var game = _context.Game.FirstOrDefault(g => g.CurrentRoundId == move1.RoundId);
            
            if (game != null) {
                 if (move1.MoveType == move2.MoveType )
            {
                return (false, 0, game);
            }

            var round = await _context.Rounds.FirstOrDefaultAsync(r => r.Id == move1.RoundId) ?? throw new InvalidOperationException("Round not found");
            if ((move1.MoveType == MoveType.Stone && move2.MoveType == MoveType.Scissor) ||
                (move1.MoveType == MoveType.Scissor && move2.MoveType == MoveType.Paper) ||
                (move1.MoveType == MoveType.Paper && move2.MoveType == MoveType.Stone))
            {
                round.WinnerId = move1.playerId;
                game.Player1Wins++;
                await _context.SaveChangesAsync();
                return (true, move1.playerId, game);
            }
            else
            {
                round.WinnerId = move2.playerId;
                game.Player1Wins++;
                await _context.SaveChangesAsync();
                return (true, move2.playerId, game);
            }
            } else {
                throw new InvalidOperationException("Game session not found.");
            }
          
        }
    }
}
