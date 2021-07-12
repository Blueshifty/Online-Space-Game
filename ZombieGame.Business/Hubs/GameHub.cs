using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ZombieGame.Business.DTOs.Request;
using ZombieGame.Business.DTOs.Response;
using ZombieGame.Business.Hubs.HubModels;
using ZombieGame.Business.Utilities.Mapper;

namespace ZombieGame.Business.Hubs
{
    public class GameHub : Hub
    {
        private readonly IMapper _mapper;
        
        private static readonly Dictionary<string, Player> Connections = new();

        private static readonly Dictionary<string, GameRoom> GameRooms = new();
        
        public GameHub(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task SendMove(MoveDto move)
        {
            try
            {
                var player = Connections[Context.ConnectionId];
                
                var oldPosX = player.PosX;

                var oldPosY = player.PosY;

                switch (move.Towards)
                {
                    case 1:
                        player.PosX--;
                        player.PosY--;
                        break;
                    case 2:
                        player.PosY -= 2;
                        break;
                    case 3:
                        player.PosX++;
                        player.PosY--;
                        break;
                    case 4:
                        player.PosX -= 2;
                        break;
                    case 6:
                        player.PosX += 2;
                        break;
                    case 7:
                        player.PosX--;
                        player.PosY++;
                        break;
                    case 8:
                        player.PosY += 2;
                        break;
                    case 9:
                        player.PosX++;
                        player.PosY++;
                        break;
                }
                
                if ( player.PosX is > 200 or < 0 || player.PosY is > 200 or < 0)
                {
                    player.PosX = oldPosX;
                    player.PosY = oldPosY;
                }
                else
                {
                    await Clients.Group(player.CurrentRoomId).SendAsync("playerMoveUpdate", new PlayerPosition()
                    {
                        PlayerName = player.Name,
                        PosX = player.PosX,
                        PosY = player.PosY
                    });
                }
            }
            catch(Exception ex)
            {
                await Clients.Caller.SendAsync("onError", "Error : " + ex.Message);
            }
        }
        
        public async Task JoinGame(string roomId)
        {
            try
            {
                var player = Connections[Context.ConnectionId];

                if (player != null && player.CurrentRoomId != roomId)
                {

                    if (player.CurrentRoomId != null)
                    {
                        await Leave(player.CurrentRoomId);
                    }

                    await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

                    player.CurrentRoomId = roomId;

                    await Clients.OthersInGroup(roomId).SendAsync("addPlayer", player);
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("onError", "You failed to join game room" + ex.Message);
            }
        }

        public async Task CreateGame(string roomName)
        {
            try
            {
                var match = Regex.Match(roomName, @"^\w+( \w+)*$");

                if (!match.Success)
                {
                    await Clients.Caller.SendAsync("onError",
                        "Invalid room name, room name must contain only letters and numbers");
                }
                else if (roomName.Length < 5 || roomName.Length > 100)
                {
                    await Clients.Caller.SendAsync("onError", "Room name must be between 5-100 characters!");
                }
                else
                {
                    var player = Connections[Context.ConnectionId];
                    
                    var gameRoom = new GameRoom(roomName, Context.ConnectionId, 10, 2);
                    
                    GameRooms.Add(gameRoom.Id, gameRoom);
                    
                    player.CurrentRoomId = gameRoom.Id;
                    
                    await Groups.AddToGroupAsync(Context.ConnectionId, gameRoom.Id);

                    await Clients.Caller.SendAsync("CreatedGameRoom", gameRoom);
                }
            }
            catch(Exception ex)
            {
                await Clients.Caller.SendAsync("onError", "Couldn't create game room : " + ex.Message);
            }
        }

        public async Task SetPlayerName(string playerName)
        {
            try
            {
                if (Connections.ContainsKey(playerName))
                {
                    await Clients.Caller.SendAsync("Name is Already Taken");
                }
                else
                {
                    Connections[Context.ConnectionId] = new Player(playerName, Context.ConnectionId);
                    
                    await Clients.Caller.SendAsync("getGameRooms");
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("onError", "Cant set name : " + ex.Message);
            }
        }

        public async Task Leave(string roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        }
        
        public IEnumerable<RoomOnDashboardDto> GetGameRooms()
        {
            return _mapper.Map<List<GameRoom>, List<RoomOnDashboardDto>>(GameRooms.Values.ToList());
        }

        public async Task GameRoomAction(RoomActionDto roomActionDto)
        {
            var gameRoom = GameRooms[roomActionDto.RoomId];
            
            if (gameRoom != null && gameRoom.CreatorId == Context.ConnectionId)
            {
                switch (roomActionDto.Action)
                {
                    case RoomActionDto.GameRoomAction.Delete:
                        break;
                    case RoomActionDto.GameRoomAction.Start:
                        gameRoom.Status = GameRoom.GameStatus.Started;
                        await Clients.Group(gameRoom.Id).SendAsync("gameStarted");
                        break;
                    case RoomActionDto.GameRoomAction.KickPlayer:
                        break;
                    case RoomActionDto.GameRoomAction.ChangeName:
                        break;
                    default:
                        break;
                }
            }
        }
        
        private string GetDevice()
        {
            var device = Context.GetHttpContext().Request.Headers["Device"].ToString();
            if (!string.IsNullOrEmpty(device) && (device.Equals("Desktop") || device.Equals("Mobile")))
                return device;

            return "Web";
        }
    }
}