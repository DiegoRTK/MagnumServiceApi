using System;
using System.Threading.Tasks;
using MagnumServiceApi.Data;
using MagnumServiceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MagnumServiceApi.Services
{
    public class RoundService(AppDbContext context) : IRoundService
    {
        private readonly AppDbContext _context = context;

        public async Task<Round> CreateRoundAsync()
        {
            try
            {
                var round = new Round { WinnerId = 0 };
                _context.Rounds.Add(round);
                await _context.SaveChangesAsync();
                return round;
            }
            catch (DbUpdateException ex)
            {
                throw new ApplicationException("An error occurred while creating the round.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An unexpected error occurred.", ex);
            }
        }

        public async Task<(bool hasFinishedRound, int? winnerId, Game game)> DetermineWinner(Move move1, Move move2)
        {
            try
            {
                var game = await _context.Game.FirstOrDefaultAsync(g => g.CurrentRoundId == move1.RoundId);
                if (game == null)
                {
                    throw new InvalidOperationException("Game session not found.");
                }

                if (move1.MoveType == move2.MoveType)
                {
                    return (false, 0, game);
                }

                var round = await _context.Rounds.FirstOrDefaultAsync(r => r.Id == move1.RoundId)
                            ?? throw new InvalidOperationException("Round not found");

                if (
                    (move1.MoveType == MoveType.Stone && move2.MoveType == MoveType.Scissor) ||
                    (move1.MoveType == MoveType.Scissor && move2.MoveType == MoveType.Paper) ||
                    (move1.MoveType == MoveType.Paper && move2.MoveType == MoveType.Stone)
                )
                {
                    round.WinnerId = move1.PlayerId;
                    game.Player1Wins++;
                    await _context.SaveChangesAsync();
                    return (true, move1.PlayerId, game);
                }
                else
                {
                    round.WinnerId = move2.PlayerId;
                    game.Player2Wins++;
                    await _context.SaveChangesAsync();
                    return (true, move2.PlayerId, game);
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                throw new ApplicationException("An error occurred while updating the database.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An unexpected error occurred.", ex);
            }
        }
    }
}
