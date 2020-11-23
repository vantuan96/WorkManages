using System.Collections.Generic;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Library.Configs;
using Kztek_Library.Models;
using Kztek_Model.Models.WM;
using Kztek_Service.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kztek_Web.Apis.Mobile
{
    [Authorize(Policy = ApiConfig.Auth_Bearer_Mobile)]
    [Route("api/mobile/[controller]")]
    public class TaskController : Controller
    {
        private ITaskService _TaskService;

        public TaskController(ITaskService _TaskService)
        {
            this._TaskService = _TaskService;
        }

        [HttpGet("getcurrenttasks/{userid}")]
        public async Task<List<WM_TaskCustomView>> GetCurrentTasks(string userid)
        {
            return await _TaskService.GetCurrentTasksByUserId(userid);
        }

        [HttpPost("completetask")]
        public async Task<MessageReport> CompleteTask([FromBody] WM_TaskComplete model)
        {
            return await _TaskService.CompleteTask(model);
        }
    }
}