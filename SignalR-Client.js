
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
function start(){
    // Register UserId on Connect
    connection.start().then(function () {
        console.clear();
        connection.invoke('RegisterConId', GetUserId())      
    }).catch(function (err) {
        console.error(err.toString());
    });
}

// Subscribe to 'BroadcastMessage' event 
connection.on("BroadcastMessage", (message) =>
{
    // update message delivery status to HUb
    var messageModel = JSON.parse(message);
    messageModel.messageStatusId = MessageDelivered;
    if(messageModel && messageModel.Data){
        
        if(!MsgIds.includes(messageModel.MessageSid)){
            MsgIds.push(messageModel.MessageSid);
            const msgBody = JSON.parse(messageModel.Data);
            PopupNotification(msgBody.Body, "success");
            
            //emit 'ChatContext' event for UI
            var evt = document.createEvent('CustomEvent');
            evt.initCustomEvent('ChatContext', false, false, messageModel);
            window.dispatchEvent(evt);

            setTimeout(function(){
                MsgIds = MsgIds.filter(msgId => msgId != messageModel.MessageSid);
            }, 1000);
        } 
    }
    
    connection.invoke('UpdateMessageStatus',messageModel);
});

// Subscribe to 'MessageStatus' event which will be emit from UI
window.addEventListener("MessageStatus", function(evt) {
    connection.invoke('UpdateMessageStatus',evt.detail);
}, false);

// function to reconnect 
connection.onclose(() => {
    // re-start the connection
    start();
});

// ChatHub URL
function hubUrl(){
    // return "https://localhost:44320/chathub";
    return "https://*.azurewebsites.net/chathub";
}

// Get UserId from browser cookie
function GetUserId(){
    const UserId = document?.cookie?.split(';').find(rec => rec.includes('UserId'))?.split('=');
    return UserId ? parseInt(UserId[1]) : 0;
}

MsgIds = [];

