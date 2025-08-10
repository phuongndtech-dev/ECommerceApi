using FluentValidation;
using ECommerceApi.Application.DTOs.Products;

namespace ECommerceApi.Application.Validators
{
    public class ProductSearchDtoValidator : AbstractValidator<ProductSearchDto>
    {
        public ProductSearchDtoValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage("Page must be greater than 0");

            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0")
                .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100");

            RuleFor(x => x.MinPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Minimum price cannot be negative")
                .When(x => x.MinPrice.HasValue);

            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Maximum price cannot be negative")
                .When(x => x.MaxPrice.HasValue);

            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(x => x.MinPrice)
                .WithMessage("Maximum price must be greater than or equal to minimum price")
                .When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue);

            RuleFor(x => x.SortBy)
                .Must(BeValidSortField)
                .WithMessage("Sort by must be one of: Name, Price, CreatedAt, Stock");
        }

        private static bool BeValidSortField(string sortBy)
        {
            var validFields = new[] { "Name", "Price", "CreatedAt", "Stock" };
            return validFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase);
        }
    }
}