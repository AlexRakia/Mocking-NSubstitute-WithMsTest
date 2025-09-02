using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using UserManagement.Controllers;
using UserManagement.Models;
using UserManagement.Services;

namespace UserManagement.Tests
{
    [TestClass]
    public class UserControllerTests
    {
        private IUserService _mockUserService = null!;
        private UserController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            // Create a substitute (mock) for IUserService
            _mockUserService = Substitute.For<IUserService>();
            _controller = new UserController(_mockUserService);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clear any received calls between tests
            _mockUserService.ClearReceivedCalls();
        }

        [TestMethod]
        public void GetUserDisplayName_WithValidUser_ReturnsUserName()
        {
            // Arrange
            var user = new User { Id = 1, Name = "John Doe" };
            _mockUserService.GetUser(1).Returns(user);

            // Act
            var result = _controller.GetUserDisplayName(1);

            // Assert
            Assert.AreEqual("John Doe", result);
        }

        [TestMethod]
        public void GetUserDisplayName_WithNullUser_ReturnsUnknownUser()
        {
            // Arrange
            _mockUserService.GetUser(1).Returns((User?)null);

            // Act
            var result = _controller.GetUserDisplayName(1);

            // Assert
            Assert.AreEqual("Unknown User", result);
        }

        [TestMethod]
        public void CreateUser_WithValidData_CallsSaveUserAndReturnsTrue()
        {
            // Arrange
            _mockUserService.ValidateUser(Arg.Any<User>()).Returns(true);
            _mockUserService.SaveUser(Arg.Any<User>()).Returns(true);

            // Act
            var result = _controller.CreateUser("Jane Doe", "jane@example.com");

            // Assert
            Assert.IsTrue(result);
            _mockUserService.Received(1).ValidateUser(Arg.Is<User>(u =>
                u.Name == "Jane Doe" && u.Email == "jane@example.com"));
            _mockUserService.Received(1).SaveUser(Arg.Any<User>());
        }

        [TestMethod]
        public void CreateUser_WithEmptyName_ReturnsFalseWithoutSaving()
        {
            // Act
            var result = _controller.CreateUser("", "jane@example.com");

            // Assert
            Assert.IsFalse(result);
            _mockUserService.DidNotReceive().SaveUser(Arg.Any<User>());
            _mockUserService.DidNotReceive().ValidateUser(Arg.Any<User>());
        }

        [TestMethod]
        public void CreateUser_WithEmptyEmail_ReturnsFalseWithoutSaving()
        {
            // Act
            var result = _controller.CreateUser("Jane Doe", "");

            // Assert
            Assert.IsFalse(result);
            _mockUserService.DidNotReceive().SaveUser(Arg.Any<User>());
            _mockUserService.DidNotReceive().ValidateUser(Arg.Any<User>());
        }

        [TestMethod]
        public void CreateUser_ValidationFails_ReturnsFalse()
        {
            // Arrange
            _mockUserService.ValidateUser(Arg.Any<User>()).Returns(false);

            // Act
            var result = _controller.CreateUser("Jane Doe", "jane@example.com");

            // Assert
            Assert.IsFalse(result);
            _mockUserService.Received(1).ValidateUser(Arg.Any<User>());
            _mockUserService.DidNotReceive().SaveUser(Arg.Any<User>());
        }

        [TestMethod]
        public void UpdateUser_WithValidUser_CallsSaveUser()
        {
            // Arrange
            var user = new User { Id = 1, Name = "Jane Doe", Email = "jane@example.com" };
            _mockUserService.ValidateUser(user).Returns(true);
            _mockUserService.SaveUser(user).Returns(true);

            // Act
            var result = _controller.UpdateUser(user);

            // Assert
            Assert.IsTrue(result);
            _mockUserService.Received(1).ValidateUser(user);
            _mockUserService.Received(1).SaveUser(user);
        }

        [TestMethod]
        public void UpdateUser_WithNullUser_ReturnsFalse()
        {
            // Act
            var result = _controller.UpdateUser(null!);

            // Assert
            Assert.IsFalse(result);
            _mockUserService.DidNotReceive().ValidateUser(Arg.Any<User>());
            _mockUserService.DidNotReceive().SaveUser(Arg.Any<User>());
        }

        [TestMethod]
        public void DeactivateUser_WithExistingUser_DeactivatesAndSaves()
        {
            // Arrange
            var user = new User { Id = 1, Name = "John Doe", IsActive = true };
            _mockUserService.GetUser(1).Returns(user);
            _mockUserService.SaveUser(user).Returns(true);

            // Act
            var result = _controller.DeactivateUser(1);

            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(user.IsActive);
            _mockUserService.Received(1).GetUser(1);
            _mockUserService.Received(1).SaveUser(user);
        }

        [TestMethod]
        public void DeactivateUser_WithNonExistentUser_ReturnsFalse()
        {
            // Arrange
            _mockUserService.GetUser(999).Returns((User?)null);

            // Act
            var result = _controller.DeactivateUser(999);

            // Assert
            Assert.IsFalse(result);
            _mockUserService.Received(1).GetUser(999);
            _mockUserService.DidNotReceive().SaveUser(Arg.Any<User>());
        }

        [TestMethod]
        public void GetActiveUserNames_WithMultipleUsers_ReturnsUserNames()
        {
            // Arrange
            var users = new[]
            {
                new User { Id = 1, Name = "John Doe", IsActive = true },
                new User { Id = 2, Name = "Jane Smith", IsActive = true },
                new User { Id = 3, Name = "Bob Johnson", IsActive = true }
            };
            _mockUserService.GetActiveUsers().Returns(users);

            // Act
            var result = _controller.GetActiveUserNames().ToList();

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.Contains("John Doe"));
            Assert.IsTrue(result.Contains("Jane Smith"));
            Assert.IsTrue(result.Contains("Bob Johnson"));
        }

        [TestMethod]
        public void FindUserByEmail_WithValidEmail_ReturnsUser()
        {
            // Arrange
            var user = new User { Id = 1, Name = "John Doe", Email = "john@example.com" };
            _mockUserService.GetUserByEmail("john@example.com").Returns(user);

            // Act
            var result = _controller.FindUserByEmail("john@example.com");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("John Doe", result.Name);
            Assert.AreEqual("john@example.com", result.Email);
        }

        [TestMethod]
        public void FindUserByEmail_WithEmptyEmail_ReturnsNull()
        {
            // Act
            var result = _controller.FindUserByEmail("");

            // Assert
            Assert.IsNull(result);
            _mockUserService.DidNotReceive().GetUserByEmail(Arg.Any<string>());
        }

        [TestMethod]
        public void FindUserByEmail_WithNullEmail_ReturnsNull()
        {
            // Act
            var result = _controller.FindUserByEmail(null!);

            // Assert
            Assert.IsNull(result);
            _mockUserService.DidNotReceive().GetUserByEmail(Arg.Any<string>());
        }

        [TestMethod]
        public void Constructor_WithNullUserService_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => new UserController(null!));
        }
    }

    [TestClass]
    public class NSubstituteAdvancedExamples
    {
        private IUserService _mockUserService = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockUserService = Substitute.For<IUserService>();
        }

        [TestMethod]
        public void AdvancedExample_ArgumentMatching()
        {
            // Arrange - Setup different return values based on arguments
            _mockUserService.GetUser(Arg.Is<int>(id => id > 0)).Returns(new User { Name = "Valid User" });
            _mockUserService.GetUser(Arg.Is<int>(id => id <= 0)).Returns((User?)null);

            // Act & Assert
            var validResult = _mockUserService.GetUser(5);
            var invalidResult = _mockUserService.GetUser(-1);

            Assert.IsNotNull(validResult);
            Assert.AreEqual("Valid User", validResult.Name);
            Assert.IsNull(invalidResult);
        }

        [TestMethod]
        public void AdvancedExample_CallbacksAndActions()
        {
            // Arrange - Use callbacks to modify behavior
            var savedUsers = new List<User>();
            _mockUserService.SaveUser(Arg.Any<User>())
                .Returns(true)
                .AndDoes(call => savedUsers.Add(call.Arg<User>()));

            // Act
            var user1 = new User { Name = "User 1", Email = "user1@test.com" };
            var user2 = new User { Name = "User 2", Email = "user2@test.com" };

            _mockUserService.SaveUser(user1);
            _mockUserService.SaveUser(user2);

            // Assert
            Assert.AreEqual(2, savedUsers.Count);
            Assert.AreEqual("User 1", savedUsers[0].Name);
            Assert.AreEqual("User 2", savedUsers[1].Name);
        }

        [TestMethod]
        public void AdvancedExample_ExceptionThrowing()
        {
            // Arrange - Configure mock to throw exception
            _mockUserService.GetUser(Arg.Any<int>())
                .Returns(x => throw new InvalidOperationException("Database connection failed"));

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => _mockUserService.GetUser(1));
        }

        [TestMethod]
        public void AdvancedExample_VerifyingCallOrder()
        {
            // Arrange
            var user = new User { Id = 1, Name = "Test User" };

            // Act
            _mockUserService.ValidateUser(user);
            _mockUserService.SaveUser(user);

            // Assert - Verify calls were made in order
            Received.InOrder(() =>
            {
                _mockUserService.ValidateUser(user);
                _mockUserService.SaveUser(user);
            });
        }

        [TestMethod]
        public void AdvancedExample_VerifyingCallCount()
        {
            // Arrange
            var user = new User { Name = "Test User" };

            _mockUserService.Received(0).SaveUser(user); // Never called (same as DidNotReceive)

            // Act
            _mockUserService.SaveUser(user);
            _mockUserService.Received(1).SaveUser(user); // Exactly 1 time
            _mockUserService.SaveUser(user);
            _mockUserService.SaveUser(user);

            // Assert - Verify exact number of calls
            _mockUserService.Received(3).SaveUser(user);
            _mockUserService.Received(Quantity.Exactly(3)).SaveUser(user);
            _mockUserService.Received(3).SaveUser(user); // Exactly 3 times
        }
    }
}