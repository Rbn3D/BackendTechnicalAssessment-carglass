using AutoMapper;
using Carglass.TechnicalAssessment.Backend.BL;
using Carglass.TechnicalAssessment.Backend.BL.Products.Converter;
using Carglass.TechnicalAssessment.Backend.DL.Repositories;
using Carglass.TechnicalAssessment.Backend.Dtos;
using Carglass.TechnicalAssessment.Backend.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Carglass.TechnicalAssessment.Backend.Tests.Unit.BL;

public class ProductAppServiceTests
{
    private static readonly DateTime SoldAt = new(2019, 1, 9, 14, 26, 17);

    private readonly Mock<ICrudRepository<Product>> _repository = new();
    private readonly ProductAppService _service;

    public ProductAppServiceTests()
    {
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<ProductProfileConverter>())
            .CreateMapper();

        _service = new ProductAppService(_repository.Object, mapper, new ProductDtoValidator(),
                                         NullLogger<ProductAppService>.Instance);
    }

    private static ProductDto ValidDto(int id = 10, string productName = "Luna trasera") => new()
    {
        Id = id,
        ProductName = productName,
        ProductType = 25,
        NumTerminal = 933933933,
        SoldAt = SoldAt
    };

    private static Product Entity(int id = 10, string productName = "Luna trasera") => new()
    {
        Id = id,
        ProductName = productName,
        ProductType = 25,
        NumTerminal = 933933933,
        SoldAt = SoldAt
    };

    [Fact]
    public void GetAll_maps_entities_to_dtos()
    {
        _repository.Setup(r => r.GetAll()).Returns(new[] { Entity(1, "Luna trasera"), Entity(2, "Luna frontal") });

        var result = _service.GetAll().ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("Luna frontal", result.Single(c => c.Id == 2).ProductName);
    }

    [Fact]
    public void GetById_nonexistent_throws_NotFoundException()
    {
        Assert.Throws<NotFoundException>(() => _service.GetById(999));
    }

    [Fact]
    public void Create_valid_client_persists_once()
    {
        var dto = ValidDto();

        _service.Create(dto);

        _repository.Verify(r => r.Create(It.Is<Product>(c =>
            c.Id == dto.Id && c.ProductName == dto.ProductName && c.ProductType == dto.ProductType)), Times.Once);
    }

    [Fact]
    public void Create_existing_id_throws_Conflict_and_does_not_persist()
    {
        _repository.Setup(r => r.GetById(10)).Returns(Entity(10));

        Assert.Throws<ConflictException>(() => _service.Create(ValidDto(10)));

        _repository.Verify(r => r.Create(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public void Update_nonexistent_throws_NotFound_and_does_not_update()
    {
        Assert.Throws<NotFoundException>(() => _service.Update(ValidDto(999)));

        _repository.Verify(r => r.Update(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public void Update_existing_calls_repository_with_new_values()
    {
        _repository.Setup(r => r.GetById(10)).Returns(Entity(10));
        var dto = ValidDto(10);
        dto.ProductName = "Updated Product Name";

        _service.Update(dto);

        _repository.Verify(r => r.Update(It.Is<Product>(c => c.Id == 10 && c.ProductName == "Updated Product Name")), Times.Once);
    }

    [Fact]
    public void Delete_nonexistent_throws_NotFound()
    {
        Assert.Throws<NotFoundException>(() => _service.Delete(ValidDto(999)));

        _repository.Verify(r => r.Delete(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public void Delete_existing_deletes_the_stored_entity()
    {
        var stored = Entity(10);
        _repository.Setup(r => r.GetById(10)).Returns(stored);

        _service.Delete(ValidDto(10));

        _repository.Verify(r => r.Delete(stored), Times.Once);
    }

    [Fact]
    public void Create_with_same_name_as_existing_is_allowed()
    {
        _repository.Setup(r => r.GetAll()).Returns(new[] { Entity(1, "Luna trasera") });

        _service.Create(ValidDto(10, "Luna trasera"));

        _repository.Verify(r => r.Create(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public void NotFound_message_mentions_producto()
    {
        var exception = Assert.Throws<NotFoundException>(() => _service.GetById(999));

        Assert.Contains("producto", exception.Message);
    }

}
