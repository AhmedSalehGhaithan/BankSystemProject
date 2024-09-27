using BlazorWebApp.Authentication.DTOs.Accounts.Request.Account;
using BlazorWebApp.Authentication.DTOs.Accounts.Response;
using BlazorWebApp.Authentication.DTOs.Accounts.Response.Account;
using BlazorWebApp.Authentication.DTOs.Extensions;
using BlazorWebApp.DTOs;
using System.Net.Http.Json;

namespace BlazorWebApp.Authentication.Service
{
    public class AccountService(HttpClientService _httpClientService) : IAccountService
    {
        private async Task<Responses> HandleResponseAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                return new Responses(false, GetErrorMessage(response));
            }
            return await response.Content.ReadFromJsonAsync<Responses>() ?? new Responses(false, "Unexpected response");
        }

        private static string GetErrorMessage(HttpResponseMessage response) =>
            $"Sorry, Unknown error occurred.{Environment.NewLine}Error Description : {Environment.NewLine}Status code : {response.StatusCode}{Environment.NewLine}Reason Phrase : {response.ReasonPhrase}";

        public async Task<Responses> ChangeUserRoleAsync(ChangeUserRoleRequest model) =>
            await HandleResponseAsync(await (await _httpClientService.GetPrivateClient()).PostAsJsonAsync(ConstantValues.ChangeUserRoleRoute, model));

        public async Task<Responses> RegisterAccountAsync(CreateAccount model) =>
            await HandleResponseAsync(await _httpClientService.GetPublicClient().PostAsJsonAsync(ConstantValues.RegisterRoute, model));

        public async Task CreateAdminAtFirstStart() =>
            await _httpClientService.GetPublicClient().PostAsync(ConstantValues.CreateAdminRoute, null);

        public async Task<Responses> CreateRoleAsync(CreateRole model) =>
            await HandleResponseAsync(await (await _httpClientService.GetPrivateClient()).PostAsJsonAsync(ConstantValues.CreateRoleRoute, model));

        public async Task<IEnumerable<GetRole>> GetRolesAsync() =>
            await HandleGetAsync<IEnumerable<GetRole>>(ConstantValues.GetRoleRoute);

        public async Task<IEnumerable<GetUsersWithRolesResponse>> GetUsersWithRolesAsync() =>
            await HandleGetAsync<IEnumerable<GetUsersWithRolesResponse>>(ConstantValues.GetUsersWithRoleRoute);

        private async Task<T> HandleGetAsync<T>(string route)
        {
            var privateClient = await _httpClientService.GetPrivateClient();
            var response = await privateClient.GetAsync(route);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(GetErrorMessage(response));
            }
            return await response.Content.ReadFromJsonAsync<T>() ?? throw new Exception("Unexpected response");
        }

        public async Task<LoginResponse> LoginAccountAsync(LoginModel model) =>
            await HandleLoginAsync(await _httpClientService.GetPublicClient().PostAsJsonAsync(ConstantValues.LoginRoute, model));

        public async Task<LoginResponse> RefreshTokenAsync(RefreshToken model) =>
            await HandleLoginAsync(await _httpClientService.GetPublicClient().PostAsJsonAsync(ConstantValues.RefreshTokenRoute, model));

        private async Task<LoginResponse> HandleLoginAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                return new LoginResponse(false, GetErrorMessage(response));
            }
            return await response.Content.ReadFromJsonAsync<LoginResponse>() ?? new LoginResponse(false, "Unexpected response");
        }

        public async Task<Responses> DeleteAccountAsync(string email) =>
            await HandleResponseAsync(await (await _httpClientService.GetPrivateClient()).DeleteAsync($"{ConstantValues.DeleteUserAccount}/{email}"));
    }
}
