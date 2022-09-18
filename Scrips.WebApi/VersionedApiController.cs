using Microsoft.AspNetCore.Components;

namespace Scrips.WebApi;

[Route("api/v{version:apiVersion}/[controller]")]
public class VersionedApiController : BaseApiController
{
}
