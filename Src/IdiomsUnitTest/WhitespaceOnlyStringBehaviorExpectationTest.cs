using System;
using AutoFixture.Idioms;
using Xunit;

namespace AutoFixture.IdiomsUnitTest
{
    public class WhitespaceOnlyStringBehaviorExpectationTest
    {
        [Fact]
        public void SutIsBehaviorExpectation()
        {
            // Arrange
            // Act
            var sut = new WhitespaceOnlyStringBehaviorExpectation();

            // Assert
            Assert.IsAssignableFrom<IBehaviorExpectation>(sut);
        }

        [Fact]
        public void VerifyNullCommandThrows()
        {
            // Arrange
            var sut = new WhitespaceOnlyStringBehaviorExpectation();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                sut.Verify(null));
        }

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(Guid))]
        [InlineData(typeof(Version))]
        [InlineData(typeof(int))]
        public void VerifyDoesNothingWhenRequestedTypeIsNotString(Type type)
        {
            // Arrange
            var executeInvoked = false;
            var mockCommand = new DelegatingGuardClauseCommand { OnExecute = v => executeInvoked = true };
            mockCommand.RequestedType = type;

            var sut = new WhitespaceOnlyStringBehaviorExpectation();

            // Act
            sut.Verify(mockCommand);

            // Assert
            Assert.False(executeInvoked);
        }

        [Fact]
        public void VerifyCorrectlyInvokesExecuteWhenRequestedTypeIsStringWithWhitespace()
        {
            // Arrange
            var mockVerified = false;
            var mockCommand = new DelegatingGuardClauseCommand
            {
                OnExecute = v => mockVerified = " ".Equals(v),
                OnCreateException = v => new InvalidOperationException(),
                RequestedType = typeof(string)
            };

            var sut = new WhitespaceOnlyStringBehaviorExpectation();

            // Act
            try
            {
                sut.Verify(mockCommand);
            }
            catch (InvalidOperationException) { }

            // Assert
            Assert.True(mockVerified);
        }

        [Fact]
        public void VerifySuccedsWhenCommandThrowsCorrectException()
        {
            // Arrange
            var cmd = new DelegatingGuardClauseCommand
            {
                OnExecute = v => { throw new ArgumentException(); },
                RequestedType = typeof(string)
            };
            var sut = new WhitespaceOnlyStringBehaviorExpectation();

            // Act & Assert
            Assert.Null(Record.Exception(() =>
                sut.Verify(cmd)));
        }

        [Fact]
        public void VerifyThrowsWhenCommandThrowsUnexpectedException()
        {
            // Arrange
            var expectedInner = new Exception();
            var expected = new Exception();
            var cmd = new DelegatingGuardClauseCommand
            {
                OnExecute = v => { throw expectedInner; },
                OnCreateExceptionWithInner = (v, e) => v == "\" \"" && expectedInner.Equals(e) ? expected : new Exception(),
                RequestedType = typeof(string)
            };
            var sut = new WhitespaceOnlyStringBehaviorExpectation();

            // Act & Assert
            var result = Assert.Throws<Exception>(() =>
                sut.Verify(cmd));
            Assert.Equal(expected, result);
        }

        [Fact]
        public void VerifyThrowsWhenCommandDoesNotThrow()
        {
            // Arrange
            var expected = new Exception();
            var cmd = new DelegatingGuardClauseCommand
            {
                OnCreateException = v => v == "\" \"" ? expected : new Exception(),
                RequestedType = typeof(string)
            };
            var sut = new WhitespaceOnlyStringBehaviorExpectation();

            // Act & Assert
            var result = Assert.Throws<Exception>(() =>
                sut.Verify(cmd));
            Assert.Equal(expected, result);
        }
    }
}