// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Presence
{
	/// <summary>
	/// Data for the <see cref="PresenceModification.SetJoinInfo" /> function.
	/// </summary>
	public class PresenceModificationSetJoinInfoOptions
	{
		/// <summary>
		/// The string which will be advertised as this player's join info.
		/// An application is expected to freely define the meaning of this string to use for connecting to an active game session.
		/// The string should not exceed <see cref="PresenceModification.PresencemodificationJoininfoMaxLength" /> in length.
		/// This affects the ability of the Social Overlay to show game related actions to take in the player's social graph.
		/// 
		/// @note The Social Overlay can handle only one of the following three options at a time:
		/// using the bPresenceEnabled flags within the Sessions interface
		/// using the bPresenceEnabled flags within the Lobby interface
		/// using <see cref="PresenceModification.SetJoinInfo" />
		/// <seealso cref="Lobby.CreateLobbyOptions" />
		/// <seealso cref="Lobby.JoinLobbyOptions" />
		/// <seealso cref="Sessions.CreateSessionModificationOptions" />
		/// <seealso cref="Sessions.JoinSessionOptions" />
		/// </summary>
		public string JoinInfo { get; set; }
	}

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
	internal struct PresenceModificationSetJoinInfoOptionsInternal : ISettable, System.IDisposable
	{
		private int m_ApiVersion;
		private System.IntPtr m_JoinInfo;

		public string JoinInfo
		{
			set
			{
				Helper.TryMarshalSet(ref m_JoinInfo, value);
			}
		}

		public void Set(PresenceModificationSetJoinInfoOptions other)
		{
			if (other != null)
			{
				m_ApiVersion = PresenceModification.PresencemodificationSetjoininfoApiLatest;
				JoinInfo = other.JoinInfo;
			}
		}

		public void Set(object other)
		{
			Set(other as PresenceModificationSetJoinInfoOptions);
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_JoinInfo);
		}
	}
}