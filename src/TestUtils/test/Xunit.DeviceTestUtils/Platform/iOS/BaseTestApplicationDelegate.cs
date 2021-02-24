using System;
using System.Collections.Generic;
using System.Globalization;
using Foundation;
using Microsoft.DotNet.XHarness.TestRunners.Common;
using Microsoft.DotNet.XHarness.TestRunners.Xunit;
using UIKit;
using Xamarin.Essentials;

namespace Xamarin.DeviceTestUtils
{
	public abstract class BaseTestApplicationDelegate : UIApplicationDelegate
	{
		public static bool IsXHarnessRun(string[] args)
		{
			// usually means this is from xharness
			return args?.Length > 0 || Environment.GetEnvironmentVariable("NUNIT_AUTOEXIT")?.Length > 0;
		}

		public override UIWindow Window { get; set; }

		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
			Window = new UIWindow(UIScreen.MainScreen.Bounds)
			{
				RootViewController = new ViewController(this)
			};
			Window.MakeKeyAndVisible();

			return true;
		}

		protected abstract IEnumerable<TestAssemblyInfo> GetTestAssemblies();

		protected virtual void TerminateWithSuccess()
		{
			Console.WriteLine("Exiting test run with success...");

			var s = new ObjCRuntime.Selector("terminateWithSuccess");
			UIApplication.SharedApplication.PerformSelector(s, UIApplication.SharedApplication, 0);
		}

		protected virtual TestRunner GetTestRunner(TestRunner testRunner, LogWriter logWriter)
		{
			return testRunner;
		}

		class ViewController : UIViewController
		{
			readonly BaseTestApplicationDelegate _applicationDelegate;

			public ViewController(BaseTestApplicationDelegate applicationDelegate)
			{
				_applicationDelegate = applicationDelegate;
			}

			public override async void ViewDidLoad()
			{
				base.ViewDidLoad();

				var entryPoint = new TestsEntryPoint(_applicationDelegate);

				await entryPoint.RunAsync();
			}
		}

		class TestsEntryPoint : iOSApplicationEntryPoint
		{
			readonly BaseTestApplicationDelegate _applicationDelegate;

			public TestsEntryPoint(BaseTestApplicationDelegate applicationDelegate)
			{
				_applicationDelegate = applicationDelegate;
			}

			protected override bool LogExcludedTests => true;

			protected override int? MaxParallelThreads => Environment.ProcessorCount;

			protected override IDevice Device { get; } = new TestDevice();

			protected override IEnumerable<TestAssemblyInfo> GetTestAssemblies()
				=> _applicationDelegate.GetTestAssemblies();

			protected override void TerminateWithSuccess()
				=> _applicationDelegate.TerminateWithSuccess();

			protected override TestRunner GetTestRunner(LogWriter logWriter)
				=> _applicationDelegate.GetTestRunner(base.GetTestRunner(logWriter), logWriter);
		}

		class TestDevice : IDevice
		{
			public string BundleIdentifier => AppInfo.PackageName;

			public string UniqueIdentifier => Guid.NewGuid().ToString("N");

			public string Name => DeviceInfo.Name;

			public string Model => DeviceInfo.Model;

			public string SystemName => DeviceInfo.Platform.ToString();

			public string SystemVersion => DeviceInfo.VersionString;

			public string Locale => CultureInfo.CurrentCulture.Name;
		}
	}
}
