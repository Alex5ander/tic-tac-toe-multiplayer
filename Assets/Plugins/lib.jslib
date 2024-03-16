/* <script crossorigin="anonymous" src="https://cdn.socket.io/4.7.5/socket.io.min.js"></script> */
mergeInto(LibraryManager.library, {
  socket: null,
  DisconnectWebGL: function () {
    this.socket.disconnect();
  },
  ConnectWebGL: function () {
    this.socket = io("https://tic-tac-toe-multiplayer-server.onrender.com/");

    this.socket.on('connect', () => {
      gameInstance.SendMessage('Socket', 'OnConnected');
    });

    this.socket.on("simbol", data => {
      gameInstance.SendMessage('Socket', 'OnSimbol', data);
    });
    this.socket.on("your-turn", _ => {
      gameInstance.SendMessage('Socket', 'OnYourTurn');
    });
    this.socket.on("start", _ => {
      gameInstance.SendMessage('Socket', 'OnStartGame');
    });
    this.socket.on("update", data => {
      gameInstance.SendMessage('Socket', "OnUpdate", JSON.stringify(data));
    });
    this.socket.on("win", _ => {
      gameInstance.SendMessage('Socket', "OnEndGame", 1);
    });
    this.socket.on("end-game", _ => {
      gameInstance.SendMessage('Socket', "OnEndGame", 0);
    });
    this.socket.on("disconnect", _ => {
      gameInstance.SendMessage('Socket', 'OnDisconnect');
    });
  },
  ClickWebGL: function (index) {
    this.socket.emit("click", index, () => {
      gameInstance.SendMessage('Socket', 'OpponentTurn');
    });
  }
});
