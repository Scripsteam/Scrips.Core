using Scrips.Core.Application.Common.Interfaces;
using Scrips.Core.Domain.Common;

namespace Scrips.Core.Application.Common.FileStorage;

public interface IFileStorageService : ITransientService
{
    public Task<string> UploadAsync<T>(FileUploadRequest? request, FileType supportedFileType, CancellationToken cancellationToken = default)
    where T : class;

    public void Remove(string? path);
}