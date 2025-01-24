using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Hgzn.Mes.WebApi.Controllers.Equip
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipLedgerController : ControllerBase
    {
        private readonly IEquipLedgerService _equipLedgerService;
        public EquipLedgerController(IEquipLedgerService equipLedgerService)
        {
            _equipLedgerService = equipLedgerService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("list")]
        public async Task<ResponseWrapper<PaginatedList<EquipLedgerReadDto>>> GetPaginatedListAsync(EquipLedgerQueryDto queryDto)
        => (await _equipLedgerService.GetPaginatedListAsync(queryDto)).Wrap();


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("NameValueList")]
        public async Task<ResponseWrapper<IEnumerable<NameValueDto>>> GetNameValueListAsync()
        => (await _equipLedgerService.GetNameValueListAsync()).Wrap()!;


        /// <summary>
        /// 获取Rfid绑定设备列表
        /// </summary>
        /// <param name="equipId"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("RfidEquipsList")]
        public async Task<ResponseWrapper<IEnumerable<RfidEquipDto>>> GetRfidEquipsListAsync(Guid equipId)
        => (await _equipLedgerService.GetRfidEquipsListAsync(equipId)).Wrap()!;


        ///// <summary>
        ///// 获取Rfid绑定设备列表
        ///// </summary>
        ///// <param name="equipId"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[Route("ImportEquipRooms")]
        //public async Task GetImportEquipRoomsAsync(EquipLedgerImportRoomInputVo data)
        //=> (await _equipLedgerService.GetImportEquipRoomsAsync(data)).Wrap();
    }
}
