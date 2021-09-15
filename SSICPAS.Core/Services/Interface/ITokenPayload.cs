using System;

namespace SSICPAS.Core.Services.Interface
{
    public interface ITokenPayload
    {
        DateTime? ExpiredDateTime();
    }
}