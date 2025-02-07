using System.Security.Claims;

namespace Hgzn.Mes.Infrastructure.Utilities.CurrentUser;

/// <summary>
/// 代表当前已认证用户的接口。
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// 获取一个值，指示用户是否已认证。
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// 获取用户的唯一标识符（ID）。
    /// </summary>
    Guid? Id { get; }

    /// <summary>
    /// 获取用户的用户名。
    /// </summary>
    string? UserName { get; }

    /// <summary>
    /// 获取用户的名字。
    /// </summary>
    string? Name { get; }

    /// <summary>
    /// 获取用户的姓氏。
    /// </summary>
    string? SurName { get; }

    /// <summary>
    /// 获取用户的电话号码。
    /// </summary>
    string? PhoneNumber { get; }

    /// <summary>
    /// 获取一个值，指示用户的电话号码是否已验证。
    /// </summary>
    bool PhoneNumberVerified { get; }

    /// <summary>
    /// 获取用户的电子邮件地址。
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// 获取一个值，指示用户的电子邮件是否已验证。
    /// </summary>
    bool EmailVerified { get; }

    /// <summary>
    /// 获取分配给用户的所有角色。
    /// </summary>
    string[] Roles { get; }

    /// <summary>
    /// 根据指定的声明类型查找单个声明。
    /// </summary>
    /// <param name="claimType">声明的类型。</param>
    /// <returns>匹配指定类型的声明，如果没有找到则返回 null。</returns>
    Claim? FindClaim(string claimType);

    /// <summary>
    /// 根据指定的声明类型查找所有相关的声明。
    /// </summary>
    /// <param name="claimType">声明的类型。</param>
    /// <returns>匹配指定类型的所有声明。</returns>
    Claim[] FindClaims(string claimType);

    /// <summary>
    /// 获取当前用户的所有声明。
    /// </summary>
    /// <returns>当前用户的所有声明。</returns>
    Claim[] GetAllClaims();

    /// <summary>
    /// 检查用户是否属于指定的角色。
    /// </summary>
    /// <param name="roleName">要检查的角色名称。</param>
    /// <returns>如果用户属于指定角色，则返回 true；否则返回 false。</returns>
    bool IsInRole(string roleName);
}
