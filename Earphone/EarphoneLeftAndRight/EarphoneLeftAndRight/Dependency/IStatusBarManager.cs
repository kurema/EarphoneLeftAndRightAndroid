using System;
using System.Collections.Generic;
using System.Text;

namespace EarphoneLeftAndRight.Dependency
{
	public interface IStatusBarManager
	{
		void RequestAddTileService();
		bool RequestAddTileServiceIsSupported { get; }
	}
}
