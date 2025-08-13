using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Shared.Enums;

namespace Hgzn.Mes.Domain.Entities.System.Authority
{
    public class Menu : UniversalEntity, ISoftDelete, IAudited, IOrder, IState, ISeedsGeneratable
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

        public bool IsShow { get; set; } = true;

        public bool IsCache { get; set; } = false;

        public bool Favorite { get; set; } = false;

        public bool State { get; set; }


        #region audit

        public Guid? CreatorId { get; set; }

        public DateTime CreationTime { get; set; }

        public int CreatorLevel { get; set; } = 0;

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

        #region API组合权限

        #region ApiRoot

        public static Menu APIRoot = new()
        {
            Id = new Guid("315a2f4d-07e7-4e06-986e-9bb5760527fa"),
            Name = "后端接口根权限",
            Code = "apiRoot",
            Description = "后端接口跟权限",
            Type = MenuType.API,
            OrderNum = 10,
            ParentId = Root.Id
        };

        #endregion APIRoot

        #region 基础增删改查

        public static Menu APIRemove = new()
        {
            Id = new Guid("7b931be6-0692-4835-900c-c4bd1bc31ea3"),
            Name = "后端接口删除权限",
            ScopeCode = $"{ScopeMethodType.Remove:F}".ToLower(),
            Code = "apiRemove",
            Description = "批量赋予后端接口删除权限",
            Type = MenuType.API,
            OrderNum = 10,
            ParentId = APIRoot.Id
        };

        public static Menu APIEdit = new()
        {
            Id = new Guid("5470e938-b833-4eea-8223-335691dbf4fa"),
            Name = "后端接口修改权限",
            ScopeCode = $"{ScopeMethodType.Edit:F}".ToLower(),
            Code = "apiEdit",
            Description = "批量赋予后端接口修改权限",
            Type = MenuType.API,
            OrderNum = 10,
            ParentId = APIRoot.Id
        };

        public static Menu APIAdd = new()
        {
            Id = new Guid("b9096eb5-dd82-4b64-aea0-b649dcee97b1"),
            Name = "后端接口新增权限",
            ScopeCode = $"{ScopeMethodType.Add:F}".ToLower(),
            Code = "apiAdd",
            Description = "批量赋予后端接口新增权限",
            Type = MenuType.API,
            OrderNum = 10,
            ParentId = APIRoot.Id
        };

        public static Menu APIQuery = new()
        {
            Id = new Guid("35c5ad6a-5047-4aaa-aac6-9a3f976407dd"),
            Name = "后端接口查询权限",
            ScopeCode = $"{ScopeMethodType.Query:F}".ToLower(),
            Code = "apiQuery",
            Description = "批量赋予后端接口查询权限",
            Type = MenuType.API,
            OrderNum = 10,
            ParentId = APIRoot.Id
        };

        public static Menu APIList = new()
        {
            Id = new Guid("26fc1d1d-1361-4e17-9d33-ae111156bc26"),
            Name = "后端接口列表权限",
            ScopeCode = $"{ScopeMethodType.List:F}".ToLower(),
            Code = "apiList",
            Description = "批量赋予后端接口列表权限",
            Type = MenuType.API,
            OrderNum = 10,
            ParentId = APIRoot.Id
        };

        #endregion

        #region 接口范围控制

        public static Menu APIBasic = new()
        {
            Id = new Guid("8be0c076-0993-40f3-a3a0-6403275aa2fb"),
            Name = "后端接口基础管理权限",
            ScopeCode = $"basic",
            Code = "apiBasic",
            Description = "批量赋予后端接口列表权限",
            Type = MenuType.API,
            OrderNum = 10,
            ParentId = APIRoot.Id
        };

        public static Menu APIEquip = new()
        {
            Id = new Guid("618c7135-34b3-43cb-b460-ec094fa65a11"),
            Name = "后端接口设备权限",
            ScopeCode = $"basic",
            Code = "apiEquip",
            Description = "批量赋予后端接口设备权限",
            Type = MenuType.API,
            OrderNum = 10,
            ParentId = APIRoot.Id
        };

        public static Menu APISystem = new()
        {
            Id = new Guid("c0303ac0-d1b9-4c23-9883-6ed9417a1286"),
            Name = "后端接口系统管理权限",
            ScopeCode = $"system",
            Code = "apiSystem",
            Description = "批量赋予后端接口系统管理权限",
            Type = MenuType.API,
            OrderNum = 10,
            ParentId = APIRoot.Id
        };

        #endregion 接口范围控制

        #endregion

        public static Menu BasisPlatform = new()
        {
            Id = new Guid("56c318dd-fa9e-45e8-89ee-c2b8b1581c41"),
            Name = "基础物联网平台",
            Code = "basisPlatform",
            Description = "basisPlatform menu",
            Type = MenuType.Catalogue,
            Route = "/basisPlatform",
            IconUrl = "ri:settings-3-line",
            OrderNum = 10,
            ParentId = Root.Id
        };

        public static Menu EquipmentResourceManagementApplication = new()
        {
            Id = new Guid("4275de80-8620-4187-842a-dd24aa01e9a9"),
            Name = "设备资源管理应用",
            Code = "equipmentResourceManagementApplication",
            Description = "basisPlatform menu",
            Type = MenuType.Catalogue,
            Route = "/EquipmentResourceManagementApplication",
            IconUrl = "ri:settings-3-line",
            OrderNum = 11,
            ParentId = Root.Id
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
            OrderNum = 100,
            ParentId = BasisPlatform.Id
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
            OrderNum = 100,
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
            OrderNum = 102,
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
            OrderNum = 101,
            ParentId = System.Id,
            RouteName = "SystemCode"
        };

        public static Menu SysConfig = new()
        {
            Id = Guid.Parse("8B91C05F-C0C7-55F1-84A3-0338ED7AC78E"),
            Name = "配置管理",
            Code = "sysConfig",
            ScopeCode = "system:baseConfig:list",
            Type = MenuType.Menu,
            IconUrl = "ep:set-up",
            Route = "/system/config/index",
            OrderNum = 102,
            ParentId = System.Id,
            RouteName = "SystemConfig"
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
            OrderNum = 102,
            ParentId = BasisPlatform.Id
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
            OrderNum = 1000,
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
            OrderNum = 1001,
            ParentId = Equip.Id,
            RouteName = "EquipLedger"
        };

        // 台账新增
        public static Menu EquipAdd = new()
        {
            Id = Guid.Parse("27deb649-a9bb-4811-9d37-00841c8b72df"),
            Name = "台账新增",
            Code = "equipAdd",
            ScopeCode = "equip:equipledger:add",
            Type = MenuType.Component,
            OrderNum = 1002,
            ParentId = EquipLedger.Id
        };

        // 台账修改
        public static Menu EquipEdit = new()
        {
            Id = Guid.Parse("2ff3e073-9444-4f72-b446-b69f86606567"),
            Name = "台账修改",
            Code = "equipEdit",
            ScopeCode = "equip:equipledger:edit",
            Type = MenuType.Component,
            OrderNum = 1003,
            ParentId = EquipLedger.Id
        };

        // 台账查询
        public static Menu EquipQuery = new()
        {
            Id = Guid.Parse("524fa02f-2c48-4779-bed8-e978b2f7c206"),
            Name = "台账查询",
            Code = "equipQuery",
            ScopeCode = "equip:equipledger:query",
            Type = MenuType.Component,
            OrderNum = 1004,
            ParentId = EquipLedger.Id
        };

        // 台账删除
        public static Menu EquipRemove = new()
        {
            Id = Guid.Parse("eeb46cb3-eaec-4e49-b7d3-aaefba3af2f4"),
            Name = "台账删除",
            Code = "equipRemove",
            ScopeCode = "equip:equipledger:remove",
            Type = MenuType.Component,
            OrderNum = 1005,
            ParentId = EquipLedger.Id
        };

        // 设备连接管理
        public static Menu EquipConnectManagement = new()
        {
            Id = Guid.Parse("62a7f52b-1df2-462d-a4b6-0816c603e6fd"),
            Name = "设备连接管理",
            Code = "equipConfig",
            ScopeCode = "equip:equipconnect:list",
            Type = MenuType.Menu,
            IconUrl = "ep:connection",
            Route = "/equip/connect/index",
            OrderNum = 1002,
            ParentId = Equip.Id,
            RouteName = "EquipConfig"
        };

        // 设备连接新增
        public static Menu EquipConnectAdd = new()
        {
            Id = Guid.Parse("7b13d4b0-92b9-4d80-b647-24ff8f87edc0"),
            Name = "设备连接新增",
            Code = "equipConnectAdd",
            ScopeCode = "equip:equipconnect:add",
            Type = MenuType.Component,
            OrderNum = 1002,
            ParentId = EquipConnectManagement.Id
        };

        // 设备连接修改
        public static Menu EquipConnectEdit = new()
        {
            Id = Guid.Parse("ceef19dc-17c6-4cdc-8c02-2c86f65afc6d"),
            Name = "设备连接修改",
            Code = "equipConnectEdit",
            ScopeCode = "equip:equipconnect:edit",
            Type = MenuType.Component,
            OrderNum = 1003,
            ParentId = EquipConnectManagement.Id
        };

        // 设备连接查询
        public static Menu EquipConnectQuery = new()
        {
            Id = Guid.Parse("d1165f41-b5c5-41f4-8a10-58088488ac3f"),
            Name = "设备连接查询",
            Code = "equipConnectQuery",
            ScopeCode = "equip:equipconnect:query",
            Type = MenuType.Component,
            OrderNum = 1004,
            ParentId = EquipConnectManagement.Id
        };

        // 设备连接删除
        public static Menu EquipConnectRemove = new()
        {
            Id = Guid.Parse("ce029ac3-959c-4fa6-8916-eaa5ffe8587c"),
            Name = "设备连接删除",
            Code = "equipConnectRemove",
            ScopeCode = "equip:equipconnect:remove",
            Type = MenuType.Component,
            OrderNum = 1005,
            ParentId = EquipConnectManagement.Id
        };

        // 设备采集配置
        public static Menu EquipDataConfig = new()
        {
            Id = Guid.Parse("bc71d5b9-cbe1-4ff3-b8ae-8f8ae94a05ff"),
            Name = "设备采集配置",
            Code = "equipDataConfig",
            ScopeCode = "equip:data-point:list",
            Type = MenuType.Menu,
            IconUrl = "ep:management",
            Route = "/equip/data-point/index",
            OrderNum = 95,
            ParentId = Equip.Id,
            RouteName = "EquipData"
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
            OrderNum = 1003,
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
            OrderNum = 91,
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
            OrderNum = 90,
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
            OrderNum = 89,
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
            OrderNum = 88,
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
            Route = "/equip/record/index",
            OrderNum = 87,
            ParentId = Equip.Id,
            RouteName = "EquipPlanRecord"
        };

        #endregion

        #region 数据管理

        // 数据管理
        public static Menu DataManagement = new()
        {
            Id = Guid.Parse("173492c1-badc-436e-9164-7198ad62a374"),
            Name = "数据管理",
            Code = "dataManagement",
            Type = MenuType.Catalogue,
            Route = "/dataManagement",
            IconUrl = "ri:settings-3-line",
            OrderNum = 103,
            ParentId = BasisPlatform.Id
        };

        public static Menu EquipRkServer = new()
        {
            Id = Guid.Parse("DE767E24-FE2A-322B-980D-8A11F6CCDFDB"),
            Name = "温湿度计数据",
            Code = "equip_RkServer",
            ScopeCode = "equip:rkserver:list",
            Type = MenuType.Menu,
            IconUrl = "ep:location-information",
            Route = "equip/rkserver/index",
            OrderNum = 1000,
            ParentId = DataManagement.Id,
            RouteName = "equip_rk_server"
        };

        // 导出试验数据
        public static Menu EquipTestAnalyseOutput = new()
        {
            Id = Guid.Parse("01985a48-3d94-74f2-916a-18d762337860"),
            Name = "导出试验数据",
            Code = "EquipTestAnalyseOutput",
            ScopeCode = "equip:testanalyse:list",
            Type = MenuType.Menu,
            IconUrl = "ri:bar-chart-fill",
            Route = "/equip/test-analyse-output/index",
            OrderNum = 1001,
            ParentId = DataManagement.Id,
            RouteName = "EquipTestAnalyseOutput"
        };

        // 试验数据
        public static Menu EquipTestAnalyse = new()
        {
            Id = Guid.Parse("372A3414-DCC0-2AAB-E54D-5F26AA3CDB7F"),
            Name = "试验数据",
            Code = "equipTestAnalyse",
            ScopeCode = "equip:testanalyse:list",
            Type = MenuType.Menu,
            IconUrl = "ri:bar-chart-fill",
            Route = "/equip/test-analyse/index",
            OrderNum = 1001,
            ParentId = DataManagement.Id,
            RouteName = "EquipTestAnalyse"
        };

        // 试验计划数据
        public static Menu TestData = new()
        {
            Id = Guid.Parse("77A29618-FBD4-5D95-5180-D3366B4E9064"),
            Name = "试验计划数据",
            Code = "testData",
            ScopeCode = "equip:testdata:list",
            Type = MenuType.Menu,
            IconUrl = "ri:bar-chart-box-line",
            Route = "/equip/test-data/index",
            OrderNum = 1002,
            ParentId = DataManagement.Id,
            RouteName = "TestData"
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
            OrderNum = 104,
            ParentId = BasisPlatform.Id
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
            OrderNum = 1000,
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
            OrderNum = 1001,
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
            OrderNum = 1002,
            ParentId = Location.Id,
            RouteName = "Room"
        };

        #endregion

        #region 展示管理

        // 设备状态监控应用
        public static Menu ShowSystem = new()
        {
            Id = Guid.Parse("6CF45997-C39B-6382-0995-AFE0CDE0038F"),
            Name = "设备状态监控应用",
            Code = "showSystem",
            Type = MenuType.Catalogue,
            Route = "/show-system",
            IconUrl = "ep:location",
            OrderNum = 10,
            ParentId = Root.Id
        };

        // 设备状态监控应用展示数据
        public static Menu ShowSystemTemp = new()
        {
            Id = Guid.Parse("0198A357-165E-7500-8A2A-13A92E862662"),
            Name = "设备状态监控应用_展示数据",
            Code = "ShowSystemTemp",
            Type = MenuType.Menu,
            Route = "/show-system-temp/index",
            IconUrl = "ep:location",
            IsLink = false,
            IsShow = false,
            OrderNum = 13,
            ParentId = Root.Id,
            RouteName = "ShowSystemTemp"
        };

        // 设备状态监控应用
        public static Menu ShowSystem1 = new()
        {
            Id = Guid.Parse("82BA9B27-6926-1657-B07D-B64281A35A26"),
            Name = "设备状态监控应用",
            Code = "ShowSystem1",
            Type = MenuType.Menu,
            IconUrl = "ep:camera",
            Route = "/show-system/index",
            OrderNum = 12,
            IsLink = false,
            ParentId = Root.Id,
            RouteName = "ShowSystem1"
        };

        // 设备状态监控应用
        public static Menu ShowSystem2 = new()
        {
            Id = Guid.Parse("78BA5C57-4A86-7F44-282E-8F5EF117C72B"),
            Name = "设备状态监控应用",
            Code = "ShowSystem2",
            Type = MenuType.Menu,
            IconUrl = "ep:camera",
            Route = "/show-system/detail",
            OrderNum = 12,
            IsLink = false,
            IsShow = false,
            ParentId = System.Id,
            RouteName = "ShowSystem2"
        };

        #endregion

        #region 基础管理

        // 地点管理
        public static Menu Basic = new()
        {
            Id = Guid.Parse("EDC608D3-2667-85CE-FB70-5A2AFE11A365"),
            Name = "基础管理",
            Code = "basic",
            Type = MenuType.Catalogue,
            Route = "/basic",
            IconUrl = "ep:turn-off",
            OrderNum = 101,
            ParentId = BasisPlatform.Id
        };

        // 单位管理
        public static Menu Unit = new()
        {
            Id = Guid.Parse("C10F1708-4177-D911-827A-563290998957"),
            Name = "单位管理",
            Code = "utils",
            ScopeCode = "basic:unit:list",
            Type = MenuType.Menu,
            IconUrl = "ep:school",
            Route = "/basic/unit/index",
            OrderNum = 99,
            ParentId = Basic.Id,
            RouteName = "utils"
        };

        // 供应商管理
        public static Menu Supplier = new()
        {
            Id = Guid.Parse("4F20F07F-6D34-CDDB-F5D6-9C4C7FCDA2DC"),
            Name = "供应商管理",
            Code = "supplier",
            ScopeCode = "basic:supplier:list",
            Type = MenuType.Menu,
            IconUrl = "ep:office-building",
            Route = "/basic/supplier/index",
            OrderNum = 99,
            ParentId = Basic.Id,
            RouteName = "supplier"
        };
        // 客户管理
        public static Menu Customer = new()
        {
            Id = Guid.Parse("5C1863E7-FAF2-5A43-395E-959E09B13C41"),
            Name = "客户管理",
            Code = "customer",
            ScopeCode = "basic:customer:list",
            Type = MenuType.Menu,
            IconUrl = "ep:avatar",
            Route = "/basic/custom/index",
            OrderNum = 99,
            ParentId = Basic.Id,
            RouteName = "customer"
        };

        #region 用户管理

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
            OrderNum = 1000,
            ParentId = Basic.Id,
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

        #region 角色管理

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
            OrderNum = 1001,
            ParentId = Basic.Id,
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

        #endregion

        #endregion

        #region 菜单管理

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
            OrderNum = 1002,
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
            ParentId = System.Id
        };

        public static Menu MenuAdd = new()
        {
            Id = Guid.Parse("5a8f4d13-2e73-4c27-bd1b-8a5c19f93ef6"),
            Name = "菜单新增",
            Code = "menuAdd",
            ScopeCode = "system:menu:add",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = System.Id
        };

        public static Menu MenuEdit = new()
        {
            Id = Guid.Parse("7e96f8a4-b2be-4e5d-bc73-91e3812f63b7"),
            Name = "菜单修改",
            Code = "menuEdit",
            ScopeCode = "system:menu:edit",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = System.Id
        };

        public static Menu MenuRemove = new()
        {
            Id = Guid.Parse("a7c56b6c-d3a7-4a0d-bf4c-e06fd63165d3"),
            Name = "菜单删除",
            Code = "menuRemove",
            ScopeCode = "system:menu:remove",
            Type = MenuType.Component,
            OrderNum = 100,
            ParentId = System.Id
        };

        #endregion

        #region 日志管理

        // 日志管理
        public static Menu Monitoring = new()
        {
            Id = new Guid("044808f5-afe5-42b4-9720-5c00900538ca"),
            Name = "日志管理",
            Code = "monitoring",
            Description = "monitoring menu",
            Type = MenuType.Catalogue,
            Route = "/monitor",
            IconUrl = "ep:monitor",
            OrderNum = 1003,
            ParentId = Basic.Id
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
            IsShow = true,
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

        #endregion

        #endregion

        #region 标签管理

        // 标签管理
        public static Menu LabelManagement = new()
        {
            Id = Guid.Parse("ad6f3d20-9d0a-47dc-8dc7-c2fe574eab00"),
            Name = "标签管理",
            Code = "labelManagement",
            Type = MenuType.Catalogue,
            Route = "/labelManagement",
            IconUrl = "ri:settings-3-line",
            OrderNum = 100,
            ParentId = EquipmentResourceManagementApplication.Id
        };

        // 房间标签
        public static Menu LocationLabel = new()
        {
            Id = Guid.Parse("A0F66616-B831-0768-3054-F00B1D9A296B"),
            Name = "房间标签",
            Code = "location_label",
            ScopeCode = "system:label:list",
            Type = MenuType.Menu,
            IconUrl = "ep:location-information",
            Route = "/location/label/index",
            OrderNum = 1000,
            ParentId = LabelManagement.Id,
            RouteName = "location_label"
        };

        // 设备发卡
        public static Menu EquipLabelManage = new()
        {
            Id = Guid.Parse("a2fa081e-c8ed-4382-bab7-556f8e3f6476"),
            Name = "设备发卡",
            Code = "equipledger_label",
            ScopeCode = "equip:ledger-label:list",
            Type = MenuType.Menu,
            IconUrl = "ep:location-information",
            Route = "equip/ledger-label/index",
            OrderNum = 1001,
            ParentId = LabelManagement.Id,
            RouteName = "equipledger_label"
        };

        // 设备标签
        public static Menu EquipLabel = new()
        {
            Id = Guid.Parse("b3fa081e-c8ed-4382-bab7-556f8e3f6476"),
            Name = "设备标签",
            Code = "equip_label",
            ScopeCode = "equip:label:list",
            Type = MenuType.Menu,
            IconUrl = "ep:location-information",
            Route = "equip/label/index",
            OrderNum = 1001,
            ParentId = LabelManagement.Id,
            RouteName = "equip_label"
        };

        #endregion

        #region 定位管理

        // 定位管理
        public static Menu OrientationManagement = new()
        {
            Id = Guid.Parse("cea13e82-ec36-4037-9d33-cc88eb90442d"),
            Name = "定位管理",
            Code = "orientationManagement",
            Type = MenuType.Catalogue,
            Route = "/orientationManagement",
            IconUrl = "ri:settings-3-line",
            OrderNum = 101,
            ParentId = EquipmentResourceManagementApplication.Id
        };

        // 手持枪历史
        public static Menu EquipLedgerHistory = new()
        {
            Id = Guid.Parse("B46B7ECC-E9B9-4AE4-5B67-6902BBCE1B8B"),
            Name = "手持枪历史",
            Code = "equipLedgerHistory",
            ScopeCode = "equip:equipledgerhistory:list",
            Type = MenuType.Menu,
            IconUrl = "fa-solid:history",
            Route = "/equip/history/index",
            OrderNum = 1000,
            ParentId = OrientationManagement.Id,
            RouteName = "EquipLedgerHistory"
        };

        // rfid跟踪历史
        public static Menu EquipLocationRecord = new()
        {
            Id = Guid.Parse("0198A132-F20A-73D9-8251-F6FFDB834650"),
            Name = "Rfid跟踪历史",
            Code = "equipLocationRecord",
            ScopeCode = "equip:equipLocationRecord:list",
            Type = MenuType.Menu,
            IconUrl = "fa-solid:history",
            Route = "/equip/locationRecord/index",
            OrderNum = 1000,
            ParentId = OrientationManagement.Id,
            RouteName = "EquipLocationRecord"
        };

        // 设备定位
        public static Menu EquipOrientation = new()
        {
            Id = Guid.Parse("6e0801e3-e866-436e-b0ff-f47845a3ff8d"),
            Name = "设备定位",
            Code = "equipOrientation",
            ScopeCode = "equip:equipledgerhistory:list",
            Type = MenuType.Menu,
            IconUrl = "fa-solid:history",
            Route = "/equip/history/index",
            OrderNum = 1001,
            ParentId = OrientationManagement.Id,
            RouteName = "equiporientation"
        };

        // 设备资源可视化
        public static Menu EquipVisualization = new()
        {
            Id = Guid.Parse("769332a0-16ba-47a6-9d1a-b36d14093863"),
            Name = "设备资源可视化",
            Code = "equipvisualization",
            ScopeCode = "equip:equipledgerhistory:list",
            Type = MenuType.Menu,
            IconUrl = "fa-solid:history",
            Route = "/equip/history/index",
            OrderNum = 1002,
            ParentId = OrientationManagement.Id,
            RouteName = "equipvisualization"
        };

        #endregion

        public static Menu[] Seeds { get; } = [
            Root,
            BasisPlatform,
            EquipmentResourceManagementApplication,
            System,
            Monitoring,
            // Online,
            User, UserQuery, UserAdd, UserEdit, UserRemove, UserResetPwd,
            Role, RoleQuery, RoleAdd, RoleEdit, RoleRemove,
            MenuRoot, MenuQuery, MenuAdd, MenuEdit, MenuRemove,
            // Dept, DeptQuery, DeptAdd, DeptEdit, DeptRemove,
            // Post, PostQuery, PostAdd, PostEdit, PostRemove,
            OperationLog, OperationLogQuery, OperationLogRemove,
            LoginLog, LoginLogQuery, LoginLogRemove,
            Dict, DictQuery, DictAdd, DictEdit, DictRemove,
            //Notice, NoticeQuery, NoticeAdd, NoticeEdit, NoticeRemove,
            // Config, ConfigQuery, ConfigAdd, ConfigEdit, ConfigRemove,
            SysCode,SysConfig,
            //MainData, UnitManage, CustomerManage, SupplierManage,
            //Warehouse, WarehouseSet, WarehouseStock, ProcureWarehouse, SupplierReturn,
            //ProductRequire, ProductReturn, ProductWarehouse, SalesOut, SalesReturn,
            Equip, EquipType, EquipLedger, EquipAdd, EquipEdit, EquipQuery, EquipRemove, EquipLabelManage, EquipNotice,EquipTestAnalyse, EquipTestAnalyseOutput,TestData,
            OrientationManagement, EquipLedgerHistory, EquipLocationRecord, EquipOrientation, EquipVisualization,
            LabelManagement, EquipLabel,
            DataManagement,EquipRkServer,
            //EquipItems, EquipPlan, EquipPlanDone,EquipRepair, EquipPlanRecord,
            EquipConnectManagement, EquipConnectAdd, EquipConnectQuery, EquipConnectEdit, EquipConnectRemove, // EquipDataConfig,
            //Product, ProductPlan, ProductOrder, ProductSchedule,
            //Quality, QualityItem, QualityInput, QualityProcess, QualityOutput,
            //Schedule, ScheduleTeam, SchedulePlan, ScheduleHoliday, ScheduleCalendar,
            Location,Building,Floor,Room,
            ShowSystem1, ShowSystemTemp, //ShowSystem,ShowSystem2,
            Basic,// Supplier,Unit,Customer,
            LocationLabel,
            APIRoot, APIQuery, APIList, APIAdd, APIRemove, APIEdit, APISystem, APIEquip, APIBasic,
        ];

        #endregion
    }
}