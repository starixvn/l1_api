using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using L1.Features.Partners;
using MongoDB.Driver;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace L1.Tests.Features.Partners
{
    public class PromotionDataAccessTests
    {
        private readonly IMongoCollection<Promotion> promotionCollection;
        private readonly PromotionDataAccess dataAccess;
        private readonly Fixture fixture = new Fixture();

        public PromotionDataAccessTests()
        {
            promotionCollection = Substitute.For<IMongoCollection<Promotion>>();

            var mongoClient = Substitute.For<IMongoClient>();
            var database = Substitute.For<IMongoDatabase>();
            mongoClient.GetDatabase(Arg.Any<string>()).Returns(database);
            database.GetCollection<Promotion>("game_promotions").Returns(promotionCollection);

            dataAccess = new PromotionDataAccess(mongoClient);
        }

        [Theory, AutoData]
        public async Task GetPromotion_WithValidData_ShouldReturnPromotion(string partnerId, string promotionCode)
        {
            // Arrange
            var expectedPromotion = new Promotion
            {
                PartnerId = partnerId,
                PromotionCode = promotionCode,
                PromotionName = "Test Promotion",
                StartTime = DateTime.UtcNow.AddDays(-5),
                EndTime = DateTime.UtcNow.AddDays(5)
            };

            var asyncCursor = Substitute.For<IAsyncCursor<Promotion>>();
            asyncCursor.Current.Returns(new List<Promotion> { expectedPromotion });
            asyncCursor.MoveNext(Arg.Any<CancellationToken>()).Returns(true, false);
            asyncCursor.MoveNextAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(true), Task.FromResult(false));

            promotionCollection
                .FindAsync(Arg.Any<FilterDefinition<Promotion>>(),
                          Arg.Any<FindOptions<Promotion, Promotion>>(),
                          Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(asyncCursor));

            // Act
            var result = await dataAccess.GetPromotion(partnerId, promotionCode);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedPromotion, result);
        }

        [Theory, AutoData]
        public async Task GetPromotion_WithNonexistentPromotion_ShouldReturnNull(string partnerId, string promotionCode)
        {
            // Arrange
            var asyncCursor = Substitute.For<IAsyncCursor<Promotion>>();
            asyncCursor.Current.Returns(new List<Promotion>());
            asyncCursor.MoveNext(Arg.Any<CancellationToken>()).Returns(false);
            asyncCursor.MoveNextAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(false));

            promotionCollection
                .FindAsync(Arg.Any<FilterDefinition<Promotion>>(),
                          Arg.Any<FindOptions<Promotion, Promotion>>(),
                          Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(asyncCursor));

            // Act
            var result = await dataAccess.GetPromotion(partnerId, promotionCode);

            // Assert
            Assert.Null(result);
        }
    }
} 