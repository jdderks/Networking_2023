///Much credit for this code goes to "Mercenary Camp" on Youtube: https://youtu.be/lPoiTw0qjtc?list=PLmcbjnHce7SeAUFouc3X9zqXxiPbCz8Zp
///Thank you Mercenary Camp for the Unity networking Tutorial.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

using System;
using Unity.Collections;

public static class NetUtility
{
    public static Action<NetMessage> C_KEEP_ALIVE;
    public static Action<NetMessage> C_WELCOME;
    public static Action<NetMessage> C_START_GAME;
    public static Action<NetMessage> C_CUBE_CLICKED;
    public static Action<NetMessage> C_ASSIGN_TEAM;
    public static Action<NetMessage> C_REMATCH;

    public static Action<NetMessage, NetworkConnection> S_KEEP_ALIVE;
    public static Action<NetMessage, NetworkConnection> S_WELCOME;
    public static Action<NetMessage, NetworkConnection> S_START_GAME;
    public static Action<NetMessage, NetworkConnection> S_CUBE_CLICKED;
    public static Action<NetMessage, NetworkConnection> S_ASSIGN_TEAM;
    public static Action<NetMessage, NetworkConnection> S_REMATCH;

    public static void OnData(DataStreamReader stream, NetworkConnection cnn, Server server = null)
    {
        NetMessage message = null;
        var opCode = (OpCode)stream.ReadByte();
        switch (opCode)
        {
            case OpCode.KEEP_ALIVE: message = new NetKeepAlive(stream); break;
            case OpCode.WELCOME: message = new NetWelcome(stream); break;
            case OpCode.START_GAME: message = new NetStartGame(stream); break;
            case OpCode.CUBE_CLICKED: message = new NetCubeClicked(stream); break;
            case OpCode.ASSIGN_TEAM: message = new NetAssignTeam(stream); break;
            default:
                Debug.LogError("Message received had no OPCODE");
                break;
        }

        if (server != null)
            message.ReceivedOnServer(cnn);
        else
            message.ReceivedOnClient();
    }
}
