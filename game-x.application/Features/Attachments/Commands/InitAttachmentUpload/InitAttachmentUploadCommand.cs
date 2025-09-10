using game_x.application.Common.Files;
using game_x.application.Features.Attachments.Dtos;

namespace game_x.application.Features.Attachments.Commands.InitAttachmentUpload;

public sealed record InitAttachmentUploadCommand(InitUploadRequest Request) : ICommand<InitUploadResponse>;
// public sealed record InitAttachmentUploadCommand(ICollection<FileUpload> FileUploads) : ICommand<InitUploadResponse>;