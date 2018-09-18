using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using DoacaoWebApi.Helpers;
using DoacaoWebApi.Models;

namespace DoacaoWebApi.Controllers
{
    [Authorize]
    public class BloodUsersController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public IHttpActionResult Post([FromBody] BloodUser bloodUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stream = new MemoryStream(bloodUser.ImageArray);
            var guid = Guid.NewGuid().ToString();
            var file = String.Format("{0}.jpg", guid);
            var folder = "~/Content/Users";
            var fullPath = String.Format("{0}/{1}", folder, file);
            var response = FilesHelper.UploadPhoto(stream, folder, file);

            if (response)
            {
                bloodUser.ImagePath = fullPath;
            }

            var user = new BloodUser()
            {
                UserName = bloodUser.UserName,
                BloodGroup = bloodUser.BloodGroup,
                Email = bloodUser.Email,
                Phone = bloodUser.Phone,
                City = bloodUser.City,
                State = bloodUser.State,
                Date = bloodUser.Date,
                ImagePath = bloodUser.ImagePath
            };

            db.BloodUsers.Add(user);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.Created);
        }

        public IEnumerable<BloodUser> Get(string bloodGroup, string city, string state)
        {
            var bloodUsers = db.BloodUsers.Where(user => user.BloodGroup.StartsWith(bloodGroup) && user.City.StartsWith(city) && user.State.StartsWith(state));
            return bloodUsers;
        }

        public IEnumerable<BloodUser> Get()
        {
            return db.BloodUsers.OrderByDescending(user => user.Date);
        }
    }
}