namespace MagnumServiceApi.Models
{
    public class Round
    {
        public int Id { get; set; }
        public int GameSessionId { get; set; }
        public int WinnerId { get; set; }

        public Game Game { get; set; }
    }
}
