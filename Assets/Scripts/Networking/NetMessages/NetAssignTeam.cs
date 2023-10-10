using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class NetAssignTeam : NetMessage
{
    public int teamToAssign;
    //public bool randomTeam;

    public NetAssignTeam()
    {
        Code = OpCode.ASSIGN_TEAM;
    }

    public NetAssignTeam(DataStreamReader reader)
    {
        Code = OpCode.ASSIGN_TEAM;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(teamToAssign);
        //writer.WriteByte((byte)(randomTeam == true ? 1 : 0));

    }

    public override void Deserialize(DataStreamReader reader)
    {
        //byte is already read in OnData
        teamToAssign = reader.ReadInt();
        //randomTeam = reader.ReadByte() == 1 ? true : false;
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_ASSIGN_TEAM?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_ASSIGN_TEAM?.Invoke(this, cnn);
    }
}
