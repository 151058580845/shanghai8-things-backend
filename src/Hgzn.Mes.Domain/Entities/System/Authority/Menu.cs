using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Shared.Enums;

namespace Hgzn.Mes.Domain.Entities.System.Authority
{
    public class Menu : UniversalEntity, ISoftDelete, IAudited, IOrder, IState
    {
        public string Name { get; set; } = null!;

        public string? Code { get; set; }

        public string? Description { get; set; }

        public Guid? ParentId { get; set; }

        public MenuType Type { get; set; }

        public int OrderNum { get; set; } = -1;
        
        public string? IconUrl { get; set; }

        public bool IsLink { get; set; }

        public string? Component { get; set; }

        public string? Route { get; set; }

        public string? RouteName { get; set; }

        public string? ScopeCode { get; set; }

        public bool Visible { get; set; } = true;

        public bool IsCache { get; set; } = false;

        public bool Favorite { get; set; } = false;

        public bool State { get; set; }


        #region audit

        public Guid? CreatorId { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid? LastModifierId { get; set; }

        public DateTime? LastModificationTime { get; set; }

        #endregion audit

        #region delete filter

        public bool SoftDeleted { get; set; } = false;

        public DateTime? DeleteTime { get; set; } = null;

        #endregion delete filter

        #region navigation

        public Menu? Parent { get; set; }

        #endregion navigation

        #region static

        public static Menu Root = new()
        {
            Id = new Guid("eb109b69-68b0-4a7f-826a-2178e9292741"),
            Name = "Root",
            Code = "root",
            Description = "root menu",
            Type = MenuType.Catalogue
        };

        public static Menu System = new()
        {
            Id = new Guid("e3a57edf-0670-4657-af10-44c4620c1012"),
            Name = "系统管理",
            Code = "system",
            Description = "system menu",
            Type = MenuType.Catalogue,
            Route = "/system",
            IconUrl = "ri:settings-3-line",
            OrderNum = 98,
            ParentId = Root.Id
        };

        public static Menu Monitoring = new()
        {
            Id = new Guid("044808f5-afe5-42b4-9720-5c00900538ca"),
            Name = "系统监控",
            Code = "monitoring",
            Description = "monitoring menu",
            Type = MenuType.Catalogue,
            Route = "/monitor",
            IconUrl = "ep:monitor",
            OrderNum = 99,
            ParentId = Root.Id
        };

        // 在线用户
        public static Menu Online = new()
        {
            Id = Guid.Parse("31b9f2b2-48f4-4eb8-bf3d-f1c5a53236cd"),
            Name = "在线用户",
            Code = "online",
            ScopeCode = "system:onlinehub:list",
            Type = MenuType.Menu,
            Route = "monitor/online/index",
            IconUrl = "ri:user-voice-line",
            OrderNum = 100,
            RouteName = "OnlineUser",
            ParentId = Monitoring.Id
        };

        // 用户管理
        public static Menu User = new()
        {
            Id = Guid.Parse("a430cf74-865e-4329-b6f2-c365a8fcb4b9"),
            Name = "用户管理",
            Code = "user",
            ScopeCode = "system:user:list",
            Type = MenuType.Menu,
            Route = "/system/user/index",
            IconUrl = "ri:admin-line",
            OrderNum = 100,
            ParentId = System.Id,
            RouteName = "SystemUser"
        };

        public static Menu UserQuery = new()
        {
            Id = Guid.Parse("fb9034d3-bd8f-44a3-b1c5-b44d2116fc17"),
            Name = "用户查询",
            Code = "userQuery",
            ScopeCode = "system:user:query",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = User.Id
        };

        public static Menu UserAdd = new()
        {
            Id = Guid.Parse("4d0a67ae-b1d5-49a6-8978-c9cc0d3a0a2c"),
            Name = "用户新增",
            Code = "userAdd",
            ScopeCode = "system:user:add",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = User.Id
        };

        public static Menu UserEdit = new()
        {
            Id = Guid.Parse("f1785827-f4c0-4a67-8b0e-03b53e5b9a92"),
            Name = "用户修改",
            Code = "userEdit",
            ScopeCode = "system:user:edit",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = User.Id
        };

        public static Menu UserRemove = new()
        {
            Id = Guid.Parse("0a8f9837-dde1-4c64-bb8b-10d22c4c5631"),
            Name = "用户删除",
            Code = "userRemove",
            ScopeCode = "system:user:remove",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = User.Id
        };

        public static Menu UserResetPwd = new()
        {
            Id = Guid.Parse("b342c51b-3d22-4c6f-91bc-9a734cda42b9"),
            Name = "重置密码",
            Code = "userResetPwd",
            ScopeCode = "system:user:resetPwd",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = User.Id
        };

        // 角色管理
        public static Menu Role = new()
        {
            Id = Guid.Parse("ab27c36e-bc06-46e9-b8ba-7a527924eb5e"),
            Name = "角色管理",
            Code = "role",
            ScopeCode = "system:role:list",
            Type = MenuType.Menu,
            Route = "/system/role/index",
            IconUrl = "ri:admin-fill",
            OrderNum = 99,
            ParentId = System.Id,
            RouteName = "SystemRole"
        };

        public static Menu RoleQuery = new()
        {
            Id = Guid.Parse("26f1b4b4-9d1e-4385-bb6e-b3808c01b245"),
            Name = "角色查询",
            Code = "roleQuery",
            ScopeCode = "system:role:query",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = Role.Id
        };

        public static Menu RoleAdd = new()
        {
            Id = Guid.Parse("7db1f33d-c0f6-466f-a63f-d58b944f1a44"),
            Name = "角色新增",
            Code = "roleAdd",
            ScopeCode = "system:role:add",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = Role.Id
        };

        public static Menu RoleEdit = new()
        {
            Id = Guid.Parse("63dcb957-52fc-438f-9351-2c9c6e097a2c"),
            Name = "角色修改",
            Code = "roleEdit",
            ScopeCode = "system:role:edit",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = Role.Id
        };

        public static Menu RoleRemove = new()
        {
            Id = Guid.Parse("2060d32a-7b65-42c0-b9ed-5c105b96b18e"),
            Name = "角色删除",
            Code = "roleRemove",
            ScopeCode = "system:role:remove",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = Role.Id
        };

        // 菜单管理
        public static Menu MenuRoot = new()
        {
            Id = Guid.Parse("b2f6f618-548a-4b0a-8e7b-1f24c3275dff"),
            Name = "菜单管理",
            Code = "menu",
            ScopeCode = "system:menu:list",
            Type = MenuType.Menu,
            Route = "/system/menu/index",
            IconUrl = "ep:menu",
            OrderNum = 98,
            ParentId = System.Id,
            RouteName = "SystemMenu"
        };

        public static Menu MenuQuery = new()
        {
            Id = Guid.Parse("17e35b7d-0f76-4635-b39b-222b8a7e0ea4"),
            Name = "菜单查询",
            Code = "menuQuery",
            ScopeCode = "system:menu:query",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = MenuRoot.Id
        };

        public static Menu MenuAdd = new()
        {
            Id = Guid.Parse("5a8f4d13-2e73-4c27-bd1b-8a5c19f93ef6"),
            Name = "菜单新增",
            Code = "menuAdd",
            ScopeCode = "system:menu:add",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = MenuRoot.Id
        };

        public static Menu MenuEdit = new()
        {
            Id = Guid.Parse("7e96f8a4-b2be-4e5d-bc73-91e3812f63b7"),
            Name = "菜单修改",
            Code = "menuEdit",
            ScopeCode = "system:menu:edit",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = MenuRoot.Id
        };

        public static Menu MenuRemove = new()
        {
            Id = Guid.Parse("a7c56b6c-d3a7-4a0d-bf4c-e06fd63165d3"),
            Name = "菜单删除",
            Code = "menuRemove",
            ScopeCode = "system:menu:remove",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = MenuRoot.Id
        };

        // 部门管理
        public static Menu Dept = new()
        {
            Id = Guid.Parse("d537cc7f-0c35-4d9d-8039-126698a1d1f6"),
            Name = "部门管理",
            Code = "dept",
            ScopeCode = "system:department:list",
            Type = MenuType.Menu,
            Route = "/system/dept/index",
            IconUrl = "ri:git-branch-line",
            OrderNum = 97,
            RouteName = "SystemDept",
            ParentId = System.Id
        };

        public static Menu DeptQuery = new()
        {
            Id = Guid.Parse("3a8575b9-0c2d-4cd1-a129-6495c3e86f74"),
            Name = "部门查询",
            Code = "deptQuery",
            ScopeCode = "system:department:query",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = Dept.Id
        };

        public static Menu DeptAdd = new()
        {
            Id = Guid.Parse("8c3c7c38-6795-4c7d-8b26-c4c34f464307"),
            Name = "部门新增",
            Code = "deptAdd",
            ScopeCode = "system:department:add",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = Dept.Id
        };

        public static Menu DeptEdit = new()
        {
            Id = Guid.Parse("d0f234b0-fcc1-4f86-b57f-8b598d163118"),
            Name = "部门修改",
            Code = "deptEdit",
            ScopeCode = "system:department:edit",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = Dept.Id
        };

        public static Menu DeptRemove = new()
        {
            Id = Guid.Parse("b2709f13-9351-4d79-bab0-5d78c57260fc"),
            Name = "部门删除",
            Code = "deptRemove",
            ScopeCode = "system:department:remove",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = Dept.Id
        };

        // 岗位管理
        public static Menu Post = new()
        {
            Id = Guid.Parse("3fbb302d-6018-48a3-8506-cbd53a26475e"),
            Name = "岗位管理",
            Code = "post",
            ScopeCode = "system:post:list",
            Type = MenuType.Menu,
            Route = "/system/post/index",
            IconUrl = "ant-design:deployment-unit-outlined",
            OrderNum = 96,
            ParentId = System.Id,
            RouteName = "SystemPost"
        };

        public static Menu PostQuery = new()
        {
            Id = Guid.Parse("d8cf5b7d-53fd-4b0d-8a1e-c9c9a5d4c56b"),
            Name = "岗位查询",
            Code = "postQuery",
            ScopeCode = "system:post:query",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = Post.Id
        };

        public static Menu PostAdd = new()
        {
            Id = Guid.Parse("7a9e1a58-77f9-43f1-bc2f-b6ff4309efdb"),
            Name = "岗位新增",
            Code = "postAdd",
            ScopeCode = "system:post:add",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = Post.Id
        };

        public static Menu PostEdit = new()
        {
            Id = Guid.Parse("179adad5-56a0-41d9-bbc7-d10dfe4b681b"),
            Name = "岗位修改",
            Code = "postEdit",
            ScopeCode = "system:post:edit",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = Post.Id
        };

        public static Menu PostRemove = new()
        {
            Id = Guid.Parse("7b31d126-5b78-47b3-8259-feb5b6a76c7b"),
            Name = "岗位删除",
            Code = "postRemove",
            ScopeCode = "system:post:remove",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = Post.Id
        };

        // 操作日志
        public static Menu OperationLog = new()
        {
            Id = Guid.Parse("6d0362a1-d89d-4973-a592-75e949abf28d"),
            Name = "操作日志",
            Code = "operationLog",
            ScopeCode = "audit:operatorlog:list",
            Type = MenuType.Menu,
            Route = "/monitor/operation-logs",
            IconUrl = "ri:history-fill",
            OrderNum = 100,
            ParentId = Monitoring.Id,
            RouteName = "OperationLog",
            Component = "monitor/logs/operation/index"
        };

        public static Menu OperationLogQuery = new()
        {
            Id = Guid.Parse("b7c36c6b-f8d7-4f8b-b139-e020f34cc544"),
            Name = "操作查询",
            Code = "operationLogQuery",
            ScopeCode = "audit:operatorlog:query",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = OperationLog.Id
        };

        public static Menu OperationLogRemove = new()
        {
            Id = Guid.Parse("0f6a9b4b-3170-4b7c-a34e-8d7f7d306ba0"),
            Name = "操作删除",
            Code = "operationLogRemove",
            ScopeCode = "audit:operatorlog:remove",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = OperationLog.Id
        };

        // 登录日志
        public static Menu LoginLog = new()
        {
            Id = Guid.Parse("bfedbb92-678d-4cc5-8d88-3cb32b30dfbf"),
            Name = "登录日志",
            Code = "loginLog",
            ScopeCode = "audit:loginlog:list",
            Type = MenuType.Menu,
            Route = "/monitor/login-logs",
            Visible = true,
            IsLink = false,
            IsCache = true,
            Component = "monitor/logs/login/index",
            IconUrl = "ri:window-line",
            OrderNum = 100,
            ParentId = Monitoring.Id,
            RouteName = "LoginLog"
        };

        public static Menu LoginLogQuery = new()
        {
            Id = Guid.Parse("36e8e2e2-35fa-4bb0-bb44-b9a8e09ff795"),
            Name = "登录查询",
            Code = "loginLogQuery",
            ScopeCode = "audit:loginlog:query",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = LoginLog.Id
        };

        public static Menu LoginLogRemove = new()
        {
            Id = Guid.Parse("05a5aeaf-4b26-42b8-8269-d6d740fa21f7"),
            Name = "登录删除",
            Code = "loginLogRemove",
            ScopeCode = "audit:loginlog:remove",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = LoginLog.Id
        };

        // 字典管理
        public static Menu Dict = new()
        {
            Id = Guid.Parse("d0a35a0d-c697-4699-975f-2fe9d40c11ef"),
            Name = "字典管理",
            Code = "dict",
            ScopeCode = "system:dictionarytype:list",
            Type = MenuType.Menu,
            IconUrl = "ep:reading",
            Route = "/system/dict/index",
            OrderNum = 95,
            ParentId = System.Id,
            RouteName = "SystemDict"
        };

        public static Menu DictQuery = new()
        {
            Id = Guid.Parse("baf8191b-4b9b-4643-9b3b-8ab5a9b972fb"),
            Name = "字典查询",
            Code = "dictQuery",
            ScopeCode = "system:dictionarytype:query",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = Dict.Id
        };

        public static Menu DictAdd = new()
        {
            Id = Guid.Parse("e75bfe27-8f39-4b24-8f8f-cd2065d6399a"),
            Name = "字典新增",
            Code = "dictAdd",
            ScopeCode = "system:dictionarytype:add",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = Dict.Id
        };

        public static Menu DictEdit = new()
        {
            Id = Guid.Parse("02e6a4b3-cb2e-456d-979e-63d6ac7e4c8a"),
            Name = "字典修改",
            Code = "dictEdit",
            ScopeCode = "system:dictionarytype:edit",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = Dict.Id
        };

        public static Menu DictRemove = new()
        {
            Id = Guid.Parse("4e3a97bc-b2f1-47b4-a35e-75b8d7c2f146"),
            Name = "字典删除",
            Code = "dictRemove",
            ScopeCode = "system:dictionarytype:remove",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = Dict.Id
        };

        // 通知管理
        public static Menu Notice = new()
        {
            Id = Guid.Parse("3f97be16-3007-4642-8387-6b8d86e6073f"),
            Name = "通知管理",
            Code = "notice",
            ScopeCode = "system:notice:list",
            Type = MenuType.Menu,
            IconUrl = "ep:chat-line-round",
            Route = "/system/notice/index",
            OrderNum = 95,
            ParentId = System.Id,
            RouteName = "SystemNotice"
        };

        public static Menu NoticeQuery = new()
        {
            Id = Guid.Parse("26cdb321-5c8e-4e3f-97da-19c735465ee7"),
            Name = "通知查询",
            Code = "noticeQuery",
            ScopeCode = "system:notice:query",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = Notice.Id
        };

        public static Menu NoticeAdd = new()
        {
            Id = Guid.Parse("a635b988-4b74-4b9e-9247-1ff8c87777ba"),
            Name = "通知新增",
            Code = "noticeAdd",
            ScopeCode = "system:notice:add",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = Notice.Id
        };

        public static Menu NoticeEdit = new()
        {
            Id = Guid.Parse("71e948a5-38c2-4297-a9d0-c6815774ff80"),
            Name = "通知修改",
            Code = "noticeEdit",
            ScopeCode = "system:notice:edit",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = Notice.Id
        };

        public static Menu NoticeRemove = new()
        {
            Id = Guid.Parse("dd9f030e-299e-40a1-87f7-226b7f9e5641"),
            Name = "通知删除",
            Code = "noticeRemove",
            ScopeCode = "system:notice:remove",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = Notice.Id
        };

        // 配置管理
        public static Menu Config = new()
        {
            Id = Guid.Parse("c591c53b-c173-4e5b-b5a9-c5e01e5a927d"),
            Name = "配置管理",
            Code = "config",
            ScopeCode = "system:config:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/system/config/index",
            OrderNum = 95,
            ParentId = System.Id,
            RouteName = "SystemConfig"
        };

        public static Menu ConfigQuery = new()
        {
            Id = Guid.Parse("46d58b30-b9b0-4d95-a243-bab2c9876c0e"),
            Name = "配置查询",
            Code = "configQuery",
            ScopeCode = "system:config:query",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = Config.Id
        };

        public static Menu ConfigAdd = new()
        {
            Id = Guid.Parse("8aeb832e-dc7e-4f23-bdf6-4f2cbd93a9ea"),
            Name = "配置新增",
            Code = "configAdd",
            ScopeCode = "system:config:add",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = Config.Id
        };

        public static Menu ConfigEdit = new()
        {
            Id = Guid.Parse("aad684a8-d0f5-40b7-a6d3-58d063be0ef4"),
            Name = "配置修改",
            Code = "configEdit",
            ScopeCode = "system:config:edit",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = Config.Id
        };

        public static Menu ConfigRemove = new()
        {
            Id = Guid.Parse("40ec09e4-c231-4f83-97b1-9534b233e381"),
            Name = "配置删除",
            Code = "configRemove",
            ScopeCode = "system:config:remove",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = Config.Id
        };

        #region 系统管理

        // 编码管理
        public static Menu SysCode = new()
        {
            Id = Guid.Parse("3b9b8f3e-8ff0-4e7b-94ff-d53d5682f780"),
            Name = "编码管理",
            Code = "sysCode",
            ScopeCode = "system:coderule:list",
            Type = MenuType.Menu,
            IconUrl = "ri:barcode-box-fill",
            Route = "/system/code/index",
            OrderNum = 95,
            ParentId = System.Id,
            RouteName = "SystemCode"
        };

        // 主数据管理
        public static Menu MainData = new()
        {
            Id = Guid.Parse("1f28cb9f-678c-4e5f-8c6b-e7d8d6d538da"),
            Name = "主数据管理",
            Code = "mainData",
            Type = MenuType.Catalogue,
            Route = "/main",
            IconUrl = "ri:settings-3-line",
            OrderNum = 100,
            ParentId = Root.Id
        };

        // 计量单位管理
        public static Menu UnitManage = new()
        {
            Id = Guid.Parse("71e3bc0a-b742-4e8e-8fd7-e9d1dbdfdba4"),
            Name = "计量单位管理",
            Code = "unitManage",
            ScopeCode = "main:unit:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/main/unit/index",
            OrderNum = 95,
            ParentId = MainData.Id,
            RouteName = "UnitManage"
        };

        // 客户管理
        public static Menu CustomerManage = new()
        {
            Id = Guid.Parse("54c4a548-daba-4784-b960-c20de9d82863"),
            Name = "客户管理",
            Code = "customerManage",
            ScopeCode = "main:customer:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/main/customer/index",
            OrderNum = 95,
            ParentId = MainData.Id,
            RouteName = "CustomerManage"
        };

        // 供应商管理
        public static Menu SupplierManage = new()
        {
            Id = Guid.Parse("fcdc0dbf-3c2a-48f5-a500-bff3e418876b"),
            Name = "供应商管理",
            Code = "supplierManage",
            ScopeCode = "main:supplier:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/main/supplier/index",
            OrderNum = 95,
            ParentId = MainData.Id,
            RouteName = "SupplierManage"
        };

        #endregion

        #region 仓储管理

        // 仓储管理
        public static Menu Warehouse = new()
        {
            Id = Guid.Parse("f92a2adf-9f3e-4b35-81c0-42320a7b95bc"),
            Name = "仓储管理",
            Code = "warehouse",
            Type = MenuType.Catalogue,
            Route = "/warehouse",
            IconUrl = "ri:settings-3-line",
            OrderNum = 100,
            ParentId = Root.Id
        };

        // 仓库设置
        public static Menu WarehouseSet = new()
        {
            Id = Guid.Parse("44c68155-51ed-4de5-bb94-3f7b3fa635ac"),
            Name = "仓库设置",
            Code = "warehouseSet",
            ScopeCode = "warehouse:set:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/warehouse/set/index",
            OrderNum = 95,
            ParentId = Warehouse.Id,
            RouteName = "WarehouseSet"
        };

        // 库存现有量
        public static Menu WarehouseStock = new()
        {
            Id = Guid.Parse("26f6a235-0543-44fa-bd7e-5220cfdd1d33"),
            Name = "库存现有量",
            Code = "warehouseStock",
            ScopeCode = "warehouse:stock:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/warehouse/stock/index",
            OrderNum = 95,
            ParentId = Warehouse.Id,
            RouteName = "WarehouseStock"
        };

        // 采购入库
        public static Menu ProcureWarehouse = new()
        {
            Id = Guid.Parse("70c56be3-66ad-418b-b72d-5c778b592eb3"),
            Name = "采购入库",
            Code = "procureWarehouse",
            ScopeCode = "warehouse:procure:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/warehouse/procure/index",
            OrderNum = 95,
            ParentId = Warehouse.Id,
            RouteName = "ProcureWarehouse"
        };

        // 供应商退货
        public static Menu SupplierReturn = new()
        {
            Id = Guid.Parse("9835dbf4-7c2f-4f0d-a87e-f539c6a9c593"),
            Name = "供应商退货",
            Code = "supplierReturn",
            ScopeCode = "warehouse:supplier-return:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/warehouse/supplier-return/index",
            OrderNum = 95,
            ParentId = Warehouse.Id,
            RouteName = "SupplierReturn"
        };

        // 生产领料
        public static Menu ProductRequire = new()
        {
            Id = Guid.Parse("cc3f76b2-b3ca-40b0-b96b-0d97adf7a053"),
            Name = "生产领料",
            Code = "productRequire",
            ScopeCode = "warehouse:product-require:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/warehouse/product-require/index",
            OrderNum = 95,
            ParentId = Warehouse.Id,
            RouteName = "ProductRequire"
        };

        // 生产退料
        public static Menu ProductReturn = new()
        {
            Id = Guid.Parse("c78480fe-818b-4f6b-b13b-cfad4cfaa53d"),
            Name = "生产退料",
            Code = "productReturn",
            ScopeCode = "warehouse:product-return:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/warehouse/product-return/index",
            OrderNum = 95,
            ParentId = Warehouse.Id,
            RouteName = "ProductReturn"
        };

        // 产品入库
        public static Menu ProductWarehouse = new()
        {
            Id = Guid.Parse("37b3c7a2-337f-4fe9-b312-0c5f5137f56f"),
            Name = "产品入库",
            Code = "productWarehouse",
            ScopeCode = "warehouse:product-warehouse:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/warehouse/product-warehouse/index",
            OrderNum = 95,
            ParentId = Warehouse.Id,
            RouteName = "ProductWarehouse"
        };

        // 销售出库
        public static Menu SalesOut = new()
        {
            Id = Guid.Parse("8b0e88bc-c4f6-44b6-bb6a-cba4c1e1d3ff"),
            Name = "销售出库",
            Code = "salesOut",
            ScopeCode = "warehouse:sales-out:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/warehouse/sales-out/index",
            OrderNum = 95,
            ParentId = Warehouse.Id,
            RouteName = "SalesOut"
        };

        // 销售退货
        public static Menu SalesReturn = new()
        {
            Id = Guid.Parse("df7b94cd-d687-4e38-848d-063013b66b2f"),
            Name = "销售退货",
            Code = "salesReturn",
            ScopeCode = "warehouse:sales-return:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/warehouse/sales-return/index",
            OrderNum = 95,
            ParentId = Warehouse.Id,
            RouteName = "SalesReturn"
        };

        #endregion

        #region 设备管理

        // 设备管理
        public static Menu Equip = new()
        {
            Id = Guid.Parse("60d2d8b3-781b-4f62-a9a0-9f287bca8706"),
            Name = "设备管理",
            Code = "equip",
            Type = MenuType.Catalogue,
            Route = "/equip",
            IconUrl = "ri:settings-3-line",
            OrderNum = 100,
            ParentId = Root.Id
        };

        // 设备类型
        public static Menu EquipType = new()
        {
            Id = Guid.Parse("0e1c18ae-b6ff-4a29-9b7e-639bb078b3b2"),
            Name = "设备类型",
            Code = "equipType",
            ScopeCode = "equip:equiptype:list",
            Type = MenuType.Menu,
            IconUrl = "ri:equalizer-line",
            Route = "/equip/type/index",
            OrderNum = 100,
            ParentId = Equip.Id,
            RouteName = "EquipType"
        };

        // 设备台账
        public static Menu EquipLedger = new()
        {
            Id = Guid.Parse("34931d71-8f6f-4018-933b-dc9cd8cb6b9f"),
            Name = "设备台账",
            Code = "equipLedger",
            ScopeCode = "equip:equipledger:list",
            Type = MenuType.Menu,
            IconUrl = "ri:equal-fill",
            Route = "/equip/ledger/index",
            OrderNum = 99,
            ParentId = Equip.Id,
            RouteName = "EquipLedger"
        };
        
        // 设备参数配置
        public static Menu EquipConfig = new()
        {
            Id = Guid.Parse("62a7f52b-1df2-462d-a4b6-0816c603e6fd"),
            Name = "设备参数配置",
            Code = "equipConfig",
            ScopeCode = "equip:equipconnect:list",
            Type = MenuType.Menu,
            IconUrl = "ep:connection",
            Route = "/equip/connect/index",
            OrderNum = 98,
            ParentId = Equip.Id,
            RouteName = "EquipConfig"
        };
        // 设备采集数据
        public static Menu EquipTestAnalyse = new()
        {
            Id = Guid.Parse("372A3414-DCC0-2AAB-E54D-5F26AA3CDB7F"),
            Name = "试验数据",
            Code = "equipTestAnalyse",
            ScopeCode = "equip:testanalyse:list",
            Type = MenuType.Menu,
            IconUrl = "ri:alarm-warning-line",
            Route = "/equip/test-analyse/index",
            OrderNum = 98,
            ParentId = Equip.Id,
            RouteName = "EquipTestAnalyse"
        };
        // 设备采集配置
        public static Menu EquipDataConfig = new()
        {
            Id = Guid.Parse("bc71d5b9-cbe1-4ff3-b8ae-8f8ae94a05ff"),
            Name = "设备采集配置",
            Code = "equipDataConfig",
            ScopeCode = "equip:data:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/equip/data/index",
            OrderNum = 89,
            ParentId = Equip.Id,
            RouteName = "EquipData"
        };
        // 设备采集配置
        public static Menu TestData = new()
        {
            Id = Guid.Parse("77A29618-FBD4-5D95-5180-D3366B4E9064"),
            Name = "实验计划数据",
            Code = "testData",
            ScopeCode = "equip:testdata:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/equip/test-data/index",
            OrderNum = 89,
            ParentId = Equip.Id,
            RouteName = "TestData"
        };
        // 设备历史记录
        public static Menu EquipLedgerHistory = new()
        {
            Id = Guid.Parse("B46B7ECC-E9B9-4AE4-5B67-6902BBCE1B8B"),
            Name = "设备历史记录",
            Code = "equipLedgerHistory",
            ScopeCode = "equip:equipledgerhistory:list",
            Type = MenuType.Menu,
            IconUrl = "fa-solid:history",
            Route = "/equip/history/index",
            OrderNum = 97,
            ParentId = Equip.Id,
            RouteName = "EquipLedgerHistory"
        };
        // 设备通知信息
        public static Menu EquipNotice = new()
        {
            Id = Guid.Parse("3C05CB50-0046-5418-F28E-ACB61E58CB68"),
            Name = "设备通知",
            Code = "equipNotice",
            ScopeCode = "equip:equipnotice:list",
            Type = MenuType.Menu,
            IconUrl = "ri:alarm-warning-line",
            Route = "/equip/notice/index",
            OrderNum = 96,
            ParentId = Equip.Id,
            RouteName = "EquipNotice"
        };
        // 设备项目
        public static Menu EquipItems = new()
        {
            Id = Guid.Parse("fd9c4626-6c96-4c39-8de0-2d57b0e2b46b"),
            Name = "设备项目",
            Code = "equipItems",
            ScopeCode = "equip:items:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/equip/item/index",
            OrderNum = 98,
            ParentId = Equip.Id,
            RouteName = "EquipItems"
        };

        // 维保计划
        public static Menu EquipPlan = new()
        {
            Id = Guid.Parse("9c4532b5-2b85-4624-bb3f-e37e416aa7b7"),
            Name = "维保计划",
            Code = "equipPlan",
            ScopeCode = "equip:plan:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/equip/plan/index",
            OrderNum = 97,
            ParentId = Equip.Id,
            RouteName = "EquipPlan"
        };

        // 维保计划执行表
        public static Menu EquipPlanDone = new()
        {
            Id = Guid.Parse("ac58e5f6-43d5-4a34-ae7f-412b0ad0e79a"),
            Name = "维保计划执行表",
            Code = "equipPlanDone",
            ScopeCode = "equip:task:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/equip/task/index",
            OrderNum = 96,
            ParentId = Equip.Id,
            RouteName = "EquipPlanDone"
        };

        // 维修单
        public static Menu EquipRepair = new()
        {
            Id = Guid.Parse("d4e7385e-945e-4082-819e-93e6a8ecf4f2"),
            Name = "维修单",
            Code = "equipRepair",
            ScopeCode = "equip:repair:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/equip/repair/index",
            OrderNum = 95,
            ParentId = Equip.Id,
            RouteName = "EquipRepair"
        };

        // 维保记录表
        public static Menu EquipPlanRecord = new()
        {
            Id = Guid.Parse("b2fa081e-c8ed-4382-bab7-556f8e3f6478"),
            Name = "维保记录表",
            Code = "equipPlanRecord",
            ScopeCode = "equip:record:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/equip/record:/index",
            OrderNum = 94,
            ParentId = Equip.Id,
            RouteName = "EquipPlanRecord"
        };

        #endregion

        #region 生产管理

        // 生产管理
        public static Menu Product = new()
        {
            Id = Guid.Parse("2f538f9f-cb7a-45a2-9bc7-93141de3759b"),
            Name = "生产管理",
            Code = "product",
            Type = MenuType.Catalogue,
            Route = "/product",
            IconUrl = "ri:settings-3-line",
            OrderNum = 100,
            ParentId = Root.Id
        };

        // 生产计划
        public static Menu ProductPlan = new()
        {
            Id = Guid.Parse("a0f4bc56-9d13-44be-8779-2fa447f59c11"),
            Name = "生产计划",
            Code = "productPlan",
            ScopeCode = "product:plan:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/product/plan/index",
            OrderNum = 95,
            ParentId = Product.Id,
            RouteName = "ProductPlan"
        };

        // 生产工单
        public static Menu ProductOrder = new()
        {
            Id = Guid.Parse("8b8f7f67-bd2b-4744-b443-b60a8c6ab157"),
            Name = "生产工单",
            Code = "productOrder",
            ScopeCode = "product:order:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/product/order/index",
            OrderNum = 95,
            ParentId = Product.Id,
            RouteName = "ProductOrder"
        };

        // 生产排产
        public static Menu ProductSchedule = new()
        {
            Id = Guid.Parse("460c2259-0f60-44f0-bd63-7f618d0be16d"),
            Name = "生产排产",
            Code = "productSchedule",
            ScopeCode = "product:schedule:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/product/schedule/index",
            OrderNum = 95,
            ParentId = Product.Id,
            RouteName = "ProductSchedule"
        };

        #endregion

        #region 质量管理

        // 质量管理
        public static Menu Quality = new()
        {
            Id = Guid.Parse("43b2d118-204f-42ea-8d44-42ea524c5c38"),
            Name = "质量管理",
            Code = "quality",
            Type = MenuType.Catalogue,
            Route = "/quality",
            IconUrl = "ri:settings-3-line",
            OrderNum = 100,
            ParentId = Root.Id
        };

        // 检测项设置
        public static Menu QualityItem = new()
        {
            Id = Guid.Parse("0248fd75-0130-4be4-841d-1b21d1166973"),
            Name = "检测项设置",
            Code = "qualityItem",
            ScopeCode = "quality:item:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/quality/item/index",
            OrderNum = 95,
            ParentId = Quality.Id,
            RouteName = "QualityItem"
        };

        // 来料检验
        public static Menu QualityInput = new()
        {
            Id = Guid.Parse("aaf3abed-e041-427f-8fc9-11c697a47d7f"),
            Name = "来料检验",
            Code = "qualityInput",
            ScopeCode = "quality:input:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/quality/input/index",
            OrderNum = 95,
            ParentId = Quality.Id,
            RouteName = "QualityInput"
        };

        // 过程检验
        public static Menu QualityProcess = new()
        {
            Id = Guid.Parse("cd45b6b1-1ab7-4854-b79a-55a5827b2199"),
            Name = "过程检验",
            Code = "qualityProcess",
            ScopeCode = "quality:process:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/quality/process/index",
            OrderNum = 95,
            ParentId = Quality.Id,
            RouteName = "QualityProcess"
        };

        // 出货检验
        public static Menu QualityOutput = new()
        {
            Id = Guid.Parse("3e720a7f-bd4c-4c78-82a6-fd053d43e6f3"),
            Name = "出货检验",
            Code = "qualityOutput",
            ScopeCode = "quality:output:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/quality/output/index",
            OrderNum = 95,
            ParentId = Quality.Id,
            RouteName = "QualityOutput"
        };

        #endregion

        #region 排班管理

        // 排班管理
        public static Menu Schedule = new()
        {
            Id = Guid.Parse("b8d7485f-3656-469d-b4a9-b1efb05d42e0"),
            Name = "排班管理",
            Code = "schedule",
            Type = MenuType.Catalogue,
            Route = "/schedule",
            IconUrl = "ri:settings-3-line",
            OrderNum = 100,
            ParentId = Root.Id
        };

        // 班组设置
        public static Menu ScheduleTeam = new()
        {
            Id = Guid.Parse("e5b79d88-d6cd-4f8d-9d8d-bd39baf5e593"),
            Name = "班组设置",
            Code = "scheduleTeam",
            ScopeCode = "schedule:team:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/schedule/output/index",
            OrderNum = 95,
            ParentId = Schedule.Id,
            RouteName = "ScheduleTeam"
        };

        // 排班计划
        public static Menu SchedulePlan = new()
        {
            Id = Guid.Parse("1a3bcdba-5d74-4049-986f-4e8c8be0452f"),
            Name = "排班计划",
            Code = "schedulePlan",
            ScopeCode = "schedule:plan:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/schedule/plan/index",
            OrderNum = 95,
            ParentId = Schedule.Id,
            RouteName = "SchedulePlan"
        };

        // 节假日设置
        public static Menu ScheduleHoliday = new()
        {
            Id = Guid.Parse("92b85a9a-4557-43d5-84d9-57d1bfa4b12d"),
            Name = "节假日设置",
            Code = "scheduleHoliday",
            ScopeCode = "schedule:holiday:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/schedule/holiday/index",
            OrderNum = 95,
            ParentId = Schedule.Id,
            RouteName = "ScheduleHoliday"
        };

        // 排班日历
        public static Menu ScheduleCalendar = new()
        {
            Id = Guid.Parse("55643f0d-d694-4877-b482-4059b3a7fc0f"),
            Name = "排班日历",
            Code = "scheduleCalendar",
            ScopeCode = "schedule:calendar:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/schedule/calendar/index",
            OrderNum = 95,
            ParentId = Schedule.Id,
            RouteName = "ScheduleCalendar"
        };

        #endregion

        #region 楼层管理

        // 地点管理
        public static Menu Location = new()
        {
            Id = Guid.Parse("A9816E1F-7561-541B-956E-B593A23C89ED"),
            Name = "地点管理",
            Code = "location",
            Type = MenuType.Catalogue,
            Route = "/location",
            IconUrl = "ep:location",
            OrderNum = 100,
            ParentId = Root.Id
        };

        // 楼栋管理
        public static Menu Building = new()
        {
            Id = Guid.Parse("A11CE7DB-3DBF-8CBE-D4B9-8CAC730B5D52"),
            Name = "楼栋管理",
            Code = "building",
            ScopeCode = "system:building:list",
            Type = MenuType.Menu,
            IconUrl = "ep:office-building",
            Route = "/location/building/index",
            OrderNum = 99,
            ParentId = Location.Id,
            RouteName = "building"
        };
        // 楼层管理
        public static Menu Floor = new()
        {
            Id = Guid.Parse("110E536E-4D8E-11A8-2892-E4B68A13B448"),
            Name = "楼层管理",
            Code = "Floor",
            ScopeCode = "system:floor:list",
            Type = MenuType.Menu,
            IconUrl = "ri:flood-fill",
            Route = "/location/floor/index",
            OrderNum = 98,
            ParentId = Location.Id,
            RouteName = "Floor"
        };
        // 房间管理
        public static Menu Room = new()
        {
            Id = Guid.Parse("D0F66616-B831-0768-3054-F00B1D9A295B"),
            Name = "房间管理",
            Code = "Room",
            ScopeCode = "system:room:list",
            Type = MenuType.Menu,
            IconUrl = "fa-solid:crosshairs",
            Route = "/location/room/index",
            OrderNum = 97,
            ParentId = Location.Id,
            RouteName = "Room"
        };
        #endregion
        
        #region 楼层管理

        // 展示区域
        public static Menu ShowSystem = new()
        {
            Id = Guid.Parse("6CF45997-C39B-6382-0995-AFE0CDE0038F"),
            Name = "展示区域",
            Code = "showSystem",
            Type = MenuType.Catalogue,
            Route = "/show-system",
            IconUrl = "ep:location",
            OrderNum = 100,
            ParentId = Root.Id
        };

        // 大屏展示1
        public static Menu ShowSystem1 = new()
        {
            Id = Guid.Parse("82BA9B27-6926-1657-B07D-B64281A35A26"),
            Name = "展示系统",
            Code = "ShowSystem1",
            Type = MenuType.Menu,
            IconUrl = "ep:office-building",
            Route = "http://192.168.110.94",
            OrderNum = 99,
            IsLink = true,
            ParentId = ShowSystem.Id,
            RouteName = "ShowSystem1"
        };
        // 大屏展示1
        public static Menu ShowSystem2 = new()
        {
            Id = Guid.Parse("78BA5C57-4A86-7F44-282E-8F5EF117C72B"),
            Name = "展示系统1",
            Code = "ShowSystem2",
            Type = MenuType.Menu,
            IconUrl = "ep:office-building",
            Route = "http://www.baidu.com",
            OrderNum = 99,
            IsLink = true,
            ParentId = ShowSystem.Id,
            RouteName = "ShowSystem2"
        };
        #endregion
        public static Menu[] Seeds { get; } = [
            Root,
            System,
            Monitoring,
            Online,
            User, UserQuery, UserAdd, UserEdit, UserRemove, UserResetPwd,
            Role, RoleQuery, RoleAdd, RoleEdit, RoleRemove,
            MenuRoot, MenuQuery, MenuAdd, MenuEdit, MenuRemove,
            Dept, DeptQuery, DeptAdd, DeptEdit, DeptRemove,
            // Post, PostQuery, PostAdd, PostEdit, PostRemove,
            OperationLog, OperationLogQuery, OperationLogRemove,
            LoginLog, LoginLogQuery, LoginLogRemove,
            Dict, DictQuery, DictAdd, DictEdit, DictRemove,
            //Notice, NoticeQuery, NoticeAdd, NoticeEdit, NoticeRemove,
            // Config, ConfigQuery, ConfigAdd, ConfigEdit, ConfigRemove,
            SysCode,
            //MainData, UnitManage, CustomerManage, SupplierManage,
            //Warehouse, WarehouseSet, WarehouseStock, ProcureWarehouse, SupplierReturn,
            //ProductRequire, ProductReturn, ProductWarehouse, SalesOut, SalesReturn,
            Equip, EquipType, EquipLedger, EquipLedgerHistory,EquipNotice,EquipTestAnalyse,TestData,
            //EquipItems, EquipPlan, EquipPlanDone,EquipRepair, EquipPlanRecord,
            EquipConfig,// EquipDataConfig,
            //Product, ProductPlan, ProductOrder, ProductSchedule,
            //Quality, QualityItem, QualityInput, QualityProcess, QualityOutput,
            //Schedule, ScheduleTeam, SchedulePlan, ScheduleHoliday, ScheduleCalendar,
            Location,Building,Floor,Room,
            ShowSystem,ShowSystem1,ShowSystem2
        ];

        #endregion
    }
}