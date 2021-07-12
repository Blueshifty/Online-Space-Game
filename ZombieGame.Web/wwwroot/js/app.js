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
                     this.connection.invoke("GetGameRooms").then(response => {
                         console.log(response);
                         this.gameRooms = response;
                     });
                });
                this.connection.on("CreatedGameRoom", (gameRoom) => {
                    this.currentRoom = gameRoom;
                });
            },
            gameRoomActions: function(action, gameRoom){
                this.connection.invoke(action,gameRoom).then(response => {
                   switch(action){
                       case 'CreateGame':
                           let modal = new bootstrap.Modal(document.getElementById('gameRoomModal'));
                           modal.hide();
                           console.log(modal);
                           this.viewState = 'atGameRoom';
                   }
                });
            },
            setPlayerName(){
                this.connection.invoke("SetPlayerName", this.nick).then(response =>{
                    this.viewState = 'gameRooms';
                });
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