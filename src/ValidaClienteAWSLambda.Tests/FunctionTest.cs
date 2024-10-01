using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Moq;
using Xunit;

namespace ValidaClienteAWSLambda.Tests
{
    public class FunctionTests
    {
        private readonly Function _function;
        private readonly Mock<ILambdaContext> _contextMock;

        public FunctionTests()
        {
            _function = new Function();
            _contextMock = new Mock<ILambdaContext>();
        }

        [Fact]
        public void IsCpfValid_ReturnsFalse_WhenCpfIsInvalid()
        {
            // Arrange
            var invalidCpf = "12345678900";

            // Act
            var result = Function.IsCpfValid(invalidCpf);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsCpfValid_ReturnsTrue_WhenCpfIsValid()
        {
            // Arrange
            var validCpf = "12345678909"; // Use a valid CPF for testing

            // Act
            var result = Function.IsCpfValid(validCpf);

            // Assert
            Assert.True(result);
        }
    }
}