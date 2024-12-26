// Copyright Epic Games, Inc. All Rights Reserved.
// This file is automatically generated. Changes to this file may be overwritten.

namespace Epic.OnlineServices.RTCAudio
{
	/// <summary>
	/// This struct is used to inform the audio system of a user.
	/// </summary>
	public class RegisterPlatformAudioUserOptions
	{
		/// <summary>
		/// Platform dependent user id.
		/// </summary>
		public string UserId { get; set; }
	}

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
	internal struct RegisterPlatformAudioUserOptionsInternal : ISettable, System.IDisposable
	{
		private int m_ApiVersion;
		private System.IntPtr m_UserId;

		public string UserId
		{
			set
			{
				Helper.TryMarshalSet(ref m_UserId, value);
			}
		}

		public void Set(RegisterPlatformAudioUserOptions other)
		{
			if (other != null)
			{
				m_ApiVersion = RTCAudioInterface.RegisterplatformaudiouserApiLatest;
				UserId = other.UserId;
			}
		}

		public void Set(object other)
		{
			Set(other as RegisterPlatformAudioUserOptions);
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_UserId);
		}
	}
}