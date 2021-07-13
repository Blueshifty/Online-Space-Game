Vue.config.devtools = true;

const vue = new Vue({
        el: "#vue-wrapper",
        delimiters: ['${', '}}'],
        data: {
            connection: null,
            nick: null,
            viewState: 'loading',
            errorMessage: null,
            gameRooms: [],
            newRoomName: null,
            currentRoom: null,
        },
        methods: {
            startConnection: async function () {
                try {
                    await this.connection.start();
                    console.log("SignalR Connected");
                } catch (err) {
                    console.log(err);
                    setTimeout(this.startConnection, 2500);
                }
            },
            initializeSignalRMethods: function () {
                this.connection.on("onError", (message) => {
                    this.errorMessage = "Error : " + message;
                    console.log(this.errorMessage);
                });
                this.connection.on("getGameRooms", () => {
                    this.getGameRooms()
                });
                this.connection.on("CreatedGameRoom", (gameRoom) => {
                    this.currentRoom = gameRoom;
                });
                this.connection.on("playerMoveUpdate", (playerMove) => {
                    console.log(playerMove);
                });
                this.connection.on("update", (players) => {
                    //console.log(players);
                });
            },
            sendMove(towards){
                this.connection.invoke("SendMove", {Towards: towards}).then(response => {
                    console.log(response);
                })
            },
            joinGame(id){
              this.connection.invoke('JoinGame', {name:this.nick, roomId:id}).then(response => {
                  console.log(response);
                  this.viewState = 'inGame';
              });
            },
            getGameRooms(){
              this.connection.invoke("GetGameRooms").then(response => {
                  console.log(response);
                  this.gameRooms = response;
              })  
            },
            setName() {
                if (this.nick.length > 1 || this.nick.length < 50) {
                    this.viewState = 'gameRooms';
                    this.getGameRooms();
                }
            },
            initSignalRConnection: async function () {
                this.connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:5001/gameHub")
                    .configureLogging(signalR.LogLevel.Information).build();

                this.initializeSignalRMethods();

                this.connection.onclose(this.startConnection);

                await this.startConnection();
            },
        },
        mounted: async function () {
                console.log(this.viewState);
                await this.initSignalRConnection();
                this.viewState = 'nick';
        }
    }
);