namespace MagnumServiceApi.Models
{
    public class Game
    {
        public int Id { get; set; }
        public int Player1Id { get; set; }
        public int Player2Id { get; set; }
        public int CurrentRoundId { get; set; }
        public int Player1Wins { get; set; }
        public int Player2Wins { get; set; }
        public int WinningScore { get; set; } = 3; // Número de victorias necesarias para ganar la partida

        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public Round CurrentRound { get; set; }
    }
}
