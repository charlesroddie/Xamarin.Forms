using System.Collections.Generic;
using System.Reflection;
using Foundation;
using Microsoft.DotNet.XHarness.TestRunners.Common;
using Xamarin.DeviceTestUtils;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	[Register(nameof(TestApplicationDelegate))]
	public class TestApplicationDelegate : BaseTestApplicationDelegate
	{
		protected override IEnumerable<TestAssemblyInfo> GetTestAssemblies()
		{
			yield return new TestAssemblyInfo(Assembly.GetExecutingAssembly(), Assembly.GetExecutingAssembly().Location);
			yield return new TestAssemblyInfo(typeof(SliderHandlerTests).Assembly, typeof(SliderHandlerTests).Assembly.Location);
		}
	}
}
