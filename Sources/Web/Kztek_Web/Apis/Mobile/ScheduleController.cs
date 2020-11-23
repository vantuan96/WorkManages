using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Library.Configs;
using Kztek_Library.Models;
using Kztek_Service.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Kztek_Web.Apis.Mobile
{
    [Authorize(Policy = ApiConfig.Auth_Bearer_Mobile)]
    [Route("api/mobile/[controller]")]
    public class ScheduleController : Controller
    {
        private IScheduleService _ScheduleService;

        public ScheduleController(IScheduleService _ScheduleService)
        {
            this._ScheduleService = _ScheduleService;
        }

        [HttpGet("getcurrentschedule/{userid}")]
        public async Task<WM_ScheduleMobile> GetCurrentSchedule(string userid)
        {
            return await _ScheduleService.GetCurrentScheduleByUserId(userid);
        }

        [HttpPost("adddiary")]
        public async Task<MessageReport> AddDiary([FromBody]WM_DiaryMobile model)
        {
            return await _ScheduleService.AddDiary(model);
        }

        [HttpPost("editdiary")]
        public async Task<MessageReport> EditDiary([FromBody]WM_DiaryMobile model)
        {
            return await _ScheduleService.EditDiary(model);
        }

        [HttpDelete("{id}")]
        public async Task<MessageReport> Delete(string id)
        {
            return await _ScheduleService.DeleteDiary(id);
        }
    }
}
