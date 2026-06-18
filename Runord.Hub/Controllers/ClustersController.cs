using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Runord.Shared.Base;
using Runord.Shared.DTOs.Cluster;
using Runord.Shared.Filters;
using Runord.Shared.Interfaces.Services;
using System.ComponentModel;

namespace Runord.Hub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    [Description("Управление кластерами (только для администраторов).")]
    public class ClustersController : ControllerBase
    {
        private readonly IClusterService _clusterService;

        public ClustersController(IClusterService clusterService)
        {
            _clusterService = clusterService;
        }

        [HttpGet]
        [EndpointSummary("Получить список кластеров")]
        [EndpointDescription("Возвращает список кластеров с возможностью фильтрации.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<IEnumerable<ClusterDto>>))]
        public async Task<ActionResult<Response<IEnumerable<ClusterDto>>>> GetClusters([FromQuery] ClusterFilter filter, CancellationToken ct)
        {
            var result = await _clusterService.GetClustersAsync(filter, ct);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [EndpointSummary("Получить кластер по ID")]
        [EndpointDescription("Возвращает详细信息 о кластере.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<ClusterDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response<ClusterDto>))]
        public async Task<ActionResult<Response<ClusterDto>>> GetCluster(Guid id, CancellationToken ct)
        {
            var result = await _clusterService.GetClusterByIdAsync(id, ct);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        [EndpointSummary("Создать новый кластер")]
        [EndpointDescription("Создает кластер с заданными параметрами.")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Response<ClusterDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Response<ClusterDto>>> CreateCluster([FromBody] CreateClusterRequest request, CancellationToken ct)
        {
            var result = await _clusterService.CreateClusterAsync(request, ct);
            return CreatedAtAction(nameof(GetCluster), new { id = result.Data?.Id }, result);
        }

        [HttpPut]
        [EndpointSummary("Обновить существующий кластер")]
        [EndpointDescription("Обновляет данные кластера.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<ClusterDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response<ClusterDto>))]
        public async Task<ActionResult<Response<ClusterDto>>> UpdateCluster([FromBody] UpdateClusterRequest request, CancellationToken ct)
        {
            var result = await _clusterService.UpdateClusterAsync(request, ct);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [EndpointSummary("Удалить кластер")]
        [EndpointDescription("Удаляет кластер по идентификатору.")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<bool>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response<bool>))]
        public async Task<ActionResult<Response<bool>>> DeleteCluster(Guid id, CancellationToken ct)
        {
            var result = await _clusterService.DeleteClusterAsync(id, ct);
            if (!result.IsSuccess) return NotFound(result);
            return Ok(result);
        }
    }
}