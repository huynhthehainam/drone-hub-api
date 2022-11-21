
using MiSmart.API.Protos;
using static MiSmart.API.Protos.AuthProtoService;
using MiSmart.API.Settings;
using System;
using Microsoft.Extensions.Options;

namespace MiSmart.API.GrpcServices
{
    public class AuthGrpcClientService
    {
        private readonly AuthProtoServiceClient authProtoServiceClient;
        private readonly InternalServiceSettings internalServiceSettings;
        public AuthGrpcClientService(AuthProtoServiceClient authProtoServiceClient, IOptions<InternalServiceSettings> options)
        {
            this.authProtoServiceClient = authProtoServiceClient;
            this.internalServiceSettings = options.Value;
        }
        public UserProtoModel? GetUserInfo(Guid uuid)
        {
            GetUserInfoProtoRequest request = new GetUserInfoProtoRequest { SecretKey = internalServiceSettings.SecretKey, Uuid = uuid.ToString() };
            var resp = authProtoServiceClient.GetUserInfo(request);
            if (resp.Id == 0)
            {
                return null;
            }
            return resp;

        }
        public UserExistingInformationProtoModel GetUserExistingInformation(String encryptedUUID)
        {
            GetUserExistingInformationRequest request = new GetUserExistingInformationRequest { EncryptedUUID = encryptedUUID };
            var resp = authProtoServiceClient.GetUserExistingInformation(request);

            return resp;
        }

    }
}