//const express = require('express');
//const http = require('http');
//
//// Socket.ioをインポート
//const socketIo = require('socket.io');
//
//const app = express();
//const server = http.Server(app);
//
//// 初期化
//const io = socketIo(server);

//const PORT = 3000;

/*app.get('/', (req, res) => {
  res.sendFile(__dirname + '/index.html');
});*/

//server.listen(PORT, () => {
//  console.log(`listening on port ${PORT}`);
//});

const connections = {};
const coreList = {};
const playerList = {};
let connectionCounter = 0;
function makeId() {
    return connectionCounter.toString();
}
// クライアントとのコネクションが確立したら'connected'という表示させる
/*io.on("connection", (socket) => {
    connectionCounter++;
    console.log("connected");
    const id = makeId();
    connections[id] = socket;
    
    socket.on("message", (msg) => {
        const [ command, ...args ] = msg.split(',');
        switch(command) {
            case "Entry":
                socket.emit("message",`System,AsignId,${id}`);
                const createAt = [10,10,10];
                coreList[id] = new Core(id, createAt);// psitionは仮
                playerList[id] = new Player(id, createAt);
                io.emit("message", `Core,Create,${id},${createAt.join(',')}`);
                io.broadcast.emit("message",`Player,Create,${id},${createAt.join(',')}`);
                break;
            case "Position":
                socket.broadcast.emit("message", `Player,Position,${id},${args.join(',')}`);
                break;
            case "Position":
                socket.broadcast.emit("message", `Player,Rotation,${id},${args.join(',')}`);
                break;
            case "ClaimRequest":
                if(coreList[arg[0]].Claim(id)) {
                    socket.emit("message",`Core,Claim,${arg[0]}`);
                }
            case "TransportRequest":
                if(coreList[arg[0]].Transport(id)) {
                    io.emit("message",`Core,Transport,${arg[0]},${id}`);
                }
                break;
            case "PlaceRequest":
                if(coreList[arg[0]].Place(id)) {
                    io.emit("message",`Core,Place,${arg[0]},${coreList[arg[0]].position.join(',')}`);
                }
                break;
            case "CoreDamageEntry":
                coreList[arg[0]].Damage(id, +arg[1]);
                break;
            case "CoreDamageEntry":
                playerList[arg[0]].Damage(id, +arg[1]);
                break;


        }
    });
    socket.on('close', () => {
      console.log('ws close');
    });
});*/

class Player {
    constructor(id, position) {
        this.id = id;
        this.position = position;
        this.rotation = [0,0,0];
        this.defaultHealth = 10;
        this.nowHealth = this.defaultHealth;
        this.gameOver = false;
        this.ghost = false;// リスポーン待機時等
    }
    setPosition(position) {
        this.position = position;
    }
    setRotation(rotation) {
        this.rotation = rotation;
    }
    
    Damage(applicant, amount) {
        if(applicant === this.id)return;
        if(this.nowHealth <= 0){ return; }

        this.nowHealth -= amount;
        
        connections[this.id].emit('message',`Player,Damage,${amount}`);
        if(nowHealth <= 0)
        {
            Kill();
        }
    }
    //体力が無くなったときに
    Kill()
    {
        
        connections[this.id].broadcast.emit("message", `Player,Deactivate,${this.id}`);
        killedAt = this.position;
        this.ghost = true;
        let gameOver = true;
        for(const key of Object.keys(coreList)) {
            if( coreList[key].owner === this.id) {
                if(coreList[key].transporting) {//運搬中のコアはロストする
                    coreList[key].Unclaim(killedAt);
                } else {
                    gameOver = false;
                }
            }
        }
        if(gameOver) {
            this.gameOver = true;
        }
    }
    Respown(targetCoreId) {
        if(coreList[targetCoreId].Warp(this.id)) {
            this.ghost = false;
            this.nowHealth = this.defaultHealth;
            connections[this.id].broadcast.emit("message", `Player,Activate,${this.id}`);
            return true;
        } else {
            this.gameOver = true;
            return false;
        }
    }
}

class Core {
    constructor(id, position) {
        this.id = id;
        this.position = position;
        this.owner = null;
        this.transporting = false;
        this.defaultHealth = 10;
        this.nowHealth = this.defaultHealth;
    }
    Unclaim(position) {
        const lastOwner = this.owner;
        connections[lastOwner].emit('message',`Core,Break,${this.id}`);
        io.emit("message",`Core,Place,${this.id},${position.join(',')}`);
        this.owner = null;
        this.transporting = false;
        this.nowHealth = 0;
        this.position = position;
    }
    Warp(applicant) {
        if(applicant === this.owner) {
            playerList[applicant].position = this.position;
            return true;
        }
        return false;
    }
    Place(applicant) {
        if(applicant === this.owner) {
            this.position = playerList[this.owner].position;
            this.transporting = false;
            return true;
        }
        return false;

    }
    Transport(applicant) {
        if(applicant === this.owner) {
            this.transporting = true;
            return true;
        }
        return false;
    }
    Claim(applicant) {
        if(playerList[applicant].ghost)return;
        if(this.nowHealth <= 0){
            this.nowHealth = this.defaultHealth;
            this.owner = applicant;
            return true;
        }
        return false;
    }
    Damage(applicant, amount) {
        if(playerList[applicant].ghost)return;
        if(this.owner === null || applicant === this.owner) return;
        if(this.nowHealth <= 0){ return; }

        this.nowHealth -= amount;

        if(nowHealth <= 0)
        {
            Break();
        } else {
            if(this.owner) {
                connections[this.owner].emit('message',`Core,Damage,${amount}`);
            }
        }
    }
    //体力が無くなったときに
    Break()
    {
        if(this.owner) {
            connections[this.owner].emit('message',`Core,Break,${this.id}`);
            this.owner = null;
        }
    }
}

const WebSocket = require("ws");

const { ApiPromise, WsProvider } = require("@polkadot/api");
const { ContractPromise } = require("@polkadot/api-contract");
const { Server } = require("http");

//const metadata = require("./metadata.json");

async function connect() {
  // Connect to the local development network
  const provider = new WsProvider("wss://rpc.shibuya.astar.network");
  const api = await ApiPromise.create({ provider });

  const contract = new ContractPromise(
    api,
    metadata,
    "a1MvMiL1VPNKHc6pb8XNAw6fcwh85fc8V3wgocpmJjYK1Tm"
  );

  console.log(contract.address.toHuman());

  return contract.address.toHuman();
}

const io = new WebSocket.Server({ port: 8080 });

server.on("connection", async (socket) => {
    connectionCounter++;
    console.log("connected");
    const id = makeId();
    connections[id] = socket;
    
    socket.on("message", (msg) => {
        if(!msg)return console.log('noMSG:'+msg);
        const [ command, ...args ] = msg.toString().split(',');
        switch(command) {
            case "Entry":
                socket.emit("message",`System,AsignId,${id}`);
                const createAt = [10,10,10];
                coreList[id] = new Core(id, createAt);// psitionは仮
                playerList[id] = new Player(id, createAt);
                server.emit("message", `Core,Create,${id},${createAt.join(',')}`);

                socket.broadcast.emit("message",`Player,Create,${id},${createAt.join(',')}`);
                break;
            case "Position":
                socket.broadcast.emit("message", `Player,Position,${id},${args.join(',')}`);
                break;
            case "Position":
                socket.broadcast.emit("message", `Player,Rotation,${id},${args.join(',')}`);
                break;
            case "ClaimRequest":
                if(coreList[arg[0]].Claim(id)) {
                    socket.emit("message",`Core,Claim,${arg[0]}`);
                }
            case "TransportRequest":
                if(coreList[arg[0]].Transport(id)) {
                    server.emit("message",`Core,Transport,${arg[0]},${id}`);
                }
                break;
            case "PlaceRequest":
                if(coreList[arg[0]].Place(id)) {
                    server.emit("message",`Core,Place,${arg[0]},${coreList[arg[0]].position.join(',')}`);
                }
                break;
            case "CoreDamageEntry":
                coreList[arg[0]].Damage(id, +arg[1]);
                break;
            case "CoreDamageEntry":
                playerList[arg[0]].Damage(id, +arg[1]);
                break;
            default:
                console.log(command);

        }
    });
    socket.on('close', () => {
      console.log('ws close');
//      delete connections[id];
    });
});
