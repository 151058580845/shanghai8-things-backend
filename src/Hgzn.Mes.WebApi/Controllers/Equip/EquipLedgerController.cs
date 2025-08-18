using System.Collections;
using Hgzn.Mes.Application.Main.Dtos.App;
using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Application.Main.Services.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enum;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.Shared.Extensions;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using ScopeMethodType = Hgzn.Mes.Domain.ValueObjects.ScopeMethodType;

namespace Hgzn.Mes.WebApi.Controllers.Equip
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipLedgerController : ControllerBase
    {
        private readonly IEquipLedgerService _equipLedgerService;
        private readonly IRoomService _roomService;
        private readonly IDictionaryInfoService _dictionaryInfoService;
        private readonly ILocationLabelService _locationLabelService;
        private readonly IBaseConfigService _baseConfigService;
        public EquipLedgerController(
            IEquipLedgerService equipLedgerService,
            IRoomService roomService,
            IDictionaryInfoService dictionaryInfoService,
            ILocationLabelService locationLabelService,
            IBaseConfigService baseConfigService)
        {
            _equipLedgerService = equipLedgerService;
            _roomService = roomService;
            _dictionaryInfoService = dictionaryInfoService;
            _locationLabelService = locationLabelService;
            _baseConfigService = baseConfigService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("page")]
        public async Task<ResponseWrapper<PaginatedList<EquipLedgerReadDto>>> GetPaginatedListAsync(
            EquipLedgerQueryDto queryDto)
        {
            var levels = await _dictionaryInfoService.GetNameValueByTypeAsync("DeviceLevel");
            var status = await _dictionaryInfoService.GetNameValueByTypeAsync("DeviceStatus");
            var entities = await _equipLedgerService.GetPaginatedListAsync(queryDto);
            foreach (var entity in entities.Items)
            {
                entity.DeviceLevelString = levels?.FirstOrDefault(t => t.Value == entity.DeviceLevel)?.Name;
                entity.DeviceStatusString = status?.FirstOrDefault(t => t.Value == entity.DeviceStatus.ToString())?.Name;
            }
            return entities.Wrap();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("list")]
        public async Task<ResponseWrapper<IEnumerable<EquipLedgerReadDto>?>> GetListAsync(EquipLedgerQueryDto queryDto)
            => (await _equipLedgerService.GetListAsync(queryDto)).Wrap()!;

        /// <summary>
        /// 根据设备采集类型获取对应的列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("list/type/{type}")]
        public async Task<ResponseWrapper<IEnumerable<EquipLedgerReadDto>?>> GetListByTypeAsync(string? type)
            => (await _equipLedgerService.GetListByTypeAsync(type)).Wrap()!;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("namevaluelist")]
        public async Task<ResponseWrapper<IEnumerable<NameValueDto>>> GetNameValueListAsync()
        => (await _equipLedgerService.GetNameValueListAsync()).Wrap();


        /// <summary>
        /// 获取Rfid绑定设备列表
        /// </summary>
        /// <param name="equipId"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("RfidEquipsList")]
        public async Task<ResponseWrapper<IEnumerable<RfidEquipReadDto>>> GetRfidEquipsListAsync(Guid equipId)
        => (await _equipLedgerService.GetRfidEquipsListAsync(equipId)).Wrap();


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

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipledger:{ScopeMethodType.Add}")]
        public async Task<ResponseWrapper<EquipLedgerReadDto>> CreateAsync(EquipLedgerCreateDto input) =>
            (await _equipLedgerService.CreateAsync(input)).Wrap();

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipledger:{ScopeMethodType.Remove}")]
        public async Task<ResponseWrapper<int>> DeleteAsync(Guid id) =>
            (await _equipLedgerService.DeleteAsync(id)).Wrap();

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipledger:{ScopeMethodType.Edit}")]
        public async Task<ResponseWrapper<EquipLedgerReadDto>> UpdateAsync(Guid id, EquipLedgerUpdateDto input) =>
            (await _equipLedgerService.UpdateAsync(id, input)).Wrap();

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipledger:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<EquipLedgerReadDto>> GetAsync(Guid id) =>
            (await _equipLedgerService.GetAsync(id)).Wrap();

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("reponse-users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipledger:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<IEnumerable<EquipResponsibleUserReadDto>>> GetResponsibleUsersAsync() =>
            (await _equipLedgerService.GetEquipResponsibleUsersAsync()).Wrap()!;

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id:guid}/{state:bool}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipledger:{ScopeMethodType.Edit}")]
        public async Task<ResponseWrapper<EquipLedgerReadDto>> UpdateStateAsync(Guid id, bool state) =>
            (await _equipLedgerService.UpdateStateAsync(id, state)).Wrap();

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("import")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipledger:add")]
        public async Task<ResponseWrapper<bool?>> ImportAsync()
        {
            return (await _equipLedgerService.ImportAsync(Request.Form.Files[0])).Wrap()!;
        }

        /// <summary>
        /// Api批量导入
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("importByApi")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipledger:{ScopeMethodType.Add}")]
        public async Task<ResponseWrapper<int>> CreateAsync()
        {
            var url = await _baseConfigService.GetValueByKeyAsync("import_equip_url");
            return (await _equipLedgerService.PostImportDatas(url)).Wrap();
        }

        /// <summary>
        /// 获取测试数据
        /// </summary>
        /// <param name="apiUrl"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getTestData")]
        public List<EquipLedgerCreateDto> GetTestData(string apiUrl)
        {
            List<EquipLedgerCreateDto> equipLedgerCreates = new List<EquipLedgerCreateDto>();

            equipLedgerCreates.Add(new EquipLedgerCreateDto()
            {
                AssetNumber = "ces0",
                DeviceStatus = DeviceStatus.Normal.ToString(),
                EquipCode = "123",
                EquipName = "挖掘机",
                Model = "测试形",
                PurchaseDate = DateTime.Now.ToLocalTime(),
                Remark = "测试",
                RoomId = Guid.NewGuid(),
                TypeId = Guid.NewGuid(),
                ValidityDate = DateTime.Now.ToLocalTime()
            });
            return equipLedgerCreates;
        }



        [HttpPut]
        [Route("label")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ResponseWrapper<LocationLabelReadDto>> UpdateEquipLabel(Guid id, LocationLabelUpdateDto dto) =>
            (await _locationLabelService.UpdateAsync(id, dto)).Wrap();

        [HttpDelete]
        [Route("label/{id:guid}/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ResponseWrapper<int>> DeleteEquipLabel(Guid id) =>
            (await _locationLabelService.DeleteAsync(id)).Wrap();

        [HttpPost]
        [Route("labels/paged")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ResponseWrapper<PaginatedList<LocationLabelReadDto>>> GetEquipLabelaPaginatedList(LocationLabelQueryDto dto) =>
            (await _locationLabelService.GetPaginatedListAsync(dto)).Wrap();

        [HttpDelete]
        [Route("labels")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ResponseWrapper<int>> DeleteRanges(IEnumerable<Guid> ids) =>
            (await _locationLabelService.DeleteRangesAsync(ids)).Wrap();

        /// <summary>
        ///     获取指定设备的rfid绑定关系
        /// </summary>
        /// <param name="equipId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("id/{equipId:guid}/labels")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<ResponseWrapper<IEnumerable<EquipLocationLabelReadDto>>> FindEquipLabelAsync(Guid equipId) =>
            (await _locationLabelService.FindEquipLabelAsync(equipId)).Wrap();

        /// <summary>
        ///     获取指定设备类型的rfid绑定关系
        /// </summary>
        /// <param name="typeIds"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("equip-type/labels")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<ResponseWrapper<IEnumerable<LocationLabelReadDto>>> QueryByDeviceType(IEnumerable<Guid>? typeIds = null) =>
            (await _locationLabelService.QueryByDeviceTypes(typeIds)).Wrap();

        
        /// <summary>
        ///     获取所有rfid系统中设备的位置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("equip/export/rfid")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<ResponseWrapper<IEnumerable<EquipLedgerSearchReadDto>>> GetEquipExportRfid() =>
            (await _equipLedgerService.GetEquipExportRfid()).Wrap();
    }
}
