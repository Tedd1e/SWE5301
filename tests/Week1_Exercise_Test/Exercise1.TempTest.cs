using System;
using System.Collections.Generic;
using System.Text;

namespace Exercise1_Test
{
    public class Exercise1_TemperatureTests
    {
        [Theory]
        [InlineData(-10, 14)]
        [InlineData(0, 32)]
        [InlineData(37, 98.6)]
        [InlineData(100, 212)]
        public void CelsiusToFahrenheit_ConvertsCorrectly(double celsius, double expectedFahrenheit)
        {
            Assert.Equal(expectedFahrenheit, Exercises.CelsiusToFahrenheit(celsius), 1);
        }

        [Theory]
        [InlineData(14, -10)]
        [InlineData(32, 0)]
        [InlineData(98.6, 37)]
        [InlineData(212, 100)]
        public void FahrenheitToCelsius_ConvertsCorrectly(double fahrenheit, double expectedCelsius)
        {
            Assert.Equal(expectedCelsius, Exercises.FahrenheitToCelsius(fahrenheit), 1);
        }

        [Fact]
        public void ConvertingThereAndBack_GivesTheOriginalValue()
        {
            Assert.Equal(21.5, Exercises.FahrenheitToCelsius(Exercises.CelsiusToFahrenheit(21.5)), 6);
        }
    }
}
