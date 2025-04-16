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
const TEST_MODE = false;
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
const base = 2;
const CORE_REPAIR_FACTOR_ON_TRANSPORTING = base*1.5;
const CORE_REPAIR_FACTOR_ON_PLACED = base;
const CORE_DEFAULT_HEALTH = 20;
const PLAYER_DEFAULT_HEALTH = 4;
const FIREBALL_DAMAGE = 2;
const REVIVAL_HEALTH_RATE = 0.2;
const CORE_WARP_COST = 50;//消費するHP
const ANGEL_MODE_TIME = 16;
const CORE_ANGEL_COST = 4;//4;//放棄するコア数

const SYSTEM_PLAYER_NAME = "SYSTEM";
//コアが破壊された後にだれにも移送されずに同じ人に再取得されるのを防ぐ時間
const CORE_SYSTEM_PROTECT_TIME = 10;
class Player {
    constructor(id, position) {
        this.id = id;
        this.position = position;
        this.rotation = [0,0,0];
        this.defaultHealth = PLAYER_DEFAULT_HEALTH;
        this.nowHealth = this.defaultHealth;
        this.gameOver = false;
        this.ghost = false;// リスポーン待機時等
        this.revivalHealthRate = REVIVAL_HEALTH_RATE;
        this.lastAngelMode = Date.now() - ANGEL_MODE_TIME * 1000;
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
        this.nowHealth -= amount;
        
        if(this.nowHealth <= 0)
        {
            const revival = this.Kill(applicant);
            console.log('revival',revival);
            connections[this.id].send(`System,SetHealth,${this.nowHealth}`);
            if(!revival) {
                connections[this.id].send(`System,Rank,${GetRank()}`);
            }
        } else {
            connections[this.id].send(`System,SetHealth,${this.nowHealth}`);
        }
    }
    System_RevivalByCore() {
        
        this.nowHealth = this.revivalHealthRate * this.defaultHealth;
        connections[this.id].broadcast(`System,Revival,${this.id}`);
    }
    //体力が無くなったときに
    Kill(applicant)
    {
        //コアを持っていたらコアが身代わりになる
        for(const core of Object.values(coreList)) {
            if(core.transporting && core.transporter ==  this.id) {
                //this.Respawn(core.id);
                core.Break(applicant);
                this.System_RevivalByCore();
                return true;
            }
        }
        
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
        if(defenceZonePlayers.includes(this.id)) {
            defenceZonePlayers = defenceZonePlayers.filter(id => id !== this.id);
            checkDefenceZone();
        }
        if(gameOver) {
            this.gameOver = true;
            let restPlayer = Object.values(playerList).filter(p=>!p.gameOver);
            //誰かがゲームオーバーしてまだ残ってる時点で二人以上いる。
            if(restPlayer.length === 1){
                connections[restPlayer[0].id].send(`System,Rank,${1}`);
                connections[restPlayer[0].id].send(`System,GameEnd`);
            }
        }
        return false;
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
        this.defaultHealth = CORE_DEFAULT_HEALTH;
        this.nowHealth = this.defaultHealth;
        //this.lastWarpedTime = Date.now();
        this.lastRepaired = null;
        this.repairAmountOnPlacedPerSec = CORE_REPAIR_FACTOR_ON_PLACED;
        this.repairAmountOnTransportingPerSec = CORE_REPAIR_FACTOR_ON_TRANSPORTING;
        this.warpCost = CORE_WARP_COST;//(Health)
        this.lastBreaked = Date.now() - 10000;
        this.lastOwner = null;
        //this.warpCoolTime = 10;
    }
    System_Repair() {//Call before Damage()
        //if(this.transporting)return;

        if(this.nowHealth < this.defaultHealth) {//元々体力が満タンではない
            const elapsedTime = Date.now() - this.lastRepaired;
            let repairFactor = this.transporting 
                ? this.repairAmountOnTransportingPerSec
                : this.repairAmountOnPlacedPerSec;
            //コアの設置＆移動開始時には必ず呼び出す
            this.nowHealth += repairFactor * elapsedTime/1000;
        }
        if(this.nowHealth > this.defaultHealth) {
            this.nowHealth = this.defaultHealth;
            //connections[this.owner].send(`System,CoreIsFull,${this.id}`);
        } else {
            //this.lastRepaired = Date.now();
        }
        //絶対にダメージなどHPに変更を加える前に呼ぶ
        this.lastRepaired = Date.now();
    }
    Unclaim(position) {
        this.lastOwner = this.owner;
        //connections[this.lastOwner].send(`Core,Damage,${this.id},${0}`);
        connections[this.lastOwner].send(`Core,Break,${this.id}`);
        server.sendAllClient(`Core,Place,${this.id},${position.join(',')}`);
        this.owner = null;
        this.transporting = false;
        this.nowHealth = 0;
        this.position = position;
        this.lastBreaked = Date.now();
        console.log("Unclaim",this.id,this.owner);
    }
    UseHealth(amount) {
        this.System_Repair(amount);
        if(this.nowHealth > amount) {
            this.nowHealth -= amount;
            connections[this.owner].send(`Core,Damage,${this.id},${this.nowHealth}`);
            return true;
        }
        return false;
    }
    Warp(applicant) {
        if(applicant === this.owner && !this.transporting) {
            //if(Date.now() - this.lastWarpedTime > this.warpCoolTime * 1000) {
            //    playerList[applicant].position = this.position;
            //    this.lastWarpedTime = Date.now();
            //    return true;
            //}
            if(this.UseHealth(this.warpCost)) {  
                playerList[applicant].position = this.position;
                this.lastWarpedTime = Date.now();
                return true;
            }
        }
        return false;
    }
    Place(applicant) {
        if(applicant === this.owner) {
            this.System_Repair();
            this.position = playerList[this.owner].position;
            this.transporting = false;
            this.transporter = null;
            return true;
        }
        return false;

    }
    Transport(applicant) {
        
        if(applicant === this.owner) {
            this.System_Repair();
            this.transporting = true;
            this.transporter = applicant;
            return true;
        }
        return false;
    }
    Claim(applicant) {
        if(playerList[applicant].ghost)return false;
        if(Date.now() - this.lastBreaked < CORE_SYSTEM_PROTECT_TIME*1000 && this.lastOwner === applicant)return false;
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
        //下記のコードの目的が、プレイヤーの周りの火球に弾が当たった時プレイヤーにも反映する
        //以外の意図が読み取れなかったので無効化
        // => 当たり判定の関係で必要だった
        if(!proxy && this.transporting) {
            playerList[this.transporter].Damage(applicant, amount);
        }

        this.System_Repair(amount);
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
            this.nowHealth = 0;
            connections[this.owner].send(`Core,Break,${this.id}`);
            this.lastOwner = this.owner;
            this.owner = null;
            if(applicant) {
                if(this.Claim(applicant)) {
                    connections[applicant].send(`Core,Claim,${this.id}`);
                    if(this.Transport(applicant)) {
                        server.sendAllClient(`Core,Transport,${this.id},${applicant}`);
                    }
                }
            }
            this.lastBreaked = Date.now();
        }
    }
}
function GetRank() {
    let counter = 1;
    for(const player of Object.values(playerList)) {
        if(!player.gameOver)counter++;
    }
    return counter;
}
playerList["GameOwner"] = new Player(SYSTEM_PLAYER_NAME, [-1000,-1000,-1000]);

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

const server = new WebSocket.Server({ 
    host: 'localhost',
    port: 8080 
});

server.sendAllClient = text => {
    Object.values(connections).forEach( socket => {
        socket.send(text);
    });
}
function getOwnedCores(id) {
    return Object.values(coreList).filter( (core) => core.owner === id );
}
let defenceZonePlayers = [];
let defenceZoneClaiming = null;
server.on("connection", async (socket) => {
    connectionCounter++;
    console.log("connected");
    const id = makeId();
    connections[id] = socket;
    socket.broadcast = (text) => {
        Object.entries(connections).forEach( ([key, connection]) => {
            if(key !== id) {
                connection.send(text);
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
                socket.send(`System,Information,${
                    PLAYER_DEFAULT_HEALTH
                },${
                    CORE_DEFAULT_HEALTH
                },${
                    CORE_REPAIR_FACTOR_ON_PLACED
                },${
                    CORE_REPAIR_FACTOR_ON_TRANSPORTING
                }`);
                
                for(const player of Object.values(playerList)) {
                    if(player.id === SYSTEM_PLAYER_NAME)continue;
                    if(player.ghost ) {
                        if(!player.gameOver) {
                            socket.send(`Player,Create,${player.id},${player.position.join(',')}`);
                            socket.send(`Player,Deactivate,${player.id}`);
                        }
                    } else {
                        socket.send(`Player,Create,${player.id},${player.position.join(',')}`);
                        socket.send(`Player,Rotation,${player.id},${player.rotation.join(',')}`);
                    }
                }
                
                for(const core of Object.values(coreList)) {
                    socket.send(`Core,Create,${core.id},${core.position.join(',')}`);
                    if(core.transporting) {
                        server.sendAllClient(`Core,Transport,${core.id},${core.transporter}`);
                    };
                }
                //setTimeout(()=>{
                const createAt = [
                    Math.random()*50,
                    24,
                    Math.random()*50
                ];
                const playerCore = new Core(id, createAt);// psitionは仮
                playerCore.owner = id;
                coreList[id] = playerCore;
                const playerObj = new Player(id, createAt);
                playerList[id] = playerObj
                server.sendAllClient(`Core,Create,${id},${createAt.join(',')}`);
                playerCore.Transport(id);
                server.sendAllClient(`Core,Transport,${playerCore.id},${id}`);


                socket.broadcast(`Player,Create,${id},${createAt.join(',')}`);
                //broadcast(id,`Player,Create,${id},${createAt.join(',')}`);
                socket.send(`System,SetPosition,${playerObj.position.join(',')}`);
                socket.send(`Core,Claim,${playerCore.id}`);
                console.log("SYSTEM_ENTRY_END");
                //},100);
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
                    let flag = true;
                    const cores = Object.values(coreList);
                    for(const core of cores) {
                        if(core.owner !== id)flag = false;
                    }
                    if(flag && cores.length >= 2) {
                        //socket.send(`System,SetHealth,${this.nowHealth}`);
                        socket.send(`System,Rank,${1}`);
                        //Object.values(playerList).filter(p=>!p.gameOver).map(p=>{
                        //    return {
                        //        cores.filter(c => c.owner === p.id)
                        //    };
                        //})
                        for(const player of Object.values(playerList).filter(p=>!p.gameOver)) {
                            connections[player.id].send(`System,Rank,${2}`);
                        }
                        socket.send(`System,GameEnd`);
                    }
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
                if(playerList[id].Respawn(args[0])) {
                    socket.broadcast(`Player,Position,${id},${playerList[id].position.join(',')}`);
                }
                break;
            case "WarpRequest":
                if(coreList[args[0]].Warp(id)) {
                    socket.send("System,SetPosition,"+playerList[id].position.join(','));
                    socket.broadcast(`Player,Position,${id},${playerList[id].position.join(',')}`);
                };
                break;
            case "CoreDamageEntry":
                coreList[args[0]].Damage(id, FIREBALL_DAMAGE);
                break;
            case "PlayerDamageEntry":
                playerList[args[0]].Damage(id, FIREBALL_DAMAGE);
                break;
            case "ShootEntry":
                socket.broadcast(`System,Fireball,${args.join(',')}`);
                //server.sendAllClient(`System,Fireball,${args.join(',')}`);
                
                break;
            case "GetRank":
                socket.send(`System,Rank,${GetRank()}`);
                break;
            case "SelfDamage":
                playerList[id].Damage(args[0], +args[1]);
                break;
            
            case "SelfDamageCore":
                coreList[args[1]].Damage(args[0], +args[2]);
                break;
            case "AngelEntry":
                if(Date.now() - playerList[id].lastAngelMode < ANGEL_MODE_TIME * 1000)break;
                const ownedCores = getOwnedCores(id);
                if(ownedCores.length >= CORE_ANGEL_COST) {
                    for(let i = 0;i < CORE_ANGEL_COST;i++) {
                        //AngelModeになるとコアを落とす。取られる可能性有
                        ownedCores[i].Unclaim(playerList[id].position);
                    }
                    playerList[id].lastAngelMode = Date.now();
                    socket.send("System,Angel");
                }
                break;
            case "EnterDefenceZone":
                defenceZonePlayers.push(id);
                checkDefenceZone();
                break;
            case "ExitDefenceZone":
                defenceZonePlayers = defenceZonePlayers.filter(f=>f!==id);
                checkDefenceZone();
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
let timeout = null;
function checkDefenceZone() {
    if(defenceZonePlayers.length === 1) {
        connections[defenceZonePlayers[0]].send("System,ClaimingDefenceZone");
        defenceZoneClaiming = defenceZonePlayers[0];
        timeout = setTimeout(()=>{
            if(!defenceZoneClaiming)return;
            const coreId = defenceZoneClaiming+"$"+Date.now();
            const createAt = playerList[defenceZoneClaiming].position;
            const playerCore = new Core(coreId, createAt);// psitionは仮
            coreList[coreId] = playerCore;
            playerCore.owner = defenceZoneClaiming;
            server.sendAllClient(`Core,Create,${coreId},${createAt.join(',')}`);
            playerCore.Transport(defenceZoneClaiming);
            connections[defenceZoneClaiming].send(`Core,Claim,${coreId}`);
            server.sendAllClient(`Core,Transport,${coreId},${defenceZoneClaiming}`);
        },32*1000);
    } else if(defenceZoneClaiming) {
        connections[defenceZoneClaiming].send("System,CancelClaimingDefenceZone");
        defenceZoneClaiming = null;
        if(timeout)clearTimeout(timeout);
        timeout = null;
    }
}
