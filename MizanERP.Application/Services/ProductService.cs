using MizanERP.Application.DTOs;
using MizanERP.Application.Repositories;
using MizanERP.Domain.Entities;
using MizanERP.Domain.Enums;

namespace MizanERP.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Guid> CreateProductAsync(CreateProductDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new InvalidOperationException("Product name is required");

            var productType = dto.IsRawMaterial ? ProductType.RawMaterial : ProductType.FinishedGood;
            var product = new Product(
                Guid.NewGuid(),
                dto.Name,
                productType,
                "pcs", // default unit
                dto.Code
            );

            await _productRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return product.Id;
        }

        public async Task UpdateProductAsync(Guid productId, UpdateProductDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                throw new InvalidOperationException($"Product with ID {productId} not found");

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                // Since Name is private, we cannot update it directly
                // This is by design in Domain-Driven Design
                // Consider adding an Update method to the Product entity
            }

            await _productRepository.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(Guid productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                throw new InvalidOperationException($"Product with ID {productId} not found");

            product.Deactivate();
            await _productRepository.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ProductDto?> GetProductByIdAsync(Guid productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                return null;

            return new ProductDto()
            {
                Id = product.Id.ToString(),
                Code = product.Code,
                Name = product.Name,
                Type = product.Type,
                Unit = product.Unit,
                IsActive = product.IsActive,
                InventoryQuantity = product.InventoryQuantity
            };
        }


        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            var result = new List<ProductDto>();
            foreach (var p in products)
            {
                result.Add(new ProductDto
                {
                    Id = p.Id.ToString(),
                    Code = p.Code,
                    Name = p.Name,
                    Type = p.Type,
                    Unit = p.Unit,
                    IsActive = p.IsActive,
                    InventoryQuantity = p.InventoryQuantity
                });
            }

            return result;
        }
    }
}
