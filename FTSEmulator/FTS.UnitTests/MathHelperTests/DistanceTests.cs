using FTS.Core;
using Xunit;

namespace FTS.UnitTests.MathHelperTests
{
    public class DistanceTests
    {

        [Fact]
        public void Distance_ZeroToZero()
        {
            PointF p1 = new PointF();
            PointF p2 = new PointF();

            Assert.Equal(0, MathHelper.Distance(p1, p2));
        }

        [Fact]
        public void Distance_ZeroOne()
        {
            PointF p1 = new PointF();
            PointF p2 = new PointF(0, 1);

            Assert.Equal(1, MathHelper.Distance(p1, p2));
        }

        [Fact]
        public void Distance_Sqrt2()
        {
            PointF p1 = new PointF();
            PointF p2 = new PointF(0, 1);

            Assert.Equal(1.4, MathHelper.Distance(p1, p2), 1);
        }


        [Fact]
        public void Distance_ZeroTen()
        {
            PointF p1 = new PointF(10, 0);
            PointF p2 = new PointF(0, 0);

            Assert.Equal(10, MathHelper.Distance(p1, p2), 1);
        }
        [Fact]
        public void Distance_TenZero()
        {
            PointF p1 = new PointF(0, 0);
            PointF p2 = new PointF(0, 10);

            Assert.Equal(10, MathHelper.Distance(p1, p2), 1);
        }

        [Fact]
        public void Distance_345()
        {
            PointF p1 = new PointF(0, 3);
            PointF p2 = new PointF(4, 0);

            Assert.Equal(5, MathHelper.Distance(p1, p2), 1);
        }
    }
}
