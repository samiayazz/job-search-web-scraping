using Microsoft.AspNetCore.Mvc;
using WebAPI.Domain.Entities;
using WebAPI.Repositories;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        LinkedInRepository _linkedInRepository = new();
        KariyerNetRepository _kariyerNetRepository = new();

        [HttpGet(Name = "GetAllJobs")]
        public IEnumerable<Job> GetAll(string keyword, string location, int limit = 50)
        {
            var jobList = new List<Job>();

            jobList.AddRange(_linkedInRepository.GetJobs(keyword, location, limit));
            jobList.AddRange(_kariyerNetRepository.GetJobs(keyword, location, limit));

            return jobList;
        }

        [HttpGet("LinkedIn", Name = "GetLinkedInJobs")]
        public IEnumerable<Job> GetLinkedIn(string keyword, string location, int limit = 50)
            => _linkedInRepository.GetJobs(keyword, location, limit);

        [HttpGet("KariyerNet", Name = "GetKariyerNetJobs")]
        public IEnumerable<Job> GetKariyerNet(string keyword, string location, int limit = 50)
            => _kariyerNetRepository.GetJobs(keyword, location, limit);
    }
}
