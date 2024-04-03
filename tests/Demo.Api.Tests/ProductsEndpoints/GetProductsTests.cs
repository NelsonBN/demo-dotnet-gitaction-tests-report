using Demo.Api.Tests.Config;
using Demo.Application.DTOs;
using FluentAssertions;

namespace Demo.Api.Tests.ProductsEndpoints;

[Collection(nameof(CollectionIntegrationTests))]
public sealed class GetProductsTests
{
    private readonly IntegrationTestsFactory _factory;

    public GetProductsTests(IntegrationTestsFactory factory)
        => _factory = factory;


    [Fact]
    public async Task Should_Return_Products_and_200_StatusCode()
    {
        // Arrange && Act
        var act = await _factory.CreateClient()
            .GetAsync("/products");


        // Assert
        act.Should()
           .Be200Ok()
           .And.Satisfy<IEnumerable<ProductResponse>>(model =>
                model.Should().HaveCountGreaterThanOrEqualTo(3));
    }
}
