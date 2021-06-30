Vue.config.devtools = true;

const vue = new Vue({
    el:"#vue-wrapper",
    delimiters: ['${', '}}'],
    data: {
        connection: null,
        nick: null,
        viewState : 'nick',
        errorMessage: null,
        gameRooms: [],
        newRoomName: null,
        currentRoom: null,
    },
    methods:{
        startConnection: async function(){
            try{
                await this.connection.start()
                console.log("SignalR Connected");
            } catch(err){
                console.log(err);
                setTimeout(this.startConnection, 2500);
            }
        },
        initSignalRConnection: async function(){
          this.connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:5001:gameHub")
              .configureLogging(signalR.LogLevel.Information).build();
          
          this.initializeSignalRMethods();
          
          this.connection.onclose(this.startConnection);
          
          await this.startConnection();
        },
        nickInput: async function(){
            this.connection.on("onError", (method, message) => {
                this.errorMessage = "Error : " + method + message;
            });
            
            
            
            
        },
    },
    mounted:{
        
    }
});