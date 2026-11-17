using System;
using System.Collections.Generic;
using System.Text;
using XuePing.Application.Common;
using XuePing.Application.DTOs;

namespace XuePing.Application.Services
{
    public interface IPublishPostAsync
    {
        Task<Result<PostDto>> ExecuteAsync(long userId,CreatePostRequest request, CancellationToken cancellationToken = default);
    }
}
