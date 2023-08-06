using Mirror;

namespace CosmicHorrorFishingBuddies.Messaging;

/// <summary>
/// Adapted from Quantum Space Buddies
/// </summary>
public abstract class CFBMessage
{
	public uint From;

	/// <summary>
	/// (default) uint.MaxValue = send to everyone <br/>
	/// 0 = send to Host
	/// </summary>
	public uint To = uint.MaxValue;

	/// <summary>
	/// Call base method when overriding
	/// </summary>
	/// <param name="writer"></param>
	public virtual void Serialize(NetworkWriter writer)
	{
		writer.Write(From);
		writer.Write(To);
	}

	/// <summary>
	/// Call base method when overriding
	/// </summary>
	public virtual void Deserialize(NetworkReader reader)
	{
		From = reader.Read<uint>();
		To = reader.Read<uint>();
	}

	public virtual bool ShouldReceive => true;
	public virtual void OnReceiveLocal() { }
	public virtual void OnReceiveRemote() { }
}

public abstract class CFBMessage<D> : CFBMessage
{
	public D Data { get; private set; }
	protected CFBMessage(D data) => Data = data;

	public override void Serialize(NetworkWriter writer)
	{
		base.Serialize(writer);
		writer.Write(Data);
	}

	public override void Deserialize(NetworkReader reader)
	{
		base.Deserialize(reader);
		Data = reader.Read<D>();
	}
}
