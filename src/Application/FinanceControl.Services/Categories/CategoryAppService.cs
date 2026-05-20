using FinanceControl.Contracts.Dtos.Categories;
using FinanceControl.Contracts.Dtos.Common;
using FinanceControl.Contracts.Filters;
using FinanceControl.Domain.Interfaces.AppServices.Categories;
using FinanceControl.Domain.Interfaces.DomService.Categories;
using FinanceControl.Domain.Interfaces.Repositories.Categories;
using FinanceControl.Domain.MapperProfiles.Categories;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Services.Categories;

public class CategoryAppService(
    ICategoryRepository repository,
    ICategoryDomService domService) : ICategoryAppService
{
    public Task<DataResultDto<CategoryDto>> FilterAsync(DataFilterDto filter, CancellationToken cancellationToken = default) =>
        repository.FilterAsync(filter, cancellationToken);

    public Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        repository.GetByIdAsync(id, cancellationToken);

    public async Task<CategoryDto> CreateAsync(CategoryRegisterDto dto, CancellationToken cancellationToken = default)
    {
        var userId = await repository.GetFirstUserIdAsync(cancellationToken);
        var entity = domService.CreateFromRegister(dto, userId);
        await repository.AddAsync(entity, cancellationToken);
        return (await repository.GetByIdAsync(entity.CategoryId, cancellationToken))!;
    }

    public async Task<CategoryDto?> UpdateAsync(CategoryUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await repository.FindTrackedAsync(dto.CategoryId, cancellationToken);
        if (entity == null) return null;

        domService.ApplyUpdate(entity, dto);
        await repository.SaveChangesAsync(cancellationToken);
        return await repository.GetByIdAsync(entity.CategoryId, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await repository.DeleteAsync(id, cancellationToken);
        }
        catch (DbUpdateException)
        {
            throw new InvalidOperationException("Não é possível excluir: existem transações vinculadas a esta categoria.");
        }
    }
}
