using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Library.Models;
using Kztek_Model.Models.PM;

namespace Kztek_Service.Api.Interfaces
{
    public interface IProjectService
    {
        Task<List<PM_ProjectCustomView>> GetProjectsByUserId(string userid, int status = 0);

        Task<List<PM_ComponentCustomView>> GetComponentsByUserId_ProjectId(string userid, string projectid, int status = 0);

        Task<PM_ComponentCustomView> GetComponentsById(string componentid);

        Task<MessageReport> CompleteComponent(PM_ComponentComplete model);
    }
}
