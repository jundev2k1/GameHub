using game_x.application.Common.Files;
using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.domain.Entities.Rewards;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Rewards.Commands.CatalogItems.Create;

public sealed class CreateCatalogItemHandler(
    IUnitOfWork unitOfWork,
    ICatalogItemRepo repo,
    ICatalogItemCacheService cache,
    IFileStorageService fileStorage,
    ILogger<CreateCatalogItemHandler> logger
    ) : ICommandHandler<CreateCatalogItemCommand, Unit>
{
    public async Task<Unit> Handle(CreateCatalogItemCommand request, CancellationToken ct = default)
    {
        Guid id = Guid.CreateVersion7();
        var isExisted = await repo.CheckExistedCodeAsync(request.Code, ct);
        if (isExisted)
            throw new BadRequestException(MessageCode.Reward.CodeIsAlreadyExisted);
        
        var catalogItem = CatalogItem.Create(
            id: id,
            code: request.Code,
            name: request.Name,
            sortOrder: request.SortOrder,
            category: request.Category,
            description: request.Description,
            iconType: request.IconType,
            monetaryValue: request.MonetaryValue,
            iconValue: request.IconValue
        );

        if (request.Icon != null) catalogItem.OnUpdateIcon(await HandleFile(id, request.Icon, ct));

        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                await repo.AddAsync(catalogItem, ct);
                await unitOfWork.SaveChangesAsync(ct);
                await cache.RefreshCache(ct);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to create catalog item.");
                throw new BadRequestException("Failed to create catalog item.", e);
            }
        }, ct);
        
        return Unit.Value;
    }
    
    private async Task<MediaFile> HandleFile(Guid id, IFormFile formFile, CancellationToken ct)
    {
        var file = FileUpload.FromFormFile(formFile);
        
        var newFileName = Guid.NewGuid() + file.Extension;
        var objectName = ObjectName.CatalogItem(id, newFileName);
        await fileStorage.UploadFileAsync(file.Content, BucketName.CatalogItem, objectName, MimeType.Of(file.ContentType), ct);
        
        return MediaFile.Create(
            bucketName: BucketName.User,
            objectName: objectName,
            fileName: file.FileName,
            mimeType: MimeType.Of(file.ContentType),
            sizeBytes: Convert.ToInt32(file.Length));
    }
}