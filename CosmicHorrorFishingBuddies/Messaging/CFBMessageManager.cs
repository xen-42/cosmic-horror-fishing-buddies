using CosmicHorrorFishingBuddies.Core;
using CosmicHorrorFishingBuddies.PlayerSync;
using CosmicHorrorFishingBuddies.Util;
using Mirror;
using Mirror.SimpleWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace CosmicHorrorFishingBuddies.Messaging;

public static class CFBMessageManager
{
	internal static readonly Type[] _types;
	internal static readonly Dictionary<Type, ushort> _typeToId = new();

	public static bool IsRemote { get; private set; }

	static CFBMessageManager()
	{
		_types = typeof(CFBMessage).GetDerivedTypes().ToArray();
		for (ushort i = 0; i < _types.Length; i++)
		{
			_typeToId.Add(_types[i], i);
			// call static constructor of message if needed
			RuntimeHelpers.RunClassConstructor(_types[i].TypeHandle);
		}
	}

	public static void Init()
	{
		NetworkServer.RegisterHandler<Wrapper>((_, wrapper) => OnServerReceive(wrapper));
		NetworkClient.RegisterHandler<Wrapper>(wrapper => OnClientReceive(wrapper));
	}

	private static void OnServerReceive(CFBMessage msg)
	{
		if (msg.To == uint.MaxValue)
		{
			NetworkServer.SendToAll<Wrapper>(msg);
		}
		else if (msg.To == 0)
		{
			NetworkServer.localConnection.Send<Wrapper>(msg);
		}
		else
		{
			var connection = msg.To.GetNetworkConnection();

			if (connection == default)
			{
				CFBCore.LogWarning($"Tried to handle message from disconnected(?) player.");
				return;
			}

			connection.Send<Wrapper>(msg);
		}
	}

	private static void OnClientReceive(CFBMessage msg)
	{
		if (PlayerTransformSync.LocalInstance == null)
		{
			CFBCore.LogWarning($"Tried to handle message {msg} before local player was established.");
			return;
		}

		CFBCore.LogInfo("GOT MESSAGE");

		try
		{
			if (!msg.ShouldReceive)
			{
				return;
			}

			if (msg.From != PlayerManager.LocalNetID)
			{
				IsRemote = true;
				msg.OnReceiveRemote();
				IsRemote = false;
			}
			else
			{
				msg.OnReceiveLocal();
			}
		}
		catch (Exception ex)
		{
			CFBCore.LogError($"Exception handling message {msg} : {ex}");
		}
	}

	public static void Send<M>(this M msg)
		where M : CFBMessage
	{
		if (PlayerTransformSync.LocalInstance == null)
		{
			CFBCore.LogWarning($"Tried to send message {msg} before local player was established.");
			return;
		}

		msg.From = PlayerManager.LocalNetID;
		NetworkClient.Send<Wrapper>(msg);
	}
}

internal struct Wrapper : NetworkMessage
{
	public CFBMessage Msg;

	public static implicit operator CFBMessage(Wrapper wrapper) => wrapper.Msg;
	public static implicit operator Wrapper(CFBMessage msg) => new() { Msg = msg };
}

public static class ReaderWriterExtensions
{
	private static CFBMessage ReadCFBMessage(this NetworkReader reader)
	{
		var id = reader.ReadUShort();
		var type = CFBMessageManager._types[id];
		var msg = (CFBMessage)FormatterServices.GetUninitializedObject(type);
		msg.Deserialize(reader);
		return msg;
	}

	private static void WriteCFBMessage(this NetworkWriter writer, CFBMessage msg)
	{
		var type = msg.GetType();
		var id = CFBMessageManager._typeToId[type];
		writer.Write(id);
		msg.Serialize(writer);
	}
}