using System.Text.Json.Serialization;

namespace game_x.application.Contract.Infrastructure.Dto;

public record MediaFileInfo([property: JsonIgnore] int Id, string FileName, string Url);
