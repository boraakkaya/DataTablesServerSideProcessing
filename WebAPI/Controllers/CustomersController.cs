using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : Controller
    {

        private readonly IConfiguration _config;
        public CustomersController(IConfiguration configuration)
        {
            _config = configuration;
        }

        // GET api/Customers
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            SPAccessTokenHelper tokenHelper = new SPAccessTokenHelper(_config);
            string spAccessToken = await tokenHelper.getSharePointAccessToken(_config.GetValue<string>("spSiteURL"));

            //Get list items using spAccessToken
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", spAccessToken);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            string requestUrl = "https://boraakkaya.sharepoint.com/_api/web/lists/getbytitle('HugeList')/items";
            var requestMethod = new HttpMethod("GET");
            var request = new HttpRequestMessage(requestMethod, requestUrl);
            HttpResponseMessage responseMessage = await client.SendAsync(request);

            SPRestAPIResponseValues values = new SPRestAPIResponseValues();
            values = await responseMessage.Content.ReadAsAsync<SPRestAPIResponseValues>();
                      
            return Json(values.value);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
