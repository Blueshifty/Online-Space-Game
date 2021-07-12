namespace ZombieGame.Business.Hubs.HubModels
{
    public class Player
    {
        public string Name { get; set; }
        public string CurrentRoomId { get; set; }
        public string ConnectionId { get; set; }
        public int PosX { get; set;}
        public int PosY { get; set; }

        public Player(string name, string connectionId)
        {
            Name = name;
            ConnectionId = connectionId;
        }
    }
}