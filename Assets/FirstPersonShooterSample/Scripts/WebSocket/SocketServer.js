const express = require('express');
const http = require('http');

// Socket.ioをインポート
const socketIo = require('socket.io');

const app = express();
const server = http.Server(app);

// 初期化
const io = socketIo(server);

const PORT = 3000;

/*app.get('/', (req, res) => {
  res.sendFile(__dirname + '/index.html');
});*/

server.listen(PORT, () => {
  console.log(`listening on port ${PORT}`);
});

const connections = {};
let connectionCounter = 0;
function makeId() {
    return connectionCounter.toString();
}
// クライアントとのコネクションが確立したら'connected'という表示させる
io.on('connection', (socket) => {
    connectionCounter++;
    console.log('connected');
    const id = makeId();
    connections[id] = socket;
    
    socket.on('message', (msg) => {
        const [ command, ...args ] = msg.split(',');
        switch(command) {
            case "deactivate":
                break;
            case "activate":
                break;
        }
    });
    socket.on('close', () => {
      console.log('ws close');
    });
});