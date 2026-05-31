using ApiBench.Web.Models;

namespace ApiBench.Web.Services;

public class EnvironmentService
{
    private readonly IndexedDbService _indexedDbService;

    public EnvironmentService(IndexedDbService indexedDbService)
    {
        _indexedDbService = indexedDbService;
    }

    public async Task<List<ApiEnvironment>> GetEnvironmentsAsync()
    {
        var envs = await _indexedDbService.GetAllAsync<ApiEnvironment>("environments");
        return envs.ToList();
    }

    public async Task SaveEnvironmentAsync(ApiEnvironment env)
    {
        await _indexedDbService.PutAsync("environments", env);
    }

    public async Task DeleteEnvironmentAsync(Guid id)
    {
        await _indexedDbService.DeleteAsync("environments", id.ToString());
    }

    public async Task<ApiEnvironment?> GetActiveEnvironmentAsync()
    {
        var envs = await GetEnvironmentsAsync();
        return envs.FirstOrDefault(e => e.IsActive);
    }

    public async Task SetActiveEnvironmentAsync(Guid id)
    {
        var envs = await GetEnvironmentsAsync();
        foreach (var env in envs)
        {
            env.IsActive = env.Id == id;
            await _indexedDbService.PutAsync("environments", env);
        }
    }

    public string ResolveVariables(string input, ApiEnvironment? env)
    {
        if (string.IsNullOrEmpty(input) || env == null)
            return input;

        foreach (var variable in env.Variables.Where(v => v.Enabled && !string.IsNullOrEmpty(v.Key)))
        {
            input = input.Replace($"{{{{{variable.Key}}}}}", variable.Value);
        }

        return input;
    }
}
