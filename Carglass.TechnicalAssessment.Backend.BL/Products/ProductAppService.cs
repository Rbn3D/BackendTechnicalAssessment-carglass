using AutoMapper;
using Carglass.TechnicalAssessment.Backend.DL.Repositories;
using Carglass.TechnicalAssessment.Backend.Dtos;
using Carglass.TechnicalAssessment.Backend.Entities;
using FluentValidation;

namespace Carglass.TechnicalAssessment.Backend.BL;

internal class ProductAppService : CrudAppService<ProductDto, Product>, IProductAppService
{
    public ProductAppService(ICrudRepository<Product> productRepository, IMapper mapper, IValidator<ProductDto> productValidator)
        : base(productRepository, mapper, productValidator)
    { }

    protected override string EntityName => "producto";
}
