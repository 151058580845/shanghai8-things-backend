using Hgzn.Mes.Application.Main.Dtos.Basic;
using Hgzn.Mes.Application.Main.Services.Basic.IService;
using Hgzn.Mes.Domain.Entities.Basic;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Utilities;

namespace Hgzn.Mes.Application.Main.Services.Basic
{
    internal class CustomerService : SugarCrudAppService<
    Customer, Guid,
    CustomerReadDto, CustomerQueryDto,
    CustomerCreateDto, CustomerUpdateDto>,
    ICustomerService
    {
        public async override Task<IEnumerable<CustomerReadDto>> GetListAsync(CustomerQueryDto? queryDto = null)
        {
            var users = await Queryable
             .WhereIF(!string.IsNullOrEmpty(queryDto.Code), x => x.Code.Contains(queryDto.Code))
             .WhereIF(!string.IsNullOrEmpty(queryDto.Name), x => x.Name.Contains(queryDto.Name))
              .OrderBy(x => x.CreationTime)
              .ToArrayAsync();

            return Mapper.Map<IEnumerable<CustomerReadDto>>(users);
        }

        public async override Task<PaginatedList<CustomerReadDto>> GetPaginatedListAsync(CustomerQueryDto input)
        {
            var entities = await Queryable
            .WhereIF(!string.IsNullOrEmpty(input.Code), x => x.Code.Contains(input.Code))
             .WhereIF(!string.IsNullOrEmpty(input.Name), x => x.Name.Contains(input.Name))
              .OrderBy(x => x.CreationTime)
              .ToPaginatedListAsync(input.PageIndex, input.PageSize);
            return Mapper.Map<PaginatedList<CustomerReadDto>>(entities);
        }


        public async override Task<CustomerReadDto> CreateAsync(CustomerCreateDto dto)
        {
            var info = Mapper.Map<Customer>(dto);
            DbContext.InsertNav<Customer>(info)
                .Include(x => x.Contacts)
                .Include(x => x.AddressBs)
                .ExecuteCommand();

            return new CustomerReadDto();
        }

        /// <summary>
        /// 修改编码规则  
        /// </summary>
        /// <param name="key"></param>1
        /// <param name="dto"></param>
        /// <returns></returns>
        public async override Task<CustomerReadDto?> UpdateAsync(Guid key, CustomerUpdateDto dto)
        {
            var info = Mapper.Map<Customer>(dto);
            info.Id = key;
            var data = DbContext.UpdateNav<Customer>(info)
                .Include(x => x.Contacts)
                .Include(x => x.AddressBs)
                .ExecuteCommand();
            return new CustomerReadDto();
        }
    }
}
