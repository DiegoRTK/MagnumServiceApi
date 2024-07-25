using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MagnumServiceApi.Models
{
    public class Round
    {
        public int Id { get; set; }
        public int GameSessionId { get; set; }
        public int WinnerId { get; set; }

        [ForeignKey(nameof(GameSessionId))]
        [JsonIgnore]
        public Game Game { get; set; }
    }
}
