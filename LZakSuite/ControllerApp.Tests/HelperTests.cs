using ControllerApp.ControllerCore;
using ControllerApp.Helpers;
using Xunit;

namespace ControllerApp.Tests
{
    public class HelperTests
    {
        [Fact]
        public void TryGetCoordinates_X_Valid()
        {
            // ok coordinates
            string[] parts = new string[] { "G1", "X22.3", "Y65.3" };

            float result = 0F;

            Assert.True(MathHelper.TryGetCoordinates(parts, Axis.X, out result));
            Assert.Equal(22.3F, result);

            Assert.True(MathHelper.TryGetCoordinates(parts, Axis.Y, out result));
            Assert.Equal(65.3F, result);
        }

        [Fact]
        public void TryGetCoordinates_Invalid_X_Valid_Y()
        {
            // X missing from parameter
            string[] parts = new string[] { "G1", "22.3", "Y65.3" };

            float result = 0F;

            Assert.False(MathHelper.TryGetCoordinates(parts, Axis.X, out result));
            Assert.Equal(0F, result);

            Assert.True(MathHelper.TryGetCoordinates(parts, Axis.Y, out result));
            Assert.Equal(65.3F, result);
        }

        [Fact]
        public void TryGetCoordinates_Valid_X_Invalid_Y()
        {
            // Y missing from parameter
            string[] parts = new string[] { "G1", "X22.3", "batata" };

            float result = 0F;

            Assert.True(MathHelper.TryGetCoordinates(parts, Axis.X, out result));
            Assert.Equal(22.3F, result);

            Assert.False(MathHelper.TryGetCoordinates(parts, Axis.Y, out result));
            Assert.Equal(0F, result);
        }

        [Fact]
        public void TryGetCoordinates_Null_Parts()
        {
            // Y missing from parameter
            string[] parts = new string[] { null, null, null };

            float result = 0F;

            Assert.False(MathHelper.TryGetCoordinates(parts, Axis.X, out result));
            Assert.Equal(0F, result);

            Assert.False(MathHelper.TryGetCoordinates(parts, Axis.Y, out result));
            Assert.Equal(0F, result);
        }

        [Fact]
        public void TryGetCoordinates_Empty_Parts()
        {
            // Y missing from parameter
            string[] parts = new string[] { string.Empty, string.Empty, string.Empty };

            float result = 0F;

            Assert.False(MathHelper.TryGetCoordinates(parts, Axis.X, out result));
            Assert.Equal(0F, result);

            Assert.False(MathHelper.TryGetCoordinates(parts, Axis.Y, out result));
            Assert.Equal(0F, result);
        }

        [Fact]
        public void TryGetCoordinates_Null_Array_Of_Parts()
        {
            // Y missing from parameter
            string[] parts = null;

            float result = 0F;

            Assert.False(MathHelper.TryGetCoordinates(parts, Axis.X, out result));
            Assert.Equal(0F, result);

            Assert.False(MathHelper.TryGetCoordinates(parts, Axis.Y, out result));
            Assert.Equal(0F, result);
        }
    }
}
