using Demo.Application.DTOs;
using Demo.Application.Services;
using Demo.Application.UseCases;
using Demo.Domain;
using NSubstitute;

namespace Demo.Application.Tests.UseCases;

public sealed class UpdateProductUseCaseTests
{
    private readonly IProductsRepository _repository;
    private readonly UpdateProductUseCase _useCase;

    public UpdateProductUseCaseTests()
    {
        _repository = Substitute.For<IProductsRepository>();
        _useCase = new(_repository);
    }

    [Fact]
    public async Task When_Product_Is_Updated_Then_It_Should_Return_Updated_Product()
    {
        // Arrange
        var id = Guid.NewGuid();

        var request = new ProductRequest { Name = "Product Name 1", Quantity = 10u };

        var product = Product.Create(
            "Product Name 132",
            1120u);

        _repository.GetAsync(Arg.Any<Guid>())
                   .Returns(product);

        _repository.ExistsAsync(Arg.Any<Guid>(), Arg.Any<string>())
                   .Returns(false);


        // Act
        await _useCase.ExecuteAsync(id, request);


        // Assert
        await _repository
            .Received(1)
            .UpdateAsync(Arg.Any<Product>());
    }

    [Fact]
    public async Task When_Product_Is_Updated_With_Existent_Name_Then_It_Should_Throw_DuplicateException()
    {
        // Arrange
        var id = Guid.NewGuid();

        var request = new ProductRequest { Name = "Product Name 2", Quantity = 10u };

        var product = Product.Create(
            "Product Name 2",
            1120u);

        _repository.GetAsync(Arg.Any<Guid>())
                   .Returns(product);

        _repository.ExistsAsync(Arg.Any<Guid>(), Arg.Any<string>())
                   .Returns(true);


        // Act
        var act = () => _useCase.ExecuteAsync(id, request);


        // Assert
        await act.Should().ThrowAsync<DuplicateException>();
    }

    [Fact]
    public async Task When_Product_Does_Not_Exist_Then_It_Should_Throw_NotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();

        var request = new ProductRequest { Name = "Product Name 2", Quantity = 10u };

        _repository.GetAsync(Arg.Any<Guid>())
                   .Returns((Product?)null);


        // Act
        var act = () => _useCase.ExecuteAsync(id, request);


        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
