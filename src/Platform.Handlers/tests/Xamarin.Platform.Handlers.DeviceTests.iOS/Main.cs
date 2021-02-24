using UIKit;
using Xamarin.DeviceTestUtils;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	public class Application
	{
		static void Main(string[] args)
		{
			if (BaseTestApplicationDelegate.IsXHarnessRun(args))
				UIApplication.Main(args, null, nameof(TestApplicationDelegate));
			else
				UIApplication.Main(args, null, nameof(AppDelegate));
		}
	}
}
