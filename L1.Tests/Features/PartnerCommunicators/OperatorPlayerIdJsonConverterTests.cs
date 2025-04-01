using System;
using Newtonsoft.Json;
using Xunit;
using L1.Features.PartnerCommunicators;

namespace L1.Tests.Features.PartnerCommunicators
{
    public class OperatorPlayerIdJsonConverterTests
    {
        private readonly JsonSerializerSettings _settings;

        public OperatorPlayerIdJsonConverterTests()
        {
            _settings = new JsonSerializerSettings();
        }

        public class TestClass
        {
            [JsonProperty("playerId")]
            [JsonConverter(typeof(OperatorPlayerIdJsonConverter))]
            public object PlayerId { get; set; }

            // Property without converter for comparison
            public object OtherId { get; set; }
        }

        [Fact]
        public void Deserialize_IntValue_ShouldNotBeNull()
        {
            // Arrange
            const string json = "{\"playerId\": 123}";

            // Act
            var result = JsonConvert.DeserializeObject<TestClass>(json, _settings);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Deserialize_IntValue_ShouldBeIntType()
        {
            // Arrange
            const string json = "{\"playerId\": 123}";

            // Act
            var result = JsonConvert.DeserializeObject<TestClass>(json, _settings);

            // Assert
            Assert.IsType<int>(result.PlayerId);
        }

        [Fact]
        public void Deserialize_IntValue_ShouldHaveCorrectValue()
        {
            // Arrange
            const string json = "{\"playerId\": 123}";

            // Act
            var result = JsonConvert.DeserializeObject<TestClass>(json, _settings);

            // Assert
            Assert.Equal(123, result.PlayerId);
        }

        [Fact]
        public void Deserialize_NegativeIntValue_ShouldBeIntType()
        {
            // Arrange
            const string json = "{\"playerId\": -123}";

            // Act
            var result = JsonConvert.DeserializeObject<TestClass>(json, _settings);

            // Assert
            Assert.IsType<int>(result.PlayerId);
        }

        [Fact]
        public void Deserialize_NegativeIntValue_ShouldHaveCorrectValue()
        {
            // Arrange
            const string json = "{\"playerId\": -123}";

            // Act
            var result = JsonConvert.DeserializeObject<TestClass>(json, _settings);

            // Assert
            Assert.Equal(-123, result.PlayerId);
        }

        [Fact]
        public void Deserialize_StringValue_ShouldBeStringType()
        {
            // Arrange
            const string json = "{\"playerId\": \"abc123\"}";

            // Act
            var result = JsonConvert.DeserializeObject<TestClass>(json, _settings);

            // Assert
            Assert.IsType<string>(result.PlayerId);
        }

        [Fact]
        public void Deserialize_StringValue_ShouldHaveCorrectValue()
        {
            // Arrange
            const string json = "{\"playerId\": \"abc123\"}";

            // Act
            var result = JsonConvert.DeserializeObject<TestClass>(json, _settings);

            // Assert
            Assert.Equal("abc123", result.PlayerId);
        }

        [Fact]
        public void Deserialize_NullValue_ShouldBeNull()
        {
            // Arrange
            const string json = "{\"playerId\": null}";

            // Act
            var result = JsonConvert.DeserializeObject<TestClass>(json, _settings);

            // Assert
            Assert.Null(result.PlayerId);
        }

        [Fact]
        public void Deserialize_NumericString_ShouldBeStringType()
        {
            // Arrange
            const string json = "{\"playerId\": \"123\"}";

            // Act
            var result = JsonConvert.DeserializeObject<TestClass>(json, _settings);

            // Assert
            Assert.IsType<string>(result.PlayerId);
        }

        [Fact]
        public void Deserialize_NumericString_ShouldHaveCorrectValue()
        {
            // Arrange
            const string json = "{\"playerId\": \"123\"}";

            // Act
            var result = JsonConvert.DeserializeObject<TestClass>(json, _settings);

            // Assert
            Assert.Equal("123", result.PlayerId);
        }

        [Fact]
        public void Deserialize_LargeIntValue_ShouldBeStringType()
        {
            // Arrange
            const string json = "{\"playerId\": 2147483648}";  // Int32.MaxValue + 1

            // Act
            var result = JsonConvert.DeserializeObject<TestClass>(json, _settings);

            // Assert
            Assert.IsType<string>(result.PlayerId);
        }

        [Fact]
        public void Deserialize_LargeIntValue_ShouldHaveCorrectValue()
        {
            // Arrange
            const string json = "{\"playerId\": 2147483648}";  // Int32.MaxValue + 1

            // Act
            var result = JsonConvert.DeserializeObject<TestClass>(json, _settings);

            // Assert
            Assert.Equal("2147483648", result.PlayerId);
        }

        [Fact]
        public void Serialize_IntValue_ShouldHaveCorrectJson()
        {
            // Arrange
            var testObject = new TestClass { PlayerId = 123 };

            // Act
            var result = JsonConvert.SerializeObject(testObject, _settings);

            // Assert
            Assert.Equal("{\"playerId\":123,\"OtherId\":null}", result);
        }

        [Fact]
        public void Serialize_NegativeIntValue_ShouldHaveCorrectJson()
        {
            // Arrange
            var testObject = new TestClass { PlayerId = -123 };

            // Act
            var result = JsonConvert.SerializeObject(testObject, _settings);

            // Assert
            Assert.Equal("{\"playerId\":-123,\"OtherId\":null}", result);
        }

        [Fact]
        public void Serialize_StringValue_ShouldHaveCorrectJson()
        {
            // Arrange
            var testObject = new TestClass { PlayerId = "abc123" };

            // Act
            var result = JsonConvert.SerializeObject(testObject, _settings);

            // Assert
            Assert.Equal("{\"playerId\":\"abc123\",\"OtherId\":null}", result);
        }

        [Fact]
        public void Serialize_NullValue_ShouldHaveCorrectJson()
        {
            // Arrange
            var testObject = new TestClass { PlayerId = null };

            // Act
            var result = JsonConvert.SerializeObject(testObject, _settings);

            // Assert
            Assert.Equal("{\"playerId\":null,\"OtherId\":null}", result);
        }
    }
} 