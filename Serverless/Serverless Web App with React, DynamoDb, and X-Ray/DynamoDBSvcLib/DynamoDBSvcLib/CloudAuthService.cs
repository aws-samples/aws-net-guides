using System;
using System.Collections.Generic;
using System.Text;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.Runtime;
using AWSAppService.Data;
using Microsoft.Extensions.Options;

namespace AWSAppService
{
    namespace Auth
    {
        public class CloudAuthService<T> : IAWSAppService<IData>
        {
            private CognitoAWSCredentials _cognitoAWSCredentials;
            private Amazon.RegionEndpoint _awsRegion;
            private AWSOptions _awsOptions;

            //public CloudAuthService(string appPoolID, string awsRegion):
            //    this(appPoolID, Amazon.RegionEndpoint.GetBySystemName(awsRegion)){ }

            public CloudAuthService(IOptions<AWSOptions> awsOptions)
            {
                _awsOptions = awsOptions.Value;
                _awsRegion = Amazon.RegionEndpoint.GetBySystemName(
                                           _awsOptions.Region);
                if (!InitService()) throw new AmazonCognitoIdentityException("Couldn't connect to Cognito Service", new Exception("InitService - CognitoClient"));
            }

            //public AWSCredentials GetAWSCredentials()
            public CognitoAWSCredentials GetAWSCredentials()
            {
                return _cognitoAWSCredentials;
            }

            public bool InitService( ) {

                _cognitoAWSCredentials = new CognitoAWSCredentials(_awsOptions.CognitoPoolId, _awsRegion);

                return _cognitoAWSCredentials != null ? true : false;
            }



            //Add Cognito UserPool features here. 
        }
    }
}
