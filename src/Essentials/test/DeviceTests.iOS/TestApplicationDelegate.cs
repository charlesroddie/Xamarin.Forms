using System.Collections.Generic;
using System.Reflection;
using Foundation;
using Microsoft.DotNet.XHarness.TestRunners.Common;
using Xamarin.DeviceTestUtils;

namespace Xamarin.Essentials.DeviceTests.iOS
{
	[Register(nameof(TestApplicationDelegate))]
	public class TestApplicationDelegate : BaseTestApplicationDelegate
	{

		protected override IEnumerable<TestAssemblyInfo> GetTestAssemblies()
		{
			yield return new TestAssemblyInfo(Assembly.GetExecutingAssembly(), Assembly.GetExecutingAssembly().Location);
			yield return new TestAssemblyInfo(typeof(Battery_Tests).Assembly, typeof(Battery_Tests).Assembly.Location);
		}

		protected override TestRunner GetTestRunner(TestRunner testRunner, LogWriter logWriter)
		{
			testRunner.SkipCategories(Traits.GetSkipTraits());
			return testRunner;
		}
	}
}