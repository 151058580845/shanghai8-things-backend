using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Application.Main.Utilities;
using Hgzn.Mes.Domain.Entities.System.Code;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Utilities;
using Hgzn.Mes.Infrastructure.Utilities;

using Microsoft.Extensions.Logging;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Hgzn.Mes.Application.Main.Services.System;

/// <summary>
/// 编码规则
/// </summary>
public class CodeRuleService : SugarCrudAppService<
    CodeRule, Guid,
    CodeRuleReadDto, CodeRuleQueryDto,
    CodeRuleCreateDto, CodeRuleUpdateDto>,
    ICodeRuleService
{
    public CodeRuleService(
            ICodeRuleDefineService codeRuleDefineService,
            ILogger<UserService> logger
        )
    {
        _userDomainService = codeRuleDefineService;
    }


    private readonly ICodeRuleDefineService _userDomainService;

    public override async Task<PaginatedList<CodeRuleReadDto>> GetPaginatedListAsync(CodeRuleQueryDto input)
    {
        var entities = await Queryable
              .WhereIF(!string.IsNullOrEmpty(input.CodeName), x => x.CodeName.Contains(input.CodeName))
               .WhereIF(!string.IsNullOrEmpty(input.CodeNumber), x => x.CodeNumber.Contains(input.CodeNumber))
               .WhereIF(input.State != null, x => x.State == input.State)
            .OrderBy(x => x.OrderNum)
            .ToPaginatedListAsync(input.PageIndex, input.PageSize);
        return Mapper.Map<PaginatedList<CodeRuleReadDto>>(entities);
    }

    /// <summary>
    /// 返回对应的编码
    /// </summary>
    /// <param name="codeNumber"></param>
    /// <returns></returns>
    public async Task<string> GenerateCodeByCodeAsync(string codeNumber)
    {
        var code = await Queryable.FirstAsync(t => t.CodeNumber == codeNumber);

        if (code == null)
        {
            return "";
        }
        var codeRules = await DbContext.Queryable<CodeRuleDefine>()
            .Where(t => t.CodeRuleId == code.Id)
            .OrderBy(t => t.OrderNum)
            .ToListAsync();
        if (codeRules.Count < 1)
        {
            return "";
        }
        StringBuilder sb = new StringBuilder();
        try
        {
            foreach (var rule in codeRules)
            {
                rule.CodeRuleId = code.Id;
                switch (rule.CodeRuleType)
                {
                    case "SerialNumber":
                        // 如果为空设置 上一次的重置天数
                        if (string.IsNullOrEmpty(rule.SourceKey)  )
                        {
                            rule.SourceKey =DateTime.Now.ToString("yyyyMMdd");
                        }

                        bool isParsed = DateTime.TryParseExact(
                            rule.SourceKey,               // 待解析的日期字符串
                            "yyyyMMdd",               // 期望的日期格式
                            CultureInfo.InvariantCulture,  // 使用固定的文化信息，不依赖于系统的区域设置
                            DateTimeStyles.None,      // 不进行额外的日期处理
                            out DateTime ruleDate              // 输出的 DateTime 对象
                        );

                       // DateTime.TryParse(rule.SourceKey.ToString(), out DateTime ruleDate);
                        // 重置流水号
                        if (ruleDate.Day != DateTime.Now.Day)
                        {
                            rule.NowFlow = rule.InitialNowFlow;
                            rule.SourceKey = DateTime.Now.ToString("yyyyMMdd");
                        }

                        rule.NowFlow++;
                        sb.Append((rule.NowFlow).ToString()?.PadLeft((int)rule.MaxFlow!, (char)rule.CodeCover!));
                        rule.NowFlowIsSure = false;

                       
                        // 保存流水号
                        var saveCount= DbContext.Updateable<CodeRuleDefine>(rule).ExecuteCommand();
                        break;

                    case "Constant":
                        sb.Append(rule.ConstantChar);
                        break;

                    case "Date":
                        sb.Append(DateTime.Now.ToString(rule.DateFormat));
                        break;

                    case "AttributeValue":
                        break;
                }
            }
            return sb.ToString();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// 获取数据列表
    /// </summary>
    /// <param name="queryDto"></param>
    /// <returns></returns>
    public override async Task<IEnumerable<CodeRuleReadDto>> GetListAsync(CodeRuleQueryDto? queryDto)
    {
        var users = await Queryable
        .WhereIF(!string.IsNullOrEmpty(queryDto.CodeName), x => x.CodeName.Contains(queryDto.CodeName))
        .WhereIF(!string.IsNullOrEmpty(queryDto.CodeNumber), x => x.CodeNumber.Contains(queryDto.CodeNumber))
        .OrderBy(x => x.OrderNum)
        .ToArrayAsync();

        return Mapper.Map<IEnumerable<CodeRuleReadDto>>(users);
    }

    /// <summary>
    /// 获取实例
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public override async Task<CodeRuleReadDto?> GetAsync(Guid key)
    {
        //var user = await Queryable
        //         .Includes(u => u.CodeRuleRules)
        //           .Where(u => u.Id == key)
        //         .FirstAsync();

        var user = await DbContext.Queryable<CodeRule>()
        .Includes(x => x.CodeRuleRules)
        .Where(u => u.Id == key)
        .FirstAsync();

        return Mapper.Map<CodeRuleReadDto>(user);
    }

    /// <summary>
    /// 创建编码规则
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public override async Task<CodeRuleReadDto> CreateAsync(CodeRuleCreateDto dto)
    {
        var info = Mapper.Map<CodeRule>(dto);
        DbContext.InsertNav<CodeRule>(info)
            .Include(x => x.CodeRuleRules)
            .ExecuteCommand();

        return new CodeRuleReadDto();
    }

    /// <summary>
    /// 修改编码规则  
    /// </summary>
    /// <param name="key"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async override Task<CodeRuleReadDto?> UpdateAsync(Guid key, CodeRuleUpdateDto dto)
    {
        var codeRuleinfo = Mapper.Map<CodeRule>(dto);
        codeRuleinfo.Id = key;
        var data = DbContext.UpdateNav<CodeRule>(codeRuleinfo)
            .Include(x => x.CodeRuleRules)
            .ExecuteCommand();
        return new CodeRuleReadDto();
    }
}