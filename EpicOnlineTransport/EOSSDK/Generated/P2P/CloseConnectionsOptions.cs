// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.P2P
{
	/// <summary>
	/// Structure containing information about who would like to close connections, and by what socket ID
	/// </summary>
	public class CloseConnectionsOptions
	{
		/// <summary>
		/// The Product User ID of the local user who would like to close all connections that use a particular socket ID
		/// </summary>
		public ProductUserId LocalUserId { get; set; }

		/// <summary>
		/// The socket ID of the connections to close
		/// </summary>
		public SocketId SocketId { get; set; }
	}

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
	internal struct CloseConnectionsOptionsInternal : ISettable, System.IDisposable
	{
		private int m_ApiVersion;
		private System.IntPtr m_LocalUserId;
		private System.IntPtr m_SocketId;

		public ProductUserId LocalUserId
		{
			set
			{
				Helper.TryMarshalSet(ref m_LocalUserId, value);
			}
		}

		public SocketId SocketId
		{
			set
			{
				Helper.TryMarshalSet<SocketIdInternal, SocketId>(ref m_SocketId, value);
			}
		}

		public void Set(CloseConnectionsOptions other)
		{
			if (other != null)
			{
				m_ApiVersion = P2PInterface.CloseconnectionsApiLatest;
				LocalUserId = other.LocalUserId;
				SocketId = other.SocketId;
			}
		}

		public void Set(object other)
		{
			Set(other as CloseConnectionsOptions);
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_LocalUserId);
			Helper.TryMarshalDispose(ref m_SocketId);
		}
	}
}