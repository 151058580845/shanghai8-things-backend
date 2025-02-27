using Hgzn.Mes.Application.Main.Dtos.Basic;
using Hgzn.Mes.Application.Main.Services.Basic.IService;
using Hgzn.Mes.Domain.Entities.Basic;
using Hgzn.Mes.Domain.Entities.System.Code;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Utilities;


namespace Hgzn.Mes.Application.Main.Services.Basic
{
    /// <summary>
    /// 供应商服务
    /// </summary>
    public class SupplierService : SugarCrudAppService<
    Supplier, Guid,
    SupplierReadDto, SupplierQueryDto,
    SupplierCreateDto, SupplierUpdateDto>,
    ISupplierService
    {
        public async override Task<IEnumerable<SupplierReadDto>> GetListAsync(SupplierQueryDto queryDto )
        {
            var users = await Queryable
            .WhereIF(!string.IsNullOrEmpty(queryDto.Code), x => x.Code.Contains(queryDto.Code))
            .WhereIF(!string.IsNullOrEmpty(queryDto.Name), x => x.Name.Contains(queryDto.Name))
             .OrderBy(x => x.CreationTime)
             .ToArrayAsync();

            return Mapper.Map<IEnumerable<SupplierReadDto>>(users);
        }

        public async override Task<PaginatedList<SupplierReadDto>> GetPaginatedListAsync(SupplierQueryDto input)
        {
            var entities = await Queryable
           .WhereIF(!string.IsNullOrEmpty(input.Code), x => x.Code.Contains(input.Code))
            .WhereIF(!string.IsNullOrEmpty(input.Name), x => x.Name.Contains(input.Name))
             .OrderBy(x => x.CreationTime)
             .ToPaginatedListAsync(input.PageIndex, input.PageSize);
            return Mapper.Map<PaginatedList<SupplierReadDto>>(entities);
        }

        public async override Task<SupplierReadDto> CreateAsync(SupplierCreateDto dto)
        {
            var info = Mapper.Map<Supplier>(dto);
            DbContext.InsertNav<Supplier>(info)
                .Include(x => x.Contacts)
                .Include(x => x.AddressBs)
                .ExecuteCommand();

            return new SupplierReadDto();
        }

        /// <summary>
        /// 修改编码规则  
        /// </summary>
        /// <param name="key"></param>1
        /// <param name="dto"></param>
        /// <returns></returns>
        public async override Task<SupplierReadDto?> UpdateAsync(Guid key, SupplierUpdateDto dto)
        {
            var info = Mapper.Map<Supplier>(dto);
            info.Id = key;
            var data = DbContext.UpdateNav<Supplier>(info)
                .Include(x => x.Contacts)
                .Include(x => x.AddressBs)
                .ExecuteCommand();
            return new SupplierReadDto();
        }

        public async Task<List<SupplierDetilDto>> GetGetSupplierDetilInfos(Guid id) {
            var contactInfos = await DbContext.Queryable<Contact>()
           .Where(u => u.ParentId == id)
           .ToListAsync();

            List<SupplierDetilDto> supplierDetilDtos = new List<SupplierDetilDto>();
            foreach (var item in contactInfos)
            {
                supplierDetilDtos.Add(new SupplierDetilDto
                {
                     OrderNum= supplierDetilDtos.Count()+1,
                    Name = item.Name,
                    Phone = item.Phone,
                    Address = item.Address
                });
            }
            return supplierDetilDtos;
        }

    }
}
