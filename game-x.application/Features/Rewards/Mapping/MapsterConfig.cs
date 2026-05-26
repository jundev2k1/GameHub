using game_x.application.Features.Rewards.Dtos;
using game_x.domain.Entities.Rewards;
using game_x.domain.Enum.Rewards;

namespace game_x.application.Features.Rewards.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<Mission, MissionDto>()
            .Map(dest => dest.Id, src => src.PublicId);
        
        cfg.NewConfig<Mission, MissionListedAdminDto>()
            .Map(dest => dest.Id, src => src.PublicId);
        
        cfg.NewConfig<Mission, MissionListedUserDto>()
            .Map(dest => dest.Id, src => src.PublicId);
        
        cfg.NewConfig<CatalogItem, CatalogItemDto>()
            .Map(dest => dest.Id, src => src.PublicId);
        
        cfg.NewConfig<RewardPool, RewardPoolDto>()
            .Map(dest => dest.Id, src => src.PublicId);

        cfg.NewConfig<RewardDefinition, RewardDefinitionDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.ItemId, src => src.CatalogItem != null ? (Guid?)src.CatalogItem.PublicId : null)
            .Map(dest => dest.ItemName, src => src.CatalogItem != null ? src.CatalogItem.Name : null)
            .Map(dest => dest.ItemIconType, src => src.CatalogItem != null ? (CatalogItemIconType?)src.CatalogItem.IconType : null)
            .Map(dest => dest.ItemIconValue, src => src.CatalogItem != null ? src.CatalogItem.IconValue : null);
        
        cfg.NewConfig<RewardPoolItem, RewardPoolItemDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.RewardDefinitionId, src => src.RewardDefinition != null ? (Guid?)src.RewardDefinition.PublicId : null)
            .Map(dest => dest.RewardDefinitionCode, src => src.RewardDefinition != null ? src.RewardDefinition.Code : null)
            .Map(dest => dest.Amount, src => src.RewardDefinition != null ? src.RewardDefinition.Amount : null)
            .Map(dest => dest.Title, src => src.RewardDefinition != null ? src.RewardDefinition.Title : null)
            .Map(dest => dest.Description, src => src.RewardDefinition != null ? src.RewardDefinition.Description : null)
            .Map(dest => dest.ItemId, src => src.RewardDefinition != null && src.RewardDefinition.CatalogItem != null ? 
                (Guid?)src.RewardDefinition.CatalogItem.PublicId : null)
            .Map(dest => dest.RewardType, src => src.RewardDefinition != null ? (RewardItemType?)src.RewardDefinition.Type : null)
            .Map(dest => dest.ItemName, src => src.RewardDefinition != null && src.RewardDefinition.CatalogItem != null 
                ? src.RewardDefinition.CatalogItem.Name : null)
            .Map(dest => dest.ItemIconType, src => src.RewardDefinition != null && src.RewardDefinition.CatalogItem != null 
                ? (CatalogItemIconType?)src.RewardDefinition.CatalogItem.IconType : null)
            .Map(dest => dest.ItemIconValue, src => src.RewardDefinition != null && src.RewardDefinition.CatalogItem != null 
                ? src.RewardDefinition.CatalogItem.IconValue : null);
        
        cfg.NewConfig<UserInventory, UserInventoryDto>()
            .Map(dest => dest.CatalogItemId, src => src.Item!.PublicId)
            .Map(dest => dest.Code, src => src.Item!.Code)
            .Map(dest => dest.Name, src => src.Item!.Name)
            .Map(dest => dest.CatalogItemId, src => src.Item!.PublicId)
            .Map(dest => dest.Category, src => src.Item!.Category)
            .Map(dest => dest.IconType, src => src.Item!.IconType)
            .Map(dest => dest.IconValue, src => src.Item!.IconValue);
        
        cfg.NewConfig<MissionReward, MissionRewardDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.RewardDefinitionId, src => src.RewardDefinition != null ? (Guid?)src.RewardDefinition.PublicId : null)
            .Map(dest => dest.ItemId, src => src.RewardDefinition != null && src.RewardDefinition.CatalogItem != null ? 
                (Guid?)src.RewardDefinition.CatalogItem.PublicId : null)
            .Map(dest => dest.RewardType, src => src.RewardDefinition != null ? (RewardItemType?)src.RewardDefinition.Type : null)
            .Map(dest => dest.ItemName, src => src.RewardDefinition != null && src.RewardDefinition.CatalogItem != null 
                ? src.RewardDefinition.CatalogItem.Name : null)
            .Map(dest => dest.ItemIconType, src => src.RewardDefinition != null && src.RewardDefinition.CatalogItem != null 
                ? (CatalogItemIconType?)src.RewardDefinition.CatalogItem.IconType : null)
            .Map(dest => dest.ItemIconValue, src => src.RewardDefinition != null && src.RewardDefinition.CatalogItem != null 
                ? src.RewardDefinition.CatalogItem.IconValue : null)
            .Map(dest => dest.ItemIcon, src => src.RewardDefinition != null && src.RewardDefinition.CatalogItem != null 
                ? src.RewardDefinition.CatalogItem.Icon : null);
    }
}