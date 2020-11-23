using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Model.Models.PM;

namespace Kztek_Service.Admin.Interfaces.PM
{
    public interface IPM_ProjectService
    {
        Task<GridModel<PM_Project>> GetPaging(string key, int pageNumber, int pageSize);

        Task<PM_Project> GetById(string id);

        Task<MessageReport> Create(PM_Project model);

        Task<MessageReport> Update(PM_Project model);

        Task<MessageReport> Delete(string id);

        Task<List<PM_Project>> GetProjectsByIds(List<string> ids, int status = 0);
    }
}
