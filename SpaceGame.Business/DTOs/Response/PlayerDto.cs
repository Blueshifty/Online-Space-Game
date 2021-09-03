namespace SpaceGame.Business.DTOs.Response
{
    public class PlayerDto
    {
        public string Name { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public string Ship { get; set; }
        public int Health { get; set; }
        public int HealthLimit { get; set; }
        public int AmmoCount { get; set; }
        public int AmmoLimit { get; set; }
        public int StarDust { get; set; }
    }
}