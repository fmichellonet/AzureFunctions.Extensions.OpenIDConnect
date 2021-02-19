using System;
using AzureFunctions.Extensions.OpenIDConnect.Configuration;
using FluentAssertions;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace AzureFunctions.Extensions.OpenIDConnect.Tests
{
    [TestFixture]
    public class DependencyInjectionShould
    {
        [Test]
        public void Be_Resolvable()
        {
            var collection = new ServiceCollection();
            collection.AddOpenIDConnect(builder =>
            {
                builder.SetIssuerBaseUrlConfiguration("https://issuer.com/");
                builder.SetTokenValidation("issuer", "audience");
                builder.SetTypeCrawler(() => new Type[] { });
            });

            var provider = collection.BuildServiceProvider();

            provider.GetService<IAuthenticationService>().Should().NotBe(null);
            provider.GetService<IRouteGuardian>().Should().NotBe(null);
            provider.GetService<IFunctionFilter>().Should().NotBe(null);
        }
    }
}