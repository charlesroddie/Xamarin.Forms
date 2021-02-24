using System;
using System.Collections.Generic;
using System.Reflection;
using Android.App;
using Android.Runtime;
using Microsoft.DotNet.XHarness.TestRunners.Common;
using Xamarin.DeviceTestUtils;

namespace Xamarin.Platform.Handlers.DeviceTests
{
	[Instrumentation(Name = "com.xamarin.handlers.devicetests.TestInstrumentation")]
	public class TestInstrumentation : BaseTestInstrumentation
	{
		protected TestInstrumentation(IntPtr handle, JniHandleOwnership transfer)
			: base(handle, transfer)
		{
		}

		protected override IEnumerable<TestAssemblyInfo> GetTestAssemblies()
		{
			yield return new TestAssemblyInfo(Assembly.GetExecutingAssembly(), Assembly.GetExecutingAssembly().Location);
			yield return new TestAssemblyInfo(typeof(SliderHandlerTests).Assembly, typeof(SliderHandlerTests).Assembly.Location);
		}
	}
}