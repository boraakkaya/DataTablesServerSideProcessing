using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateCustomersJSONController : Controller
    {
        private readonly IConfiguration _config;
        public UpdateCustomersJSONController(IConfiguration configuration)
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

            List<Customer> updatedCustomersList = values.value;

            //start writing to wwwroot
            HttpClient vfsClient = new HttpClient();
            vfsClient.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue(
        "Basic",
        Convert.ToBase64String(
            System.Text.ASCIIEncoding.ASCII.GetBytes(
                string.Format("{0}:{1}", _config.GetValue<string>("VFSUsername"), _config.GetValue<string>("VFSPassword")))));

            vfsClient.DefaultRequestHeaders.Add("If-Match", "*");
            var vfsRequest = new HttpRequestMessage(new HttpMethod("PUT"), "https://datatablespocwebapp.scm.azurewebsites.net/api/vfs/LocalSiteRoot/VirtualDirectory0/site/wwwroot/wwwroot/Customers.json");

            vfsRequest.Content = new StringContent(JsonConvert.SerializeObject(values.value), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage vfsResponse = await vfsClient.SendAsync(vfsRequest);
            //To DO check vfsResponse Result

            return Json(values.value);
        }
    }
}
