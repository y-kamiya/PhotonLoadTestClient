// ----------------------------------------------------------------------------
// <copyright file="CustomTypes.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

#pragma warning disable 1587
/// \file
/// <summary>Sets up support for Unity-specific types. Can be a blueprint how to register your own Custom Types for sending.</summary>
#pragma warning restore 1587


using System.IO;
using System.Runtime.InteropServices;
using ExitGames.Client.Photon;
using System;
using UnityEngine;



/// <summary>
/// Internally used class, containing de/serialization methods for various Unity-specific classes.
/// Adding those to the Photon serialization protocol allows you to send them in events, etc.
/// </summary>
public static class CustomTypes
{
    /// <summary>Register</summary>
    public static void Register()
    {
        PhotonPeer.RegisterType(typeof(Vector3), (byte)'V', SerializeVector3, DeserializeVector3);
    }


    #region Custom De/Serializer Methods


    public static readonly byte[] memVector3 = new byte[3 * 4];
    private static short SerializeVector3(MemoryStream outStream, object customobject)
    {
        Vector3 vo = (Vector3)customobject;

        int index = 0;
        lock (memVector3)
        {
            byte[] bytes = memVector3;
            Protocol.Serialize(vo.x, bytes, ref index);
            Protocol.Serialize(vo.y, bytes, ref index);
            Protocol.Serialize(vo.z, bytes, ref index);
            outStream.Write(bytes, 0, 3 * 4);
        }

        return 3 * 4;
    }

    private static object DeserializeVector3(MemoryStream inStream, short length)
    {
        Vector3 vo = new Vector3();
        lock (memVector3)
        {
            inStream.Read(memVector3, 0, 3 * 4);
            int index = 0;
            Protocol.Deserialize(out vo.x, memVector3, ref index);
            Protocol.Deserialize(out vo.y, memVector3, ref index);
            Protocol.Deserialize(out vo.z, memVector3, ref index);
        }

        return vo;
    }

    #endregion
}
