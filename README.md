# Signal-R-ChatHub with Redis-Backplane

- A multi-node Realtime Hi-Scaled Chat-Hub: Desiged with
  1. ASP.NET Core 3.1
  1. Microsoft.Azure.SignalR 1.4.0
  2. Microsoft.AspNetCore.SignalR.StackExchangeRedis 3.1.0


![download](https://user-images.githubusercontent.com/35859780/171971897-743f9fc8-62ba-4727-a760-b096e3dab5bf.png)


- This service can be used to send notification, message and email to its client.  
- The SignalR Servers and Clients are using TCP/IP connection to communicate.
- This SignalR API is designed to handle any number of client connection by adding more nodes.
- When, we have many nodes one node wont be able to communicate with another node without a "Backplane"
- I am using "Redis-Backplane" to resolve communication among nodes.
- If one node want to send message to all nodes, we can achieve using this design.
