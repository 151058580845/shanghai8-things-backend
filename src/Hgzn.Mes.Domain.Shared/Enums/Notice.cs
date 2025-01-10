using System.ComponentModel;

namespace Hgzn.Mes.Domain.Shared.Enums;

public enum Notice
{
}

/// <summary>
/// 通知显示
/// </summary>
public enum NoticeShow
{
    [Description("走马灯")] MerryGoRound = 0,
    [Description("提示弹窗")] Popup = 1
}

/// <summary>
/// 通知类型
/// </summary>
public enum NoticeType
{
    [Description("普通公告")] MerryGoRound = 0,

    [Description("物料告警")] Popup = 1
}

// <summary>
/// 通知时间类型
/// </summary>
public enum NoticeTimeType
{
    [Description("立即发送")] Now = 0,

    [Description("延时发送")] Delayed = 1,

    [Description("定时发送")] Scheduled = 2,

    [Description("循环发送")] Recurring = 3,

    [Description("触发发送")] Triggered = 4,

    [Description("自定义时间发送")] Custom = 5,

    [Description("工作日发送")] Workday = 6,

    [Description("预约时间发送")] Appointment = 7
}

/// <summary>
/// 优先级
/// </summary>
public enum Priority
{
    [Description("低优先级")] Low = 0, // 低优先级，表示相对不紧急的任务或通知
    [Description("普通优先级")] Normal = 1, // 普通优先级，表示默认的优先级
    [Description("高优先级")] High = 2, // 高优先级，表示重要且需要优先处理的任务或通知
    [Description("紧急优先级")] Critical = 3, // 紧急优先级，表示极为重要且必须尽快处理的任务或通知
    [Description("立即处理")] Immediate = 4 // 立即处理，表示需要马上进行处理的紧急任务或通知
}

/// <summary>
/// 通知状态
/// </summary>
/*
 *  草稿（Draft） → 审核中（Under Review） → 审核拒绝（Rejected）（如果未通过）或者 待发布（Pending Publish）（如果审核通过）。
    待发布（Pending Publish） → 到达设定的发布时间 → 已发布（Published）。
    已发布（Published） → 管理员决定撤销公告 → 已撤销（Revoked）。
    已发布（Published） → 公告有效期结束 → 过期（Expired）。
 */
public enum NoticeStatus
{
    [Description("草稿状态")] Draft = 0, // 草稿状态，表示通知尚未完成并保存为草稿
    [Description("审核中")] Review = 1, // 审核中，表示通知已完成编辑，正在等待审核
    [Description("已拒绝")] Rejected = 2, // 已拒绝，表示通知未通过审核
    [Description("待发布")] Pending = 3, // 待发布，表示通知审核通过，准备发布
    [Description("已发布")] Publish = 4, // 已发布，表示通知已成功发布
    [Description("已撤回")] Revoked = 5, // 已撤回，表示已发布的通知被撤回
    [Description("已过期")] Expired = 6 // 已过期，表示通知已超过有效期
}

/// <summary>
/// 通知目标
/// </summary>
public enum NoticeTargetType
{
    [Description("个人用户")] User = 0, // 个人用户
    [Description("角色")] Role = 1, // 角色，例如管理员、审核员
    [Description("部门")] Dept = 2, // 部门，例如人力资源部、财务部
    [Description("岗位")] Post = 3, // 岗位，例如经理、主管
    [Description("用户组")] Group = 4, // 用户组，例如特定项目组成员
    [Description("组织")] Organization = 5, // 组织，例如整个公司或企业
    [Description("团队")] Team = 6, // 团队，例如特定的工作团队
    [Description("全部用户")] AllUsers = 7, // 全部用户，向系统内所有用户发送通知
    [Description("自定义目标")] Custom = 8 // 自定义目标，可以根据特定条件选择用户或部门
}