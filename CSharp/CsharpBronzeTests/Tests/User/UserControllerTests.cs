using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CsharpBronze.Controllers.User;
using CsharpBronze.Models.User;
using System.Collections.Generic;

namespace CsharpBronze.Tests.User
{
    [TestClass]
    public class UserControllerTests
    {
        private MemoryCache? _memoryCache;
        private UserController? _controller;

        [TestInitialize]
        public void Initialize()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _controller = new UserController(_memoryCache);
        }

        [TestMethod]
        public void CreateUser_WithValidPassword()
        {
            // Arrange
            var userModel = new UserModel
            {
                Name = "Test",
                Email = "TestEmail",
                Username = "testuser",
                Password = "password",
                PasswordConfirmed = "password"
            };

            // Act
            var result = _controller?.CreateUser(userModel) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual("User created successfully", result.Value.GetType().GetProperty("message")?.GetValue(result.Value));
        }

        [TestMethod]
        public void CreateUser_WithInvalidPasswordTest()
        {
            // Arrange
            var userModel = new UserModel
            {
                Name = "Test2",
                Email = "TestEmail",
                Username = "testuser",
                Password = "password",
                PasswordConfirmed = "differentpassword"
            };

            // Act
            var result = _controller?.CreateUser(userModel) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Passwords do not match. Please re-enter passwords.", result.Value);
        }

        [TestMethod]
        public void GetUser_WithValidUserId()
        {
            // Arrange
            var userData = new List<UserModel>
            {
                new UserModel { UserId = 1, Name = "Ansel Test" },
                new UserModel { UserId = 2, Name = "Ansel Test 2" }
            };

            _controller = new UserController(_memoryCache!, userData);

            // Act
            var result = _controller?.GetUser(1) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsNotNull(result.Value);
            var message = result.Value.GetType().GetProperty("user")?.GetValue(result.Value) as UserModel;
            Assert.AreEqual("Ansel Test", message?.Name);
        }

        [TestMethod]
        public void DeleteUser_WithValidUserId()
        {
            // Arrange
            var userData = new List<UserModel>
            {
                new UserModel { UserId = 1, Name = "Ansel Test" },
                new UserModel { UserId = 2, Name = "Ansel Test 2" }
            };

            _controller = new UserController(_memoryCache!, userData);

            // Act
            var result = _controller.DeleteUser(1) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.IsNotNull(result.Value);
            var message = result.Value.GetType().GetProperty("message")?.GetValue(result.Value) as string;
            Assert.AreEqual("Successfully deleted user with ID 1", message);
        }
    }
}
