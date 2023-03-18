# TCP Game

Client - server architecture game using TCP.

## Server
The server is waiting for two clients. When two clients are connected, it sends them their IDs. Then it waits for a number from each client. After receiving the numbers the server sends clients information if client has won or has lost. The winner is a client with a greater number.

## Client
The client connects with the server and send it a random number. Then it receives a results and prints it to the console.

# Running
To run server:
```
dotnet run --project ./src/Server/
```
To run client:
```
dotnet run --project ./src/Client/
```
