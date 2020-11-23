using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Library.Configs;
using Kztek_Library.Models;
using Kztek_Model.Models.PM;
using Kztek_Service.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Kztek_Web.Apis.Mobile
{
    [Authorize(Policy = ApiConfig.Auth_Bearer_Mobile)]
    [Route("api/mobile/[controller]")]
    public class ProjectController : Controller
    {
        private IProjectService _ProjectService;

        public ProjectController(IProjectService _ProjectService)
        {
            this._ProjectService = _ProjectService;
        }

        [HttpGet("getcurrentprojects/{userid}")]
        public async Task<List<PM_ProjectCustomView>> GetCurrentProjects(string userid)
        {
            return await _ProjectService.GetProjectsByUserId(userid);
        }

        [HttpGet("getprojectdetail/{userid}/{projectid}")]
        public async Task<List<PM_ComponentCustomView>> GetProjectDetail(string userid, string projectid)
        {
            return await _ProjectService.GetComponentsByUserId_ProjectId(userid, projectid);
        }

        [HttpGet("getcomponentdetail/{componentid}")]
        public async Task<PM_ComponentCustomView> GetComponentDetail(string componentid)
        {
            return await _ProjectService.GetComponentsById(componentid);
        }

        [HttpPost("completecomponent")]
        public async Task<MessageReport> CompleteComponent([FromBody] PM_ComponentComplete model)
        {
            return await _ProjectService.CompleteComponent(model);
        }
    }
}
