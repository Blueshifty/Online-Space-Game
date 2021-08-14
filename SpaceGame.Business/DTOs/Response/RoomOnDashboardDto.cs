namespace SpaceGame.Business.DTOs.Response
{
    public class RoomOnDashboardDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int PlayerCount { get; set; }
        public int CurrentPlayerCount { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
    }
}