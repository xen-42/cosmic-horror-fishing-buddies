using CosmicHorrorFishingBuddies.Core;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CosmicHorrorFishingBuddies.Util;

public static class Extensions
{
	public static IEnumerable<Type> GetDerivedTypes(this Type type) =>
		type.Assembly
			.GetTypes()
			.Where(x => !x.IsInterface && !x.IsAbstract && type.IsAssignableFrom(x))
			.OrderBy(x => x.FullName);

	public static NetworkConnectionToClient GetNetworkConnection(this uint playerId)
	{
		var conn = NetworkServer.connections.Values.FirstOrDefault(x => playerId == x.GetPlayerId());
		if (conn == default)
		{
			CFBCore.LogError($"GetNetworkConnection on {playerId} found no connection\n{Environment.StackTrace}");
		}

		return conn;
	}

	public static uint GetPlayerId(this NetworkConnectionToClient conn)
	{
		if (conn.identity == null)
		{
			CFBCore.LogError($"GetPlayerId on {conn} has no identity.");
			return uint.MaxValue;
		}

		return conn.identity.netId;
	}
}
