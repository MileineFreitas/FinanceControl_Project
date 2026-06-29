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
    public Task<DataResultDto<CategoryDto>> FilterAsync(DataFilterDto filter, bool activeOnly = false) =>
        repository.FilterAsync(filter, activeOnly);

    public Task<CategoryDto?> GetByIdAsync(Guid id) =>
        repository.GetByIdAsync(id);

    public async Task<CategoryDto> CreateAsync(CategoryRegisterDto dto)
    {
        var userId = await repository.GetFirstUserIdAsync();
        var entity = domService.CreateFromRegister(dto, userId);
        await repository.AddAsync(entity);
        return (await repository.GetByIdAsync(entity.CategoryId))!;
    }

    public async Task<CategoryDto?> UpdateAsync(CategoryUpdateDto dto)
    {
        var entity = await repository.FindTrackedAsync(dto.CategoryId);
        if (entity == null) return null;

        domService.ApplyUpdate(entity, dto);
        await repository.SaveChangesAsync();
        return await repository.GetByIdAsync(entity.CategoryId);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            return await repository.DeleteAsync(id);
        }
        catch (DbUpdateException)
        {
            throw new InvalidOperationException("Não é possível excluir: existem transações vinculadas a esta categoria.");
        }
    }
}
