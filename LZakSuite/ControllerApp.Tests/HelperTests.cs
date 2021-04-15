using ControllerApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ControllerApp.Tests
{
    public class HelperTests
    {
        [Fact]
        public static void StringToFloat_Valid()
        {
            string value = "3.22";

            var converted = value.StringToFloat();

            Assert.Equal(3.22, converted);
        }
    }
}
