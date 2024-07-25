using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MagnumServiceApi.Models
{
    public enum MoveType
    {
        NoMove,
        Stone, // Piedra
        Paper, // Papel
        Scissor // Tijera
    }

    public class Move
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }

        public int RoundId { get; set; }

        [ForeignKey(nameof(RoundId))]
        [JsonIgnore]
        public Round Round { get; set; }

        public MoveType MoveType { get; set; }
    }

    public class MoveRequest
    {
        public int PlayerId { get; set; }
        public int RoundId { get; set; }
        public MoveType MoveType { get; set; }
        public int GameId { get; set; }
    }
}
