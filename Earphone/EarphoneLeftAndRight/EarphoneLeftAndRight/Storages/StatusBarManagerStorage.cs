using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace EarphoneLeftAndRight.Storages
{
	public static class StatusBarManagerStorage
	{
		private static bool isTileAdded = false;

		//WeakEventManager
		//https://learn.microsoft.com/xamarin/community-toolkit/helpers/weakeventmanagert
		//https://learn.microsoft.com/dotnet/api/system.windows.weakeventmanager
		//Or just hit F12 on Command.
		static readonly WeakEventManager _weakEventManager = new WeakEventManager();

		public static bool IsTileAdded
		{
			get => isTileAdded; set
			{
				isTileAdded = value;
				_weakEventManager.HandleEvent(isTileAdded, EventArgs.Empty, nameof(IsTileAddedChanged));
			}
		}
		//public static EventHandler? IsTileAddedChanged;
		public static event EventHandler IsTileAddedChanged
		{
			add { _weakEventManager.AddEventHandler(value); }
			remove { _weakEventManager.RemoveEventHandler(value); }
		}
	}
}
