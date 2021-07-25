namespace ZombieGame.Business.DTOs.Request
{
    public class DisconnectUser
    {
        public string ConnectionId { get; set; }
        public string RoomId { get; set; }
    }
}