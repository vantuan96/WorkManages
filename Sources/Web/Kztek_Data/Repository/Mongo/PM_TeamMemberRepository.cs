using System;
using Kztek_Data.Infrastructure;
using Kztek_Model.Models.PM;

namespace Kztek_Data.Repository.Mongo
{
    public interface IPM_TeamMemberRepository : IMongoRepository<PM_TeamMember>
    {
    }

    public class PM_TeamMemberRepository : MongoRepository<PM_TeamMember>, IPM_TeamMemberRepository
    {

    }
}
