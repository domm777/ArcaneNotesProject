using System.Security.Claims;
using ArcaneNotes.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace ArcaneNotes.Controllers;

public class LoginController : Controller {
    private readonly IHttpClientFactory _httpClientFactory;

    public LoginController(IHttpClientFactory httpClientFactory) {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public IActionResult Login() {
        if (User.Identity.IsAuthenticated)
            return RedirectToAction("Index", "Home");
        return View();
    }



    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model) {
        if (!ModelState.IsValid) return View(model);

        var client = _httpClientFactory.CreateClient("ArcaneApi");
        var response = await client.PostAsJsonAsync("api/auth/login", new { model.Email, model.Password });

        if (response.IsSuccessStatusCode) {
            var result = await response.Content.ReadFromJsonAsync<TokenResponse>();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, result.Name),
                new Claim(ClaimTypes.NameIdentifier, result.UserId),
                new Claim(ClaimTypes.Email, model.Email)
            };
            var authProperties = new AuthenticationProperties {
                IsPersistent = model.RememberMe
            };

            authProperties.StoreTokens(new[]
            {
                new AuthenticationToken { Name = "access_token", Value = result.Token }
            });

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }


    [HttpGet]
    public IActionResult Register() {
        if (User.Identity.IsAuthenticated)
            return RedirectToAction("Home", "Index");
        return View();
    }

    //[HttpPost]
    //public async Task<IActionResult> Register(RegisterViewModel model) {
    //    if (!ModelState.IsValid) return View(model);

    //    var client = _httpClientFactory.CreateClient("ArcaneApi");
    //    var response = await client.PostAsJsonAsync("api/auth/register", new { model.Email, model.Password });

    //    // 1. ADD THIS FOR DEBUGGING
    //    var rawContent = await response.Content.ReadAsStringAsync();

    //    if (response.IsSuccessStatusCode) {
    //        try {
    //            var result = await response.Content.ReadFromJsonAsync<TokenResponse>();

    //            if (result == null || string.IsNullOrEmpty(result.Token) || string.IsNullOrEmpty(result.UserId)) {
    //                Console.WriteLine($"DEBUG: API Success but unexpected structure: {rawContent}");
    //                ModelState.AddModelError(string.Empty, "Registration failed: Invalid response format from server.");
    //                return View(model);
    //            }
    //        } catch (Exception ex) {
    //            Console.WriteLine($"DEBUG: Exception during deserialization: {ex.Message}. Response was: {rawContent}");
    //            throw;
    //        }
    //    } else {
    //        Console.WriteLine($"DEBUG: API Error: {response.StatusCode}. Content: {rawContent}");
    //        ModelState.AddModelError(string.Empty, "Registration failed. Email may already exist.");
    //        return View(model);
    //    }
    //    return View(model);
    //}

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model) {
        if (!ModelState.IsValid) return View(model);

        var client = _httpClientFactory.CreateClient("ArcaneApi");
        var response = await client.PostAsJsonAsync("api/auth/register", new { model.Email, model.Password });

        if (response.IsSuccessStatusCode) {

            // Read as string first to inspect it
            var jsonString = await client.PostAsJsonAsync("api/auth/login", new { model.Email, model.Password });

            if (jsonString.IsSuccessStatusCode) {
                var result = await jsonString.Content.ReadFromJsonAsync<TokenResponse>();
                try {
                    //var result = System.Text.Json.JsonSerializer.Deserialize<TokenResponse>( jsonString,
                    //    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    //);

                    if (result == null || string.IsNullOrEmpty(result.Token)) {
                        // LOG THIS: The actual content received
                        System.Diagnostics.Debug.WriteLine($"API Response was: {jsonString}");
                        ModelState.AddModelError(string.Empty, "Registration failed: Server returned unexpected data.");
                        return View(model);
                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, result.Name ?? model.Email),
                        new Claim(ClaimTypes.NameIdentifier, result.UserId),
                        new Claim(ClaimTypes.Email, model.Email)
                    };
                    var authProperties = new AuthenticationProperties {
                        IsPersistent = false
                    };

                    authProperties.StoreTokens(new[]
                    {
                        new AuthenticationToken { Name = "access_token", Value = result.Token }
                    });

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties
                    );

                    return RedirectToAction("Index", "Home");

                } catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine($"Deserialization error: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "Registration failed: Could not parse server response.");
                    return View(model);
                }
            }
            return View(model);

        } else {
            ModelState.AddModelError(string.Empty, "Registration failed. Email may already exist.");
            return View(model);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Logout() {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}

public record TokenResponse(
    [property: JsonPropertyName("token")] string Token,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("userId")] string UserId);