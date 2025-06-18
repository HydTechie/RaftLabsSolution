using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RaftLabs.Users.Infrastructure.Models;
using RaftLabs.Users.Infrastructure.Options;
using System.Net.Http.Json;
using RaftLabs.Users.Infrastructure;

namespace RaftLabs.Users.Infrastructure.Services
{
    public class ExternalUserApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExternalUserApiClient> _logger;
        private readonly string _baseUrl;
        private readonly IOptions<ApiOptions> _options;

        public ExternalUserApiClient(HttpClient httpClient, IOptions<ApiOptions> options, ILogger<ExternalUserApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _options = options;
             _baseUrl = options.Value.BaseUrl.TrimEnd('/');
        }

        public async Task<ApiUserDto?> GetUserByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetWithHeadersAsync($"{_baseUrl}/users/{id}", _options);
                if (response.IsSuccessStatusCode)
                {
                    var wrapper = await response.Content.ReadFromJsonAsync<ApiResponse<ApiUserDto>>();
                    return wrapper?.Data;
                }

                _logger.LogWarning("Failed to fetch user {UserId}: {StatusCode}", id, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user by ID: {UserId}", id);
                throw;
            }
        }

        public async Task<List<ApiUserDto>> GetAllUsersAsync()
        {
            var users = new List<ApiUserDto>();
            int page = 1;

            try
            {
                while (true)
                {
                    var response = await _httpClient.GetWithHeadersAsync($"{_baseUrl}/users?page={page}", _options);
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("Failed to fetch page {PageNumber}: {StatusCode}", page, response.StatusCode);
                        break;
                    }

                    var data = await response.Content.ReadFromJsonAsync<PaginatedResponse<ApiUserDto>>();
                    if (data == null || data.Data == null || data.Data.Count == 0)
                        break;

                    users.AddRange(data.Data);
                    if (page >= data.Total_Pages) break;

                    page++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users with pagination");
                throw;
            }

            return users;
        }

        private class ApiResponse<T>
        {
            public T Data { get; set; } = default!;
        }
    }
}
