using Amazon;
using Amazon.CognitoIdentity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AWSAppService
{
    public interface IAWSAppService<T>
    {
        bool InitService();
    }
}
