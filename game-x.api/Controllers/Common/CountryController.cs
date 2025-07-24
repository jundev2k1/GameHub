using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace game_x.api.Controllers.Common;

[Authorize(Roles = $"{AppRoles.Root}, {AppRoles.Admin},{AppRoles.Staff},{AppRoles.User}")]
[Route("api/common")]
public class CountryController : BaseApiController
{
    [HttpGet("countries")]
    public IActionResult GetAllCountries([FromQuery] string? search = null)
    {
        var countries = search.IsNotNullOrEmpty()
            ? CountryInfo.AllCountries
                .Where(c => c.CountryName.Contains(search!, StringComparison.OrdinalIgnoreCase))
                .ToList()
            : CountryInfo.AllCountries;
        return ApiResponseFactory.Ok(countries);
    }
}
