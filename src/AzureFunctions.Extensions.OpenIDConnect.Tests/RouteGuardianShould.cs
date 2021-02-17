using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace AzureFunctions.Extensions.OpenIDConnect.Tests
{
    [TestFixture]
    public class RouteGuardianShould
    {
        [Test]
        public async Task Not_Authorize_NonHttpTrigger_Function()
        {
            // Arrange
            var guardian = new RouteGuardian(RouteGuardian.AppDomainCrawler);

            // Act
            var result = await guardian.ShouldAuthorize("NonHttpTrigger_Function");

            // Assert
            result.Should().Be(false);
        }


        internal class NonHttpTrigger_Function
        {
            [Authorize]
            [FunctionName("NonHttpTrigger_Function")]
            public async Task<IActionResult> Run(HttpRequest req, ILogger log)
            {
                var responseMessage = "Hello. This HTTP triggered function is protected.";

                return new OkObjectResult(responseMessage);
            }
        }
    }
}