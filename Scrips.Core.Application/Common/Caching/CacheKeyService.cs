﻿using Scrips.Core.Application.Common.Interfaces;

namespace Scrips.Core.Application.Common.Caching;

public interface ICacheKeyService : IScopedService
{
    public string GetCacheKey(string name, object id, bool includeTenantId = true);
}