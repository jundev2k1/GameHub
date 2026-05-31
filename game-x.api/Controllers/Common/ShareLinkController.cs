namespace game_x.api.Controllers.Common;

[AllowAnonymous]
[Route("api/common")]
public sealed class ShareLinkController : BaseApiController
{
    // [HttpGet("share/{code}")]
    // public IActionResult GetAllCountriesAsync(string code)
    // {
    //     var countries = search.IsNotNullOrEmpty()
    //         ? [.. CountryInfo.AllCountries.Where(c => c.CountryName.Contains(search!, StringComparison.OrdinalIgnoreCase))]
    //         : CountryInfo.AllCountries;
    //     return ApiResponseFactory.Ok(countries);
    // }
}