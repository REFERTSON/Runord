using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Runord.Shared.Base;
using Runord.Shared.DTOs.Cluster;
using Runord.Shared.Interfaces.Services;

namespace Runord.Hub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ClustersController : ControllerBase
    {
        private readonly IClusterService _clusterService;

        public ClustersController(IClusterService clusterService)
        {
            _clusterService = clusterService;
        }

        [HttpGet]
        public async Task<ActionResult<Result<IEnumerable<ClusterDto>>>> GetClusters()
        {
            var result = await _clusterService.GetAllClustersAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Result<ClusterDto>>> GetClusterNode(Guid id)
        {
            var result = await _clusterService.GetClusterByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        // Добавление узла (если нужно)
        [HttpPost]
        public async Task<ActionResult<Result<ClusterDto>>> AddNode([FromBody] CreateClusterRequest request)
        {
            var result = await _clusterService.CreateClusterAsync(request);
            return CreatedAtAction(nameof(GetClusterNode), new { id = result.Data?.Id }, result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Result<bool>>> RemoveNode(Guid id)
        {
            var result = await _clusterService.DeleteClusterAsync(id);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }
    }
}
