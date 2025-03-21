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
const TEST_MODE = true;
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
        if(!TEST_MODE && applicant === this.id)return;
        if(this.nowHealth <= 0){ return; }
        for(const core of Object.values(coreList)) {
            if(core.transporting && core.transporter ==  this.id) {
                core.Damage(applicant, amount, true);
                return;
            }
        }
        this.nowHealth -= amount;
        
        connections[this.id].send(`System,SetHealth,${this.nowHealth}`);
        if(this.nowHealth <= 0)
        {
            this.Kill();
        }
    }
    //体力が無くなったときに
    Kill()
    {
        
        connections[this.id].broadcast(`Player,Deactivate,${this.id}`);
        const killedAt = this.position;
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
    Respawn(targetCoreId) {
        if(coreList[targetCoreId].Warp(this.id)) {
            this.ghost = false;
            this.nowHealth = this.defaultHealth;
            connections[this.id].send(`System,SetHealth,${this.nowHealth}`);
            connections[this.id].send(`System,SetPosition,${this.position}`);
            connections[this.id].broadcast(`Player,Activate,${this.id}`);
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
        this.transporter = null;
        this.defaultHealth = 10;
        this.nowHealth = this.defaultHealth;
    }
    Unclaim(position) {
        const lastOwner = this.owner;
        connections[lastOwner].send(`Core,Break,${this.id}`);
        server.sendAllClient(`Core,Place,${this.id},${position.join(',')}`);
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
            this.transporter = null;
            return true;
        }
        return false;

    }
    Transport(applicant) {
        if(applicant === this.owner) {
            this.transporting = true;
            this.transporter = applicant;
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
    Damage(applicant, amount, proxy=false) {
        if(playerList[applicant].ghost)return;
        if(!TEST_MODE && (this.owner === null || applicant === this.owner)) return;
        if(this.nowHealth <= 0){ return; }
        if(!proxy && this.transporting) {
            playerList[this.transporter].Damage(applicant, amount);
        }

        this.nowHealth -= amount;

        if(this.nowHealth <= 0)
        {
            this.Break(applicant);
        } else {
            if(this.owner) {
                connections[this.owner].send(`Core,Damage,${this.id},${this.nowHealth}`);
            }
        }
    }
    //体力が無くなったときに
    Break(applicant)
    {
        if(this.owner) {
            connections[this.owner].send(`Core,Break,${this.id}`);
            this.owner = null;

            if(this.Claim(applicant)) {
                connections[applicant].send(`Core,Claim,${this.id}`);
                if(this.Transport(applicant)) {
                    server.sendAllClient(`Core,Transport,${this.id},${this.transporter}`);
                }
            }

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

const server = new WebSocket.Server({ port: 8080 });

server.sendAllClient = text => {
    Object.values(connections).forEach( socket => {
        socket.send(text);
    });
}
server.on("connection", async (socket) => {
    connectionCounter++;
    console.log("connected");
    const id = makeId();
    connections[id] = socket;
    socket.broadcast = (text) => {
        Object.keys(connections).forEach( key => {
            if(key !== id) {
                connections[key].send(text);
            }
        });
    }

    socket.on("message", (msg) => {
        if(!msg)return console.log('noMSG:'+msg);
        const [ command, ...args ] = msg.toString().split(',');
        console.log(command,args);
        switch(command) {
            case "Entry":
                socket.send(`System,AsignId,${id}`);
                
                for(const player of Object.values(playerList)) {
                    if(player.ghost ) {
                        if(!player.gameOver) {
                            socket.send(`Player,Create,${player.id},${player.position.join(',')}`);
                            socket.send(`Player,Deactivate,${player.id}`);
                        }
                    } else {
                        socket.send(`Player,Create,${player.id},${player.position.join(',')}`);
                    }
                }
                
                for(const core of Object.values(coreList)) {
                    socket.send(`Core,Create,${core.id},${core.position.join(',')}`)
                    if(core.transporting) {
                        server.sendAllClient(`Core,Transport,${core.id},${core.transporter}`);
                    };
                }

                const createAt = [10+Math.random()*10-5,2.5,10+Math.random()*10-5];
                const playerCore = new Core(id, createAt);// psitionは仮
                playerCore.owner = id;
                coreList[id] = playerCore;
                const playerObj = new Player(id, createAt);;
                playerList[id] = playerObj
                server.sendAllClient(`Core,Create,${id},${createAt.join(',')}`);
                playerCore.Transport(id);
                server.sendAllClient(`Core,Transport,${playerCore.id},${id}`);


                socket.broadcast(`Player,Create,${id},${createAt.join(',')}`);
                socket.send(`Core,Claim,${playerCore.id}`);
                socket.send(`System,SetPosition,${playerObj.position.join(',')}`);
                break;
            case "Position":
                playerList[id].setPosition(args);
                socket.broadcast(`Player,Position,${id},${args.join(',')}`);
                break;
            case "Rotation":
                playerList[id].setRotation(args);
                socket.broadcast(`Player,Rotation,${id},${args.join(',')}`);
                break;
            case "ClaimRequest":
                if(coreList[args[0]].Claim(id)) {
                    socket.send(`Core,Claim,${args[0]}`);
                } else console.log("claimReq is denied",coreList[args[0]]);
                break;
            case "TransportRequest":
                if(coreList[args[0]].Transport(id)) {
                    server.sendAllClient(`Core,Transport,${args[0]},${id}`);
                }
                break;
            case "PlaceRequest":
                if(coreList[args[0]].Place(id)) {
                    server.sendAllClient(`Core,Place,${args[0]},${coreList[args[0]].position.join(',')}`);
                }
                break;
            case "RespawnRequest":
                playerList[id].Respawn(args[0]);
                break;
            case "WarpRequest":
                if(coreList[args[0]].Warp(id)) {
                    socket.send("System,SetPosition,"+playerList[id].position.join(','));
                };
                break;
            case "CoreDamageEntry":
                coreList[args[0]].Damage(id, +args[1]);
                break;
            case "PlayerDamageEntry":
                playerList[args[0]].Damage(id, +args[1]);
                break;
            case "ShootEntry":
                socket.broadcast(`System,Fireball,${args.join(',')}`);
                //server.sendAllClient(`System,Fireball,${args.join(',')}`);
                
                break;
            default:
                console.log("default:"+command);
                break;

        }
    });
    socket.on('close', () => {
      console.log('ws close');
//      delete connections[id];
    });
});
