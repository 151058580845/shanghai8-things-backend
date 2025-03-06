using System.Collections;
using Hgzn.Mes.Application.Main.Dtos.App;
using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Application.Main.Services.System.IService;
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
        public EquipLedgerController(
            IEquipLedgerService equipLedgerService,
            IRoomService roomService,
            IDictionaryInfoService dictionaryInfoService,
            ILocationLabelService locationLabelService)
        {
            _equipLedgerService = equipLedgerService;
            _roomService = roomService;
            _dictionaryInfoService = dictionaryInfoService;
            _locationLabelService = locationLabelService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("page")]
        public async Task<ResponseWrapper<PaginatedList<EquipLedgerReadDto>>> GetPaginatedListAsync(EquipLedgerQueryDto queryDto)
        => (await _equipLedgerService.GetPaginatedListAsync(queryDto)).Wrap();

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("list")]
        public async Task<ResponseWrapper<IEnumerable<EquipLedgerReadDto>?>> GetListAsync(EquipLedgerQueryDto queryDto)
            => (await _equipLedgerService.GetListAsync(queryDto)).Wrap()!;

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
        /// <param name="id"></param>
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
        /// 获取试验系统列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("test/list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<ResponseWrapper<EquipLedgerTestReadDto>> GetTestListAsync()
        {
            var testList = await _dictionaryInfoService.GetNameValueByTypeAsync("TestSystem");
            var testRead = new EquipLedgerTestReadDto();
            var list = new List<TestListReadDto>();
            foreach (var test in testList)
            {
                list.Add(new TestListReadDto()
                {
                    TestName = test.Value
                });
            }
            foreach (var entity in list)
            {
                var rooms = await _roomService.GetRoomListByTestName(entity.TestName);
                var ids = await rooms.Select(x => x.Id).ToListAsync();
                var equips = (await _equipLedgerService.GetEquipsListByRoomAsync(ids)).ToList();
                if (equips.Count != 0)
                {
                    entity.EquipCount = equips.Count(t => t.DeviceStatus == DeviceStatus.Normal);
                    entity.Rate = equips.Count(t => t.DeviceStatus == DeviceStatus.Normal);
                    entity.HealthRate =  equips.Count(t => t.DeviceStatus == DeviceStatus.Normal);
                }
            }
            
            testRead.TestList = list;
            testRead.TestErrorList = new List<TestErrorListReadDto>();
            testRead.UpRate = 0;
            testRead.DownRate = 0;
            testRead.NormalCount = 0;
            testRead.FreeCount = 0;
            testRead.LeaveCount = 0;
            testRead.HealthCount = 0;
            testRead.BetterCount = 0;
            testRead.ErrorCount = 0;
            return testRead.Wrap();
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("import")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[Authorize(Policy = $"equip:equipledger:{ScopeMethodType.Add}")]
        public async Task<ResponseWrapper<int>> CreateAsync(IEnumerable<EquipLedgerCreateDto> inputs)
        {
            var addCount = 0;
            foreach (var item in inputs)
            {
                if (await _equipLedgerService.CreateAsync(item) != null) addCount += 1;
            }

            return addCount.Wrap();
        }


        /// <summary>
        /// Api批量导入
        /// </summary>
        /// <param name="apiUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("importByApi")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"equip:equipledger:{ScopeMethodType.Add}")]
        public async Task<ResponseWrapper<int>> CreateAsync(string apiUrl) =>
             (await _equipLedgerService.PostImportDatas(apiUrl)).Wrap();

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
                PurchaseDate = DateTime.Now,
                Remark = "测试",
                RoomId = Guid.NewGuid(),
                TypeId = Guid.NewGuid(),
                ValidityDate = DateTime.Now
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
        public async Task<ResponseWrapper<int>> DeleteEquipLabel(Guid id, LocationLabelUpdateDto dto) =>
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
           
    }
}
