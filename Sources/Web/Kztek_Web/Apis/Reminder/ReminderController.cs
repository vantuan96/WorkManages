using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Library.Models;
using Kztek_Model.Models;
using Kztek_Service.Api.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kztek_Web.Apis.Reminder
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReminderController : ControllerBase
    {
        private IReminderService _ReminderService;

        public ReminderController(IReminderService _ReminderService)
        {
            this._ReminderService = _ReminderService;
        }

        [HttpPost("receiveschedulereminder")]
        public async Task<MessageReport> ReceiveScheduleReminder([FromBody] ReminderModel model)
        {
            return await _ReminderService.ReminderToClient(model);
        }

        [HttpGet("data")]
        public async Task<List<SY_ReminderSystem>> data()
        {
            return await _ReminderService.DataNeedReminder();
        }
    }
}