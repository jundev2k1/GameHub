using game_x.application.Common.Files;
using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Rewards.Commands.CatalogItems.Update;

public sealed class UpdateCatalogItemHandler(
    IUnitOfWork unitOfWork,
    ICatalogItemRepo repo,
    ICatalogItemCacheService cache,
    IFileStorageService fileStorage,
    ILogger<UpdateCatalogItemHandler> logger
    ) : ICommandHandler<UpdateCatalogItemCommand, Unit>
{
    public async Task<Unit> Handle(UpdateCatalogItemCommand cmd, CancellationToken ct = default)
    {
        await Validate(cmd, ct);

        var iconFile = cmd.Icon != null ? await HandleFile(cmd.Id, cmd.Icon, ct) : null;
        
        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                await repo.UpdateAsync(cmd.Id, x =>
                {
                    x.OnUpdate(
                        code: cmd.Code,
                        name: cmd.Name, 
                        description: cmd.Description,
                        category: cmd.Category,
                        monetaryValue: cmd.MonetaryValue,
                        iconType: cmd.IconType,
                        iconValue: cmd.IconValue,
                        isActive: cmd.IsActive,
                        sortOrder: cmd.SortOrder);
                    
                    if(iconFile != null) x.OnUpdateIcon(iconFile);
                }, ct);
                await unitOfWork.CommitAsync(ct);
                await cache.RefreshCache(ct);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to update catalog item.");
                throw;
            }
        }, ct);
        
        return Unit.Value;
    }

    private async Task Validate(UpdateCatalogItemCommand cmd, CancellationToken ct = default)
    {
        if (cmd.Code != null)
        {
            bool isExisted = await repo.CheckExistedCodeAsync(cmd.Code, ct);
            if (isExisted)
                throw new BadRequestException(MessageCode.Reward.CodeIsAlreadyExisted);
        }
    }
    
    private async Task<MediaFile> HandleFile(Guid id, IFormFile formFile, CancellationToken ct)
    {
        var file = FileUpload.FromFormFile(formFile);
        
        var newFileName = Guid.NewGuid() + file.Extension;
        var objectName = ObjectName.CatalogItem(id, newFileName);
        await fileStorage.UploadFileAsync(file.Content, BucketName.CatalogItem, objectName, MimeType.Of(file.ContentType), ct);
        
        return MediaFile.Create(
            bucketName: BucketName.CatalogItem,
            objectName: objectName,
            fileName: file.FileName,
            mimeType: MimeType.Of(file.ContentType),
            sizeBytes: Convert.ToInt32(file.Length));
    }
}