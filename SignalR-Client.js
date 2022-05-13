
// Message Status Enums
const MessageSent = 0;
const MessageDelivered = 1;
const MessageUnread = 2;
const MessageRead = 3;


// Make initial connection to ChatHub
const connection = new signalR.HubConnectionBuilder()
    .configureLogging(signalR.LogLevel.Debug)
    .withUrl(hubUrl(),  {skipNegotiation: true,transport: signalR.HttpTransportType.WebSockets})
    .build();

// call start function
start();

// function to connect
async function start(){
    // Register UserId on Connect
    await connection.start().then(function () {
        console.log('SignalR Connected!');
        connection.invoke('RegisterConId', GetUserId())      
    }).catch(function (err) {
        console.error(err.toString());
    });
}

// Subscribe to 'BroadcastMessage' event 
connection.on("BroadcastMessage", (message) =>
{
    // update message delivery status to HUb
    message.messageStatusId = MessageDelivered;
    connection.invoke('UpdateMessageStatus',message);
    
    //emit 'ChatContext' event for UI
    var evt = document.createEvent('CustomEvent');
    evt.initCustomEvent('ChatContext', false, false, message);
    window.dispatchEvent(evt);
});

// Subscribe to 'MessageStatus' event which will be emit from UI
window.addEventListener("MessageStatus", function(evt) {
    connection.invoke('UpdateMessageStatus',evt.detail);
}, false);

// function to reconnect 
connection.onclose(async () => {
    // re-start the connection
    await start();
});

// ChatHub URL
function hubUrl(){
    return "https://signalrapi.omnizant.com/chathub";
}

// Get UserId from browser cookie
function GetUserId(){
    const UserId = document?.cookie?.split(';').find(rec => rec.includes('UserId'))?.split('=');
    return UserId ? parseInt(UserId[1]) : 0;
}



