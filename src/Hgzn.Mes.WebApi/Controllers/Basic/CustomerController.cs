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
    /// 客户控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        public CustomerController(
        ICustomerService customerService, ISupplierService supplierService
        )
        {
            _supplierService = supplierService;
            _customerService = customerService;
        }

        private readonly ICustomerService  _customerService;
        private readonly ISupplierService _supplierService;

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"basic:customer:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<CustomerReadDto>> GetAsync(Guid id)
        {
            var readDto = await _customerService.GetAsync(id);
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
        [Authorize(Policy = $"basic:customer:{ScopeMethodType.Remove}")]
        public async Task<ResponseWrapper<int>> DeleteAsync(Guid id) =>
            (await _customerService.DeleteAsync(id)).Wrap();

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
        [Authorize(Policy = $"basic:customer:{ScopeMethodType.Edit}")]
        public async Task<ResponseWrapper<CustomerReadDto>> UpdateAsync(Guid id, CustomerUpdateDto input)
        {   
            if (input != null && input.ContactEntities.Any())
            {
                input.Contacts = new List<Domain.Entities.Basic.Contact>();
                input.ContactEntities.ForEach(a => {
                    input.Contacts.Add(new Domain.Entities.Basic.Contact()
                    {
                        ParentId = id,
                        Name = a.Name,
                            Phone = a.Phone,
                        Address = a.Address
                    });
                });
            }
            return (await _customerService.UpdateAsync(id, input)).Wrap();
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
        [Authorize(Policy = $"basic:customer:{ScopeMethodType.Query}")]
        public async Task<ResponseWrapper<PaginatedList<CustomerReadDto>>> GetPaginatedListAsync(CustomerQueryDto input) =>
            (await _customerService.GetPaginatedListAsync(input)).Wrap();

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Policy = $"basic:customer:{ScopeMethodType.Add}")]
        public async Task<ResponseWrapper<CustomerReadDto>> PostCreateAsync(CustomerCreateDto dto)
        {
            if (dto != null && dto.ContactEntities.Any())
            {
                dto.Contacts = new List<Domain.Entities.Basic.Contact>();
                dto.ContactEntities.ForEach(a => {
                    
                    dto.Contacts.Add(new Domain.Entities.Basic.Contact()
                    {
                        Name = a.Name,
                        Phone = a.Phone,
                        Address = a.Address,
                    });
                });
            }

            return (await _customerService.CreateAsync(dto)).Wrap();
        }
    }
}
