namespace game_x.api.Controllers.Common;

[Authorize(Roles = $"{AppRoles.Root},{AppRoles.Admin},{AppRoles.Cs},{AppRoles.User}")]
[Route("api/common")]
public sealed class CountryController : BaseApiController
{
    [HttpGet("countries")]
    public IActionResult GetAllCountriesAsync([FromQuery] string? search = null)
    {
        var countries = search.IsNotNullOrEmpty()
            ? [.. CountryInfo.AllCountries.Where(c => c.CountryName.Contains(search!, StringComparison.OrdinalIgnoreCase))]
            : CountryInfo.AllCountries;
        return ApiResponseFactory.Ok(countries);
    }
}