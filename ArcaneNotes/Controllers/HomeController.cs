using ArcaneNotes.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using EntityLibrary.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace ArcaneNotes.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var client = _httpClientFactory.CreateClient("ArcaneApi");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync("api/WorkSpace/");

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }

            var workspaces = await response.Content.ReadFromJsonAsync<List<WorkSpace>>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || workspaces == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var model = new HomeViewModel(workspaces, userId);
            return View(model);
        }
        
        [HttpGet("workspace/{workSpace}")]
        public async Task<IActionResult> OpenWorkSpace(string workSpace)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var client = _httpClientFactory.CreateClient("ArcaneApi");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync($"api/WorkSpace/{workSpace}/");

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }

            var gotWorkSpace = await response.Content.ReadFromJsonAsync<WorkSpace>();
            //always start with the owners note first, we can change this in the future if we want
            GMFormViewModel model = new(gotWorkSpace, gotWorkSpace.OwnerId);
            return View("GMForm", model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorkSpace(HomeViewModel model)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var client = _httpClientFactory.CreateClient("ArcaneApi");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.PostAsJsonAsync("api/WorkSpace/", new { title = model.title, gmName = model.gmName });

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }

            var workSpace = await response.Content.ReadFromJsonAsync<WorkSpace>();
            return RedirectToAction("OpenWorkSpace", new { workSpace = workSpace.Id });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //--Delete workspace
        
        //Load Note from DB for page
        [HttpGet("Workspace/GetNoteData/{workPlaceId}/{userId}")]
        public async Task<IActionResult> GetNoteData(string workPlaceId, string userId)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var client = _httpClientFactory.CreateClient("ArcaneApi");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            var response = await client.GetAsync($"api/workspace/{workPlaceId}/{userId}/note");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return Content(json, "application/json"); 
            }

            return StatusCode((int)response.StatusCode);
        }
        
        //Add new collab
        [HttpPost("Workspace/AddCollaborator/{workPlaceId}/{email}")]
        public async Task<IActionResult> AddCollaborator(string workPlaceId, string email)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var client = _httpClientFactory.CreateClient("ArcaneApi");
    
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            
            var response = await client.PostAsync($"api/workspace/{workPlaceId}/{email}/collaborator", null);
    
            return StatusCode((int)response.StatusCode);
        }
        
        //Save your data
        [HttpPost("Workspace/Save/{workPlaceId}/{userId}")]
        public async Task<IActionResult> SaveWorkspace(string workPlaceId, string userId, [FromBody] JsonElement data)
        {
            var myUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (myUserId != userId)
                return Unauthorized(); // Return 401 Unauthorized instead of 400 BadRequest for auth failures

            var token = await HttpContext.GetTokenAsync("access_token");
            var client = _httpClientFactory.CreateClient("ArcaneApi");
    
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
    
            using var content = new StringContent(data.GetRawText(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"api/workspace/{workPlaceId}/note", content);
    
            return StatusCode((int)response.StatusCode);
        }
    }
}
