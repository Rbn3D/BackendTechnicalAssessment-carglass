using AutoMapper;
using Carglass.TechnicalAssessment.Backend.BL;
using Carglass.TechnicalAssessment.Backend.BL.Clients.Converter;
using Carglass.TechnicalAssessment.Backend.DL.Repositories;
using Carglass.TechnicalAssessment.Backend.Dtos;
using Carglass.TechnicalAssessment.Backend.Entities;
using FluentValidation;
using Moq;

namespace Carglass.TechnicalAssessment.Backend.Tests.Unit.BL;

public class ClientAppServiceTests
{
    private readonly Mock<ICrudRepository<Client>> _repository = new();
    private readonly ClientAppService _service;

    public ClientAppServiceTests()
    {
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<ClientProfileConverter>())
            .CreateMapper();

        _service = new ClientAppService(_repository.Object, mapper, new ClientDtoValidator());
    }

    private static ClientDto ValidDto(int id = 10, string docNum = "12345678Z") => new()
    {
        Id = id,
        DocType = "nif",
        DocNum = docNum,
        Email = "test@sample.com",
        GivenName = "Test",
        FamilyName1 = "User",
        Phone = "600000000"
    };

    private static Client Entity(int id = 10, string docNum = "12345678Z") => new()
    {
        Id = id,
        DocType = "nif",
        DocNum = docNum,
        Email = "test@sample.com",
        GivenName = "Test",
        FamilyName1 = "User",
        Phone = "600000000"
    };

    [Fact]
    public void GetAll_maps_entities_to_dtos()
    {
        _repository.Setup(r => r.GetAll()).Returns(new[] { Entity(1, "11111111H"), Entity(2, "22222222J") });

        var result = _service.GetAll().ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("22222222J", result.Single(c => c.Id == 2).DocNum);
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

        _repository.Verify(r => r.Create(It.Is<Client>(c =>
            c.Id == dto.Id && c.DocNum == dto.DocNum && c.Email == dto.Email)), Times.Once);
    }

    [Fact]
    public void Create_existing_id_throws_Conflict_and_does_not_persist()
    {
        _repository.Setup(r => r.GetById(10)).Returns(Entity(10));

        Assert.Throws<ConflictException>(() => _service.Create(ValidDto(10)));

        _repository.Verify(r => r.Create(It.IsAny<Client>()), Times.Never);
    }

    [Fact]
    public void Create_duplicated_docnum_throws_Conflict_and_does_not_persist()
    {
        _repository.Setup(r => r.GetAll()).Returns(new[] { Entity(1, "12345678Z") });

        Assert.Throws<ConflictException>(() => _service.Create(ValidDto(10, "12345678Z")));

        _repository.Verify(r => r.Create(It.IsAny<Client>()), Times.Never);
    }

    [Fact]
    public void Create_invalid_dto_throws_ValidationException_before_touching_repository()
    {
        var dto = ValidDto(docNum: "1234567Z");

        Assert.Throws<ValidationException>(() => _service.Create(dto));

        _repository.VerifyNoOtherCalls();
    }

    [Fact]
    public void Update_nonexistent_throws_NotFound_and_does_not_update()
    {
        Assert.Throws<NotFoundException>(() => _service.Update(ValidDto(999)));

        _repository.Verify(r => r.Update(It.IsAny<Client>()), Times.Never);
    }

    [Fact]
    public void Update_existing_calls_repository_with_new_values()
    {
        _repository.Setup(r => r.GetById(10)).Returns(Entity(10));
        var dto = ValidDto(10);
        dto.Email = "new@sample.com";

        _service.Update(dto);

        _repository.Verify(r => r.Update(It.Is<Client>(c => c.Id == 10 && c.Email == "new@sample.com")), Times.Once);
    }

    [Fact]
    public void Delete_nonexistent_throws_NotFound()
    {
        Assert.Throws<NotFoundException>(() => _service.Delete(ValidDto(999)));

        _repository.Verify(r => r.Delete(It.IsAny<Client>()), Times.Never);
    }

    [Fact]
    public void Delete_existing_deletes_the_stored_entity()
    {
        var stored = Entity(10);
        _repository.Setup(r => r.GetById(10)).Returns(stored);

        _service.Delete(ValidDto(10));

        _repository.Verify(r => r.Delete(stored), Times.Once);
    }

}
