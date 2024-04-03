using Demo.Application.Services;
using Demo.Application.UseCases;
using Demo.Domain;
using NSubstitute;

namespace Demo.Application.Tests.UseCases;

public sealed class DeleteProductUseCaseTests
{
    private readonly IProductsRepository _repository;
    private readonly DeleteProductUseCase _useCase;

    public DeleteProductUseCaseTests()
    {
        _repository = Substitute.For<IProductsRepository>();
        _useCase = new(_repository);
    }


    [Fact]
    public async Task When_Product_Exists_Should_Be_Deleted()
    {
        // Arrange
        var id = Guid.NewGuid();

        _repository.ExistsAsync(id)
                   .Returns(true);


        // Act
        await _useCase.ExecuteAsync(id);


        // Assert
        await _repository
            .Received(1)
            .DeleteAsync(Arg.Any<Guid>());
    }

    [Fact]
    public async Task When_Product_Not_Exists_Should_Trow_NotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();

        _repository.ExistsAsync(id)
                   .Returns(false);

        // Act
        var result = () => _useCase.ExecuteAsync(id);


        // Assert
        await result.Should().ThrowAsync<NotFoundException>();
    }
}
