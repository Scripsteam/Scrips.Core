﻿using Scrips.Core.Domain.Common.Contracts;

namespace Scrips.Core.Application.Common.Caching;

public static class CacheKeyServiceExtensions
{
    public static string GetCacheKey<TEntity>(this ICacheKeyService cacheKeyService, object id, bool includeTenantId = true)
    where TEntity : IEntity =>
        cacheKeyService.GetCacheKey(typeof(TEntity).Name, id, includeTenantId);
}