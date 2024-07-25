using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MagnumServiceApi.Models
{
    public class Game
    {
        public int Id { get; set; }

        [Required]
        public int Player1Id { get; set; }

        [Required]
        public int Player2Id { get; set; }

        [Required]
        public int CurrentRoundId { get; set; }

        [Range(0, int.MaxValue)]
        public int Player1Wins { get; set; }

        [Range(0, int.MaxValue)]
        public int Player2Wins { get; set; }

        [Range(1, int.MaxValue)]
        public int WinningScore { get; set; } = 3;

        [ForeignKey(nameof(Player1Id))]
        [JsonIgnore]
        public Player Player1 { get; set; }

        [ForeignKey(nameof(Player2Id))]
        [JsonIgnore]
        public Player Player2 { get; set; }

        [ForeignKey(nameof(CurrentRoundId))]
        [JsonIgnore]
        public Round CurrentRound { get; set; }
    }
}
