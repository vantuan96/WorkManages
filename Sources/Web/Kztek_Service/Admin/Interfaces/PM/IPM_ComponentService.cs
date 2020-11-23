using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Model.Models.PM;

namespace Kztek_Service.Admin.Interfaces.PM
{
    public interface IPM_ComponentService
    {
        Task<List<PM_Component>> GetAllByProjectId(string projectid);

        Task<List<PM_Component_Submit>> GetAllCustomByProjectId(string projectid);

        Task<PM_Component> GetById(string id);

        Task<PM_Component> GetByCode(string projectid, string code);

        Task<PM_Component_Submit> GetCustomById(string id);

        Task<PM_Component_Submit> GetCustomByModel(PM_Component model);

        Task<MessageReport> Create(PM_Component model);

        Task<MessageReport> Update(PM_Component model);

        Task<MessageReport> Delete(string id);

        Task<List<PM_Component>> GetAllByIds(List<string> ids, int status = 0);

        Task<MessageReport> DeleteByProjectId(string projectid);

        Task<MessageReport> MComplete(string id);

        Task<List<PM_Component>> GetAllCurrentByIds(List<string> ids, int status = 0);
    }
}
