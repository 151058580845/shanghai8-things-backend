using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.ValueObjects;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.Equip
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipMeasurementController : ControllerBase
    {
        private readonly IEquipMeasurementService _equipMeasurementService;
        public EquipMeasurementController(IEquipMeasurementService equipMeasurementService)
        {
            _equipMeasurementService = equipMeasurementService;
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="refresh"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes(contentType: "multipart/form-data")]
        public async Task<ResponseWrapper<bool?>> EquipMeasurementRefreshAsync() =>
             (await _equipMeasurementService.EquipMeasurementRefreshAsync(Request.Form.Files[0])).Wrap()!;

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ResponseWrapper<List<EquipMeasurementReadDto?>>> GetMeasurementDue() =>
            (await _equipMeasurementService.GetMeasurementDue()).Wrap()!;
    }
}
