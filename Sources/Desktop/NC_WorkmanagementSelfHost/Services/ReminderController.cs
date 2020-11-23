using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NC_WorkmanagementSelfHost.Models;

namespace NC_WorkmanagementSelfHost.Services
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReminderController : ControllerBase
    {
        public ReminderController()
        {

        }

        [HttpGet("ping")]
        public async Task<string> Ping()
        {
            return await Task.FromResult(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
        }

        [HttpPost("registerreminder")]
        public async Task<MessageReport> RegisterReminder([FromForm]RegisterModel model)
        {
            //Khai báo
            var result = new MessageReport()
            {
                isSuccess = false,
                Message = "error"
            };

            //Xử lý
            try
            {
                await ScheduleService.Register(model);

                result = new MessageReport()
                {
                    isSuccess = true,
                    Message = "done"
                };
            }
            catch (Exception ex)
            {
                result = new MessageReport()
                {
                    isSuccess = false,
                    Message = ex.Message
                };
            }

            //Trả về kết quả
            return await Task.FromResult(result);
        }

        [HttpPost]
        public async Task<MessageReport> RemoveRegisterReminder(RegisterModel model)
        {
            //Khai báo
            var result = new MessageReport()
            {
                isSuccess = false,
                Message = "error"
            };

            //Xử lý
            try
            {
                var re = await ScheduleService.RemoveJob(model);

                if (re)
                {
                    result = new MessageReport()
                    {
                        isSuccess = true,
                        Message = "done"
                    };
                }
                else
                {
                    result = new MessageReport()
                    {
                        isSuccess = false,
                        Message = "failed"
                    };
                }
            }
            catch (Exception ex)
            {
                result = new MessageReport()
                {
                    isSuccess = false,
                    Message = ex.Message
                };
            }

            //Trả về kết quả
            return await Task.FromResult(result);
        }
    }
}
