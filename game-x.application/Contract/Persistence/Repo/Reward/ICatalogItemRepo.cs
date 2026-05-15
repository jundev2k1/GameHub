using game_x.application.Features.Rewards.Dtos;
using game_x.domain.Entities.Rewards;

namespace game_x.application.Contract.Persistence.Repo.Reward;

public interface ICatalogItemRepo
{
    Task<CatalogItemDto[]> GetListAsync(CancellationToken ct = default);
    
    Task<bool> CheckExistedCodeAsync(string code, CancellationToken ct = default);
    
    Task<CatalogItem> GetByIdAsync(Guid id, CancellationToken ct = default);
    
    Task<CatalogItem> GetByCodeAsync(string code, CancellationToken ct = default);
    
    Task AddAsync(CatalogItem entity, CancellationToken ct = default);
}