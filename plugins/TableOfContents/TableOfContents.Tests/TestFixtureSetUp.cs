using System;
using System.Collections.Generic;
using CommunityServer.Components;
using Ninject;
using NUnit.Framework;

namespace Telligent.Evolution.TableOfContents.Tests
{
	[SetUpFixture]
	public class SetUpFixture
	{
		[SetUp]
		public void SetUp()
		{
			// This is a hack because The CommunityServer.Components.Formatter class accesses the ISecurityService in the static constructor

			var kernel = new StandardKernel();
			kernel.Bind<ISecurityService>().To<DummySecurityService>().InSingletonScope();

			Telligent.Common.Services.Initialize(kernel);
		}

		[TearDown]
		public void TearDown()
		{
			Telligent.Common.Services.Shutdown();
		}
	}

}