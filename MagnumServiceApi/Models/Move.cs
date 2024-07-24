namespace MagnumServiceApi.Models{

    public enum MoveType   
    {
        NoMove,
        Stone, // Piedra
        Paper, // Papel
        Scissor // Tijera
    }

    public class Move {
        public int Id {get;set;}
        public int playerId {get;set;}

        public int RoundId {get; set;}

        public Round Round { get; set; }
        public MoveType MoveType { get; set; }
    }

    public class MoveRequest {
        public int playerId {get; set;}
        public int RoundId {get; set;}
        public MoveType MoveType { get; set; }
    }
}