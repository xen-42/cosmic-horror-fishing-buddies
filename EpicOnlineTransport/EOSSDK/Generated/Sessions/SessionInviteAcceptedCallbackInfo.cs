// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.Sessions
{
	/// <summary>
	/// Output parameters for the <see cref="OnSessionInviteAcceptedCallback" /> function.
	/// </summary>
	public class SessionInviteAcceptedCallbackInfo : ICallbackInfo, ISettable
	{
		/// <summary>
		/// Context that was passed into <see cref="SessionsInterface.AddNotifySessionInviteAccepted" />
		/// </summary>
		public object ClientData { get; private set; }

		/// <summary>
		/// Session ID that should be used for joining
		/// </summary>
		public string SessionId { get; private set; }

		/// <summary>
		/// The Product User ID of the user who accepted the invitation
		/// </summary>
		public ProductUserId LocalUserId { get; private set; }

		/// <summary>
		/// The Product User ID of the user who sent the invitation
		/// </summary>
		public ProductUserId TargetUserId { get; private set; }

		/// <summary>
		/// Invite ID that was accepted
		/// </summary>
		public string InviteId { get; private set; }

		public Result? GetResultCode()
		{
			return null;
		}

		internal void Set(SessionInviteAcceptedCallbackInfoInternal? other)
		{
			if (other != null)
			{
				ClientData = other.Value.ClientData;
				SessionId = other.Value.SessionId;
				LocalUserId = other.Value.LocalUserId;
				TargetUserId = other.Value.TargetUserId;
				InviteId = other.Value.InviteId;
			}
		}

		public void Set(object other)
		{
			Set(other as SessionInviteAcceptedCallbackInfoInternal?);
		}
	}

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
	internal struct SessionInviteAcceptedCallbackInfoInternal : ICallbackInfoInternal
	{
		private System.IntPtr m_ClientData;
		private System.IntPtr m_SessionId;
		private System.IntPtr m_LocalUserId;
		private System.IntPtr m_TargetUserId;
		private System.IntPtr m_InviteId;

		public object ClientData
		{
			get
			{
				object value;
				Helper.TryMarshalGet(m_ClientData, out value);
				return value;
			}
		}

		public System.IntPtr ClientDataAddress
		{
			get
			{
				return m_ClientData;
			}
		}

		public string SessionId
		{
			get
			{
				string value;
				Helper.TryMarshalGet(m_SessionId, out value);
				return value;
			}
		}

		public ProductUserId LocalUserId
		{
			get
			{
				ProductUserId value;
				Helper.TryMarshalGet(m_LocalUserId, out value);
				return value;
			}
		}

		public ProductUserId TargetUserId
		{
			get
			{
				ProductUserId value;
				Helper.TryMarshalGet(m_TargetUserId, out value);
				return value;
			}
		}

		public string InviteId
		{
			get
			{
				string value;
				Helper.TryMarshalGet(m_InviteId, out value);
				return value;
			}
		}
	}
}