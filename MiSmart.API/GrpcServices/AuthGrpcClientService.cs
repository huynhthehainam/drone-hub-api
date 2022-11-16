
using MiSmart.API.Protos;
using static MiSmart.API.Protos.AuthProtoService;
using System;

namespace MiSmart.API.GrpcServices
{
    public class AuthGrpcClientService
    {
        private readonly AuthProtoServiceClient authProtoServiceClient;
        public AuthGrpcClientService(AuthProtoServiceClient authProtoServiceClient)
        {
            this.authProtoServiceClient = authProtoServiceClient;
        }
        public UserProtoModel GetUserInfo(Int64 id)
        {
            GetUserInfoProtoRequest request = new GetUserInfoProtoRequest { Id = id };
            var resp = authProtoServiceClient.GetUserInfo(request);

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