﻿using NetworkCore;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TcpServer.Session;
using System.Diagnostics;
using TcpServer.Manager;
using TcpServer;




//Manager class init
var Gmanager = GlobalManager.Instance; ;

RoomServerLauncher launcher = new RoomServerLauncher();
var connector = new Connector();

RoomServerManager roomServerManager = new RoomServerManager(launcher,connector);
roomServerManager.CreateAndConnectRoom("31005");

Listener _listener = new Listener();

string host = Dns.GetHostName();
IPHostEntry iphost = Dns.GetHostEntry(host); //현재 컴퓨터의 호스트
IPAddress ipAddr = IPAddress.Any;

Debug.Assert(!(ipAddr == null));

IPEndPoint endPoint = new IPEndPoint(ipAddr!, 7777); //ipEndPoint에 아이피와 포트 데이터

_listener.Init(endPoint, () => { return SessionManager.Instance.Generator();});

Console.Write("ready to listening ......");

while(true)
{

}

