Vue.config.devtools = true;

const vue = new Vue({
    el: '#dashboard-wrapper',
    delimiters: ['${', '}}'],
    data: {
       rooms: [], 
       conn: null,
   },
   methods:{
        startConn: async function () {
           try {
               await this.conn.start();
               console.log("SignalR Connected to dashboard");
           } catch (err) {
               console.log(err);
               setTimeout(this.startConn, 2500);
           }
       },
       init: async function () {

           this.conn = new signalR.HubConnectionBuilder().withUrl("/dashboardHub")
               .configureLogging(signalR.LogLevel.Information).build();

           await this.startConn();
           
           this.conn.onclose(this.startConn);

           this.conn.on("onInit", (message) => {
               this.rooms = message || [];
           });
       },
       disconnect: function (user){
            this.conn.invoke("onDisconnectUser", user).then();
       }
   },
   mounted: async function(){
        await this.init();
   }
});