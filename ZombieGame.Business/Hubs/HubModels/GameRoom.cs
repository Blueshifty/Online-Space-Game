using System;

namespace ZombieGame.Business.Hubs.HubModels
{
    public class GameRoom
    {
        public string Id { get; }

        public DateTime CreatedAt { get; }
        public string CreatorId { get; set; }
        
        public int PlayerCount { get; set; }
        
        public string Name { get; set;}

        public int[,] Map { get; set; }

        public GameStatus Status { get; set; }

        public GameRoom(string name, string creatorId, int playerCount, int size)
        {
            Name = name;
            Id = Guid.NewGuid().ToString();
            CreatorId = creatorId;
            PlayerCount = playerCount;
            CreatedAt = DateTime.Now;
            Map = new int[size * 100, size * 100];
            Status = GameStatus.Waiting;
        }
        public enum GameStatus
        {
            Waiting,
            Started,
            Ended
        }
    }
}