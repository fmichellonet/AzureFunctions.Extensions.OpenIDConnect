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
    using System;
    using System.Collections.Generic;

    [TestFixture]
    public class RouteGuardianShould
    {
        [Test]
        public async Task Not_Authorize_NonHttpTrigger_Function()
        {
            // Arrange
            var guardian = new RouteGuardian(() => new List<Type>{ typeof(NonHttpTrigger_Function)});

            // Act
            var result = await guardian.ShouldAuthorize("NonHttpTrigger_Function");

            // Assert
            result.Should().Be(false);
        }

        [Test]
        public async Task Not_Authorize_NonAuthorize_Function()
        {
            // Arrange
            var guardian = new RouteGuardian(() => new List<Type> { typeof(NonAuthorize_Function) });

            // Act
            var result = await guardian.ShouldAuthorize("NonAuthorize_Function");

            // Assert
            result.Should().Be(false);
        }

        [Test]
        public async Task Authorize_Otherwise()
        {
            // Arrange
            var guardian = new RouteGuardian(() => new List<Type> { typeof(Authorizable_Function) });

            // Act
            var result = await guardian.ShouldAuthorize("Authorizable_Function");

            // Assert
            result.Should().Be(true);
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

        internal class NonAuthorize_Function
        {
            [FunctionName("NonAuthorize_Function")]
            public async Task<IActionResult> Run(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
            {
                var responseMessage = "Hello. This HTTP triggered function is protected.";

                return new OkObjectResult(responseMessage);
            }
        }

        internal class Authorizable_Function
        {
            [Authorize]
            [FunctionName("Authorizable_Function")]
            public async Task<IActionResult> Run(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
            {
                var responseMessage = "Hello. This HTTP triggered function is protected.";

                return new OkObjectResult(responseMessage);
            }
        }
    }
}