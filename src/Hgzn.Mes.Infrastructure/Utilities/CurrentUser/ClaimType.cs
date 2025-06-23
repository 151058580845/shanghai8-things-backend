using System.Security.Claims;

namespace Hgzn.Mes.Infrastructure.Utilities.CurrentUser;

public static class ClaimType
{
    /// <summary>
    /// Default: <see cref="ClaimTypes.Name"/>
    /// </summary>
    public static string UserName { get; set; } = "username";

    /// <summary>
    /// 用户角色等级
    /// </summary>
    public static string Level { get; set; } = "level";

    /// <summary>
    /// Default: <see cref="ClaimTypes.GivenName"/>
    /// </summary>
    public static string Name { get; set; } = "name";

    /// <summary>
    /// Default: <see cref="ClaimTypes.Surname"/>
    /// </summary>
    public static string SurName { get; set; } = "surname";

    /// <summary>
    /// Default: <see cref="ClaimTypes.NameIdentifier"/>
    /// </summary>
    public static string UserId { get; set; } = "uid";

    /// <summary>
    /// Default: <see cref="ClaimTypes.Role"/>
    /// </summary>
    public static string RoleId { get; set; } = "rid";

    /// <summary>
    /// Default: <see cref="ClaimTypes.Email"/>
    /// </summary>
    public static string Email { get; set; } = "email";

    /// <summary>
    /// Default: "email_verified".
    /// </summary>
    public static string EmailVerified { get; set; } = "email_verified";

    /// <summary>
    /// Default: "phone_number".
    /// </summary>
    public static string PhoneNumber { get; set; } = "phone_number";

    /// <summary>
    /// Default: "phone_number_verified".
    /// </summary>
    public static string PhoneNumberVerified { get; set; } = "pnum_verif";

    /// <summary>
    /// Default: "tenantid".
    /// </summary>
    public static string TenantId { get; set; } = "tenantid";

    /// <summary>
    /// Default: "editionid".
    /// </summary>
    public static string EditionId { get; set; } = "editionid";

    /// <summary>
    /// Default: "client_id".
    /// </summary>
    public static string ClientId { get; set; } = "client_id";

    /// <summary>
    /// Default: "impersonator_tenantid".
    /// </summary>
    public static string ImpersonatorTenantId { get; set; } = "imp_tenantid";

    /// <summary>
    /// Default: "impersonator_userid".
    /// </summary>
    public static string ImpersonatorUserId { get; set; } = "imp_uid";

    /// <summary>
    /// Default: "impersonator_tenantname".
    /// </summary>
    public static string ImpersonatorTenantName { get; set; } = "imp_tenantname";

    /// <summary>
    /// Default: "impersonator_username".
    /// </summary>
    public static string ImpersonatorUserName { get; set; } = "imp_username";

    /// <summary>
    /// Default: "picture".
    /// </summary>
    public static string Picture { get; set; } = "picture";

    /// <summary>
    /// Default: "remember_me".
    /// </summary>
    public static string RememberMe { get; set; } = "remember_me";

    /// <summary>
    /// Default: "session_id".
    /// </summary>
    public static string SessionId { get; set; } = "session_id";
}