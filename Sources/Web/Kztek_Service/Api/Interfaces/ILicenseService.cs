using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kztek_Model.Models.MN;

namespace Kztek_Service.Api.Interfaces
{
    public interface ILicenseService
    {
        Task<MN_License> License(string name);

        Task<List<MN_License>> GetAll();
    }
}
