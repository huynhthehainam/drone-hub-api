
using MiSmart.API.Protos;
using static MiSmart.API.Protos.AuthProtoService;
using System;
using System.Threading.Tasks;

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
            if (resp.Id != 0)
            {
                return resp;
            }
            return null;
        }
        public UserExistingInformationProtoModel GetUserExistingInformation(Int64 id)
        {
            GetUserExistingInformationRequest request = new GetUserExistingInformationRequest { Id = id };
            var resp = authProtoServiceClient.GetUserExistingInformation(request);

            return resp;
        }

    }
}