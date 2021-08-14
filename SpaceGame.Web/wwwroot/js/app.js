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
            bullets: [],
            player: null,
            playerController: null,
            moveState: null,
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
                    //console.log(playerMove);
                });
                this.connection.on("update", (players,bullets) => {
                    this.player = players.find(p => p.name === this.nick);
                    this.players = players.filter(p => p.name !== this.nick);
                    this.bullets = bullets;
                    window.requestAnimationFrame(this.gameLoop);
                    //console.log(players);
                });
                this.connection.on("updateBullets", (bullets) => {
                    
                });
            },
            sendMove: function(towards){
                //(towards);
                this.connection.invoke("SendMove", towards).then(response => {
                    //console.log(response);
                })
            },
            fireBullet: function(){
                this.connection.invoke("FireBullet").then(response => {
                    //console.log(response);
                });
            },
            joinGame: function(id){
              this.connection.invoke('JoinGame', {name:this.nick, roomId:id}).then(response => {
                  //console.log(response);
                  this.viewState = 'inGame';
                  this.$nextTick(() => {
                      this.initGame();
                  });
                  this.currentRoom = this.gameRooms.find(g => g.id === id);
              });
            },
            getGameRooms: function(){
              this.connection.invoke("GetGameRooms").then(response => {
                  //console.log(response);
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
                this.connection = new signalR.HubConnectionBuilder().withUrl("/gameHub")
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
                this.gameContext.font = "10px Arial";
                window.addEventListener("keydown", this.keyListener)
                window.addEventListener("keyup", this.keyListener);
            },
            keyListener : function(event){
                let keyState = (event.type === "keydown");
                let newState = null;
                switch(event.keyCode) {
                    case 32:
                        this.fireBullet();
                        break;
                    case 37:
                        newState = {towards: 4,keyState: keyState};
                        break;
                    case 38:
                        newState = {towards: 1, keyState: keyState};
                        break;
                    case 39:
                        newState = {towards: 2, keyState: keyState};
                        break;
                    case 40:
                        newState = {towards:3, keyState: keyState};
                        break;
                }
                if(JSON.stringify(this.moveState) !== JSON.stringify(newState)){
                    this.moveState = newState;
                    this.sendMove(this.moveState);
                }
            },
            gameLoop: function(){
                
                this.gameContext.fillStyle = "#F7F7F7";
                
                this.gameContext.fillRect(0,0,600,600);
                
                this.drawPlayer(300,300, "#98DEFF", this.nick);
                
                this.players.forEach(p => {
                       const pX =  this.player.posX > p.posX ? 300 - (this.player.posX - p.posX) : 300 + (p.posX - this.player.posX);
                       const pY =  this.player.posY > p.posY ? 300 - (this.player.posY - p.posY) : 300 + (p.posY - this.player.posY);
                        if(pX > 0 && pX < 600 && pY > 0 && pY < 600){
                            this.drawPlayer(pX,pY, "#FB0000", p.name);
                        }
                });
                
                this.bullets.forEach(b => {
                    const pX =  this.player.posX > b.posX ? 300 - (this.player.posX - b.posX) : 300 + (b.posX - this.player.posX);
                    const pY =  this.player.posY > b.posY ? 300 - (this.player.posY - b.posY) : 300 + (b.posY - this.player.posY);
                    if(pX > 0 && pX < 600 && pY > 0 && pY < 600){
                        this.drawBullet(pX,pY);
                    }
                });
                
                /*
                 Sınırları cizen kod
                 */
                this.gameContext.beginPath();
                let x1 = 300 - this.player.posX;
                let y1= 300 - this.player.posY;
                
                if(this.player.posY <= 300){
                    this.gameContext.moveTo(x1 > 0 ? x1 : 0, y1);
                    this.gameContext.lineTo((this.currentRoom.sizeX - this.player.posX) + 310,y1);
                    this.gameContext.stroke();
                }
                
                if(this.player.posX <= 300){
                    this.gameContext.moveTo(x1, y1 > 0 ? y1 : 0);
                    this.gameContext.lineTo(x1,(this.currentRoom.sizeY - this.player.posY) + 310);
                    this.gameContext.stroke();
                }
                
                if((this.currentRoom.sizeX - this.player.posX) <= 300){
                    this.gameContext.moveTo(this.currentRoom.sizeX - this.player.posX + 310, y1 > 0 ? y1 : 0);
                    this.gameContext.lineTo(this.currentRoom.sizeX -this.player.posX + 310, (this.currentRoom.sizeY - this.player.posY) + 310);
                    this.gameContext.stroke();
                }
                
                if((this.currentRoom.sizeY - this.player.posY) <= 300) {
                    this.gameContext.moveTo(x1 > 0 ? x1: 0, (this.currentRoom.sizeY - this.player.posY) +310);
                    this.gameContext.lineTo((this.currentRoom.sizeX - this.player.posX) + 310, (this.currentRoom.sizeY - this.player.posY) + 310);
                    this.gameContext.stroke();
                }
            },
            drawPlayer: function(x,y,color, nick){
              this.gameContext.fillStyle = color;
              this.gameContext.beginPath();
              this.gameContext.rect(x, y, 10,10);
              const metric = this.gameContext.measureText(nick);
              this.gameContext.fillText(nick,x-(metric.width/2) + 5, y+20);
              this.gameContext.fill();
            },
            drawBullet: function(x,y){
                this.gameContext.fillStyle = "#000000";
                this.gameContext.beginPath();
                this.gameContext.rect(x,y, 3, 3);
                this.gameContext.fill();
            }
        },
        mounted: async function () {
                //console.log(this.viewState);
                await this.initSignalRConnection();
                this.viewState = 'nick';
        }
    }
);