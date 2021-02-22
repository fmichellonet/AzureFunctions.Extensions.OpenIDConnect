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
        public void Not_Authorize_When_Not_HttpTrigger()
        {
            // Arrange
            var guardian = new RouteGuardian(() => new List<Type>{ typeof(Not_HttpTrigger) });

            // Act
            var result = guardian.IsProtectedRoute("Not_HttpTrigger");

            // Assert
            result.Should().Be(false);
        }

        [Test]
        public void Not_Authorize_When_No_Authorize_Attribute_On_Method_And_Type()
        {
            // Arrange
            var guardian = new RouteGuardian(() => new List<Type> { typeof(No_Authorize_Attribute_On_Method_And_Type) });

            // Act
            var result = guardian.IsProtectedRoute("No_Authorize_Attribute_On_Method_And_Type");

            // Assert
            result.Should().Be(false);
        }

        [Test]
        public void Authorize_When_Authorize_Attribute_Is_On_Method()
        {
            // Arrange
            var guardian = new RouteGuardian(() => new List<Type> { typeof(Authorize_Attribute_Is_On_Method) });

            // Act
            var result = guardian.IsProtectedRoute("Authorize_Attribute_Is_On_Method");

            // Assert
            result.Should().Be(true);
        }

        [Test]
        public void Authorize_When_Authorize_Attribute_Is_On_Class()
        {
            // Arrange
            var guardian = new RouteGuardian(() => new List<Type> { typeof(Authorize_Attribute_Is_On_Class) });

            // Act
            var result = guardian.IsProtectedRoute("Authorize_Attribute_Is_On_Class");

            // Assert
            result.Should().Be(true);
        }

        [Test]
        public void NotAuthorize_When_Authorize_Attribute_Is_On_Class_But_AllowAnonimous_On_Method()
        {
            // Arrange
            var guardian = new RouteGuardian(() => new List<Type> { typeof(Attribute_Is_On_Class_But_AllowAnonimous_On_Method) });

            // Act
            var result = guardian.IsProtectedRoute("Attribute_Is_On_Class_But_AllowAnonimous_On_Method");

            // Assert
            result.Should().Be(false);
        }



        internal class Not_HttpTrigger
        {
            [Authorize]
            [FunctionName("Not_HttpTrigger")]
            public IActionResult Run(HttpRequest req, ILogger log)
            {
                var responseMessage = "Hello. This HTTP triggered function is protected.";

                return new OkObjectResult(responseMessage);
            }
        }

        internal class No_Authorize_Attribute_On_Method_And_Type
        {
            [FunctionName("No_Authorize_Attribute_On_Method_And_Type")]
            public IActionResult Run(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
            {
                var responseMessage = "Hello. This HTTP triggered function is protected.";

                return new OkObjectResult(responseMessage);
            }
        }

        internal class Authorize_Attribute_Is_On_Method
        {
            [Authorize]
            [FunctionName("Authorize_Attribute_Is_On_Method")]
            public IActionResult Run(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
            {
                var responseMessage = "Hello. This HTTP triggered function is protected.";

                return new OkObjectResult(responseMessage);
            }
        }

        [Authorize]
        internal class Authorize_Attribute_Is_On_Class
        {
            [FunctionName("Authorize_Attribute_Is_On_Class")]
            public IActionResult Run(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
            {
                var responseMessage = "Hello. This HTTP triggered function is protected.";

                return new OkObjectResult(responseMessage);
            }
        }

        [Authorize]
        internal class Attribute_Is_On_Class_But_AllowAnonimous_On_Method
        {
            [AllowAnonymous]
            [FunctionName("Attribute_Is_On_Class_But_AllowAnonimous_On_Method")]
            public IActionResult Run(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
            {
                var responseMessage = "Hello. This HTTP triggered function is protected.";

                return new OkObjectResult(responseMessage);
            }
        }
    }
}