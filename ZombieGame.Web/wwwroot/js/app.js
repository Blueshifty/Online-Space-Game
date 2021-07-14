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
            gameContext : null,
            players: [],
            player: null,
            playerController: null,
            up:false,
            right:false,
            down:false,
            left:false
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
                    this.player = players.find(p => p.name === this.nick);
                    this.players = players.filter(p => p.name !== this.nick);
                    window.requestAnimationFrame(this.gameLoop);
                    //console.log(players);
                });
            },
            sendMove: function(towards){
                console.log(towards);
                this.connection.invoke("SendMove", {Towards: towards}).then(response => {
                    //console.log(response);
                })
            },
            joinGame: function(id){
              this.connection.invoke('JoinGame', {name:this.nick, roomId:id}).then(response => {
                  console.log(response);
                  this.viewState = 'inGame';
                  this.$nextTick(() => {
                      this.initGame();
                  })
              });
            },
            getGameRooms: function(){
              this.connection.invoke("GetGameRooms").then(response => {
                  console.log(response);
                  this.gameRooms = response;
              })  
            },
            setName: function(){
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
            initGame: function(){
                this.gameContext = document.querySelector("canvas").getContext("2d");
                this.gameContext.canvas.height = 600;
                this.gameContext.canvas.width = 600;
                this.gameContext.canvas.style.height = "600px";
                this.gameContext.canvas.style.width = "600px";
                window.addEventListener("keydown", this.keyListener)
                window.addEventListener("keyup", this.keyListener);
                this.keyLoop();
            },
            keyListener : function(event){
                let key_state = (event.type === "keydown");
                switch(event.keyCode) {
                    case 37:
                        this.left = key_state;
                        break;
                    case 38:
                        this.up = key_state;
                        break;
                    case 39:
                        this.right = key_state;
                        break;
                    case 40:
                        this.down = key_state;
                }
            },
            keyLoop: function(){
                if(this.up){
                    if(this.right){
                        this.sendMove(9);
                    }else if(this.left){
                        this.sendMove(7);
                    }else{
                        this.sendMove(8);
                    }
                }else if(this.down){
                    if(this.right){
                        this.sendMove(3);
                    }else if(this.left){
                        this.sendMove(1);
                    }else{
                        this.sendMove(2);
                    }
                }else if(this.left) {
                    this.sendMove(4);
                }else if(this.right){
                    this.sendMove(6);
                }
                setTimeout(this.keyLoop,75);
            },
            gameLoop: function(){
                this.gameContext.fillStyle = "#F7F7F7";
                this.gameContext.fillRect(0,0,600,600);
                this.drawPlayer(300,300, "#98DEFF");
                console.log(this.player);
                this.players.forEach(p => {
                       const pX =  this.player.posX > p.posX ? 300 - (this.player.posX - p.posX) : 300 + (p.posX - this.player.posX);
                       const pY =  this.player.posY > p.posY ? 300 - (this.player.posY - p.posY) : 300 + (p.posY - this.player.posY);
                       console.log(pY, pX);
                        if(pX > 0 && pX < 800 && pY > 0 && pY < 800){
                            this.drawPlayer(pX,pY, "#FB0000");
                        }
                });
            },
            drawPlayer: function(x,y,color){
              this.gameContext.fillStyle = color;
              this.gameContext.beginPath();
              this.gameContext.rect(x, y, 10,10);
              this.gameContext.fill();
            },
        },
        mounted: async function () {
                console.log(this.viewState);
                await this.initSignalRConnection();
                this.viewState = 'nick';
        }
    }
);