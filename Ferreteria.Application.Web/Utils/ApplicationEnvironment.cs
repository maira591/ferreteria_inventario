using AutoMapper;
using Ferreteria.Application.Website.Models;
using Ferreteria.Infrastructure.Configuration;
using Microsoft.AspNetCore.Http;
using InfrastructureUserBasic = Ferreteria.Infrastructure.Model.UserBasic;

namespace Ferreteria.Application.Website.Utils
{
    public class ApplicationEnvironment : IApplicationEnvironment
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly IMapper _mapper;

        private string CurrentIpContext { get; set; }
        private string CurrentUserId { get; set; }

        public ApplicationEnvironment(IHttpContextAccessor accessor, IMapper mapper)
        {
            _accessor = accessor;
            _mapper = mapper;
        }

        public ApplicationEnvironment(string currentIpContext, string currentUserId)
        {
            CurrentIpContext = currentIpContext;
            CurrentUserId = currentUserId;
        }

        public ApplicationEnvironment()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetConnectionString()
        {
            var ac = new ApplicationConfigurator();
            return ac.GetKey("connectionStringValue");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetCurrentUser()
        {
            var user = _accessor.HttpContext.Session.GetObject<UserBasicModel>("Identity");
            return user.Email;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public InfrastructureUserBasic GetCurrentUserBasic()
        {
            var user = _accessor.HttpContext.Session.GetObject<UserBasicModel>("Identity");
            return _mapper.Map<InfrastructureUserBasic>(user);
        }

        public string GetDefaultSchema()
        {
            var ac = new ApplicationConfigurator();
            return ac.GetKey("HasDefaultSchema");
        }

        public string GetIp()
        {
            string currentIp = string.IsNullOrEmpty(CurrentIpContext) ? _accessor.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString() : CurrentIpContext;
            return currentIp;
        }

        
    }
}