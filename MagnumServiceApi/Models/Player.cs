using System.ComponentModel.DataAnnotations;

namespace MagnumServiceApi.Models
{
    public class Player
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }
    }

    public class RegisterPlayersRequest
    {
        [Required]
        public required string Player1Name { get; set; }

        [Required]
        public required string Player2Name { get; set; }
    }

    public class PlayerInfo
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }
    }
}
