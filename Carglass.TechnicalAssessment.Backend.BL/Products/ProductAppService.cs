using AutoMapper;
using Carglass.TechnicalAssessment.Backend.DL.Repositories;
using Carglass.TechnicalAssessment.Backend.Dtos;
using Carglass.TechnicalAssessment.Backend.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Carglass.TechnicalAssessment.Backend.BL;

internal class ProductAppService : CrudAppService<ProductDto, Product>, IProductAppService
{
    public ProductAppService(ICrudRepository<Product> productRepository, IMapper mapper, IValidator<ProductDto> productValidator, ILogger<ProductAppService> logger)
        : base(productRepository, mapper, productValidator, logger)
    { }

    protected override string EntityName => "producto";
}
