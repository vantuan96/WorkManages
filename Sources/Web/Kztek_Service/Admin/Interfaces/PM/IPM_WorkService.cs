using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Model.Models.PM;

namespace Kztek_Service.Admin.Interfaces.PM
{
    public interface IPM_WorkService
    {
        Task<List<PM_Work>> GetAllByComponentId(string componentid);

        Task<List<PM_Work>> GetAllByProjectId(string projectid);

        Task<PM_Work> GetById(string id);

        Task<MessageReport> Create(PM_Work model);

        Task<MessageReport> Update(PM_Work model);

        Task<MessageReport> Delete(string id);

        Task<List<PM_Work>> GetAllUnfinishedWorkByUserId(string userid);

        Task<List<PM_Work>> GetAllUnfinishedWorkByUserId_ProjectId(string userid, string projectid);

        Task<PM_Work> GetByProjectId_ComponentId_UserId(string projectid, string componentid, string userid);

        Task<MessageReport> DeleteByComponentId(string componentid);

        Task<MessageReport> DeleteByProjectId(string projectid);

        Task<MessageReport> MCompleteByComponentId(string componentid);
    }
}
