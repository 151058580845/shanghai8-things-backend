using AutoMapper;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles.Equip
{
    public class TestDataDtoProfile : Profile
    {
        public TestDataDtoProfile()
        {
            CreateMap<TestData, TestDataReadDto>().ForMember(d => d.ProductInfo, opt => opt.MapFrom(x => GetProductInfo(x)));
            CreateMap<TestDataCreateDto, TestData>();
        }

        public string GetProductInfo(TestData data)
        {
            StringBuilder rst = new StringBuilder();
            if (data == null || data.UUT == null || !data.UUT.Any()) return rst.ToString();
            rst.Append("{");
            for (int i = 0; i < data.UUT.Count(); i++)
            {
                rst.Append("{");
                rst.Append($"{data.UUT[i].Name},{data.UUT[i].Code},{data.UUT[i].TechnicalStatus}");
                if (i == data.UUT.Count() - 1)
                    rst.Append("}");
                else
                    rst.Append("};");
            }
            rst.Append("}");
            return rst.ToString();
        }
    }
}
