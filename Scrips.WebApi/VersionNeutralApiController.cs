using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Scrips.WebApi;

[Route("api/[controller]")]
[ApiVersionNeutral]
public class VersionNeutralApiController : BaseApiController
{
}
