using Hgzn.Mes.Application.Main.Dtos.Basic;
using Hgzn.Mes.Application.Main.Services.Basic.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.ValueObjects;
using Hgzn.Mes.WebApi.Utilities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.Basic
{
    /// <summary>
    /// 供应商控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        public SupplierController(
          ISupplierService supplierService
          )
        {
            _supplierService = supplierService;
        }

        private readonly ISupplierService  _supplierService;

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"basic:supplier:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<SupplierReadDto>> GetAsync(Guid id) {
            SupplierReadDto readDto=await _supplierService.GetAsync(id);
            var detilData = await _supplierService.GetGetSupplierDetilInfos(id);
            readDto.ContactEntities = detilData;
            return readDto.Wrap();
        }
            

        /// <summary>
        ///     删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"basic:supplier:{ScopeMethodType.Remove}")]
        public async Task<ResponseWrapper<int>> DeleteAsync(Guid id) =>
            (await _supplierService.DeleteAsync(id)).Wrap();

        /// <summary>
        ///     更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"basic:supplier:{ScopeMethodType.Edit}")]
        public async Task<ResponseWrapper<SupplierReadDto>> UpdateAsync(Guid id, SupplierUpdateDto input)
        {
            if (input!=null && input.ContactEntities.Any()) {
                input.ContactEntities.ForEach(a => {
                    input.Contacts.Add(new Domain.Entities.Basic.Contact() { 
                         ParentId=id,
                         Name=a.Name,
                         Phone=a.Phone,
                         Address=a.Address
                    });
                });
            }
            return (await _supplierService.UpdateAsync(id, input)).Wrap();
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"basic:supplier:{ScopeMethodType.Add}")]
        public async Task<ResponseWrapper<SupplierReadDto>> PostCreateAsync(SupplierCreateDto dto) {
            if (dto != null && dto.ContactEntities.Any())
            {
                dto.ContactEntities.ForEach(a => {
                    //dto.AddressBs.Add(new Domain.Entities.Basic.AddressB()
                    //{
                    //    Address = a.Address,
                    //});
                    dto.Contacts.Add(new Domain.Entities.Basic.Contact()
                    {
                        Name = a.Name,
                        Phone = a.Phone,
                        Address = a.Address,
                    });
                });
            }

            return (await _supplierService.CreateAsync(dto)).Wrap();
        }
        

        /// <summary>
        ///     分页查询
        /// </summary>
        /// <param name="input">用于验证用户身份</param>
        /// <returns>更换密码状态</returns>
        [HttpPost]
        [Route("page")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"basic:supplier:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<PaginatedList<SupplierReadDto>>> GetPaginatedListAsync(SupplierQueryDto input) =>
            (await _supplierService.GetPaginatedListAsync(input)).Wrap();

        ///// <summary>
        ///// 查询子项信息的接口
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("{id:guid}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<ResponseWrapper<List<SupplierDetilDto>>> GetSupplierDetilInfo(Guid id) {
        //    return (await _supplierService.GetGetSupplierDetilInfos(id)).Wrap();
        //}
    }
}
