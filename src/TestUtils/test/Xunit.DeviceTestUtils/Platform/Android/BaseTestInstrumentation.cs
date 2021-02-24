using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Android.App;
using Android.OS;
using Android.Runtime;
using Microsoft.DotNet.XHarness.TestRunners.Common;
using Microsoft.DotNet.XHarness.TestRunners.Xunit;
using Xamarin.Essentials;

namespace Xamarin.DeviceTestUtils
{
	public abstract class BaseTestInstrumentation : Instrumentation
	{
		protected BaseTestInstrumentation()
			: base()
		{
		}

		protected BaseTestInstrumentation(IntPtr handle, JniHandleOwnership transfer)
			: base(handle, transfer)
		{
		}

		public override void OnCreate(Bundle arguments)
		{
			base.OnCreate(arguments);

			Start();
		}

		public override async void OnStart()
		{
			base.OnStart();

			var bundle = new Bundle();

			var entryPoint = new TestsEntryPoint(this, TestResultsFilename);
			entryPoint.TestsCompleted += (sender, results) =>
			{
				var message =
					$"Tests run: {results.ExecutedTests} " +
					$"Passed: {results.PassedTests} " +
					$"Inconclusive: {results.InconclusiveTests} " +
					$"Failed: {results.FailedTests} " +
					$"Ignored: {results.SkippedTests}";
				bundle.PutString("test-execution-summary", message);

				bundle.PutLong("return-code", results.FailedTests == 0 ? 0 : 1);
			};

			await entryPoint.RunAsync();

			if (File.Exists(entryPoint.TestsResultsFinalPath))
				bundle.PutString("test-results-path", entryPoint.TestsResultsFinalPath);

			if (bundle.GetLong("return-code", -1) == -1)
				bundle.PutLong("return-code", 1);

			Finish(Result.Ok, bundle);
		}

		protected virtual string TestResultsFilename => "TestResults.xml";

		protected virtual void TerminateWithSuccess()
		{
		}

		protected virtual TestRunner GetTestRunner(TestRunner testRunner, LogWriter logWriter)
		{
			return testRunner;
		}

		protected abstract IEnumerable<TestAssemblyInfo> GetTestAssemblies();

		class TestsEntryPoint : AndroidApplicationEntryPoint
		{
			readonly string _resultsPath;
			readonly BaseTestInstrumentation _instrumentation;

			public TestsEntryPoint(BaseTestInstrumentation instrumentation, string resultsFileName)
			{
#pragma warning disable CS0618 // Type or member is obsolete
				var root = ((int)Build.VERSION.SdkInt) >= 30
					? global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath
					: Application.Context.GetExternalFilesDir(null)?.AbsolutePath ?? FileSystem.AppDataDirectory;
#pragma warning restore CS0618 // Type or member is obsolete

				var docsDir = Path.Combine(root, "Documents");

				if (!Directory.Exists(docsDir))
					Directory.CreateDirectory(docsDir);

				_resultsPath = Path.Combine(docsDir, resultsFileName);
				_instrumentation = instrumentation;
			}

			protected override bool LogExcludedTests => true;

			public override TextWriter Logger => null;

			public override string TestsResultsFinalPath => _resultsPath;

			protected override int? MaxParallelThreads => System.Environment.ProcessorCount;

			protected override IDevice Device { get; } = new TestDevice();

			protected override IEnumerable<TestAssemblyInfo> GetTestAssemblies() =>
				_instrumentation.GetTestAssemblies();

			protected override void TerminateWithSuccess() =>
				_instrumentation.TerminateWithSuccess();

			protected override TestRunner GetTestRunner(LogWriter logWriter) =>
				_instrumentation.GetTestRunner(base.GetTestRunner(logWriter), logWriter);
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