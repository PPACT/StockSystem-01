# StockSystem 库存管理系统

基于 .NET 10 + Vue 3 的全栈库存管理系统，支持物料管理、出入库操作、库存日志审计、用户管理。

## 技术栈

| 层 | 技术 |
|----|------|
| 后端 | .NET 10 Web API, EF Core 10, JWT Bearer, BCrypt, Serilog, NPOI |
| 前端 | Vue 3 (Composition API), Vite, Element Plus, Pinia, Vue Router, Axios |
| 数据库 | SQL Server (LocalDB 本地 / Docker SQL Server 2022) |
| 部署 | Docker Compose 三容器编排 (Nginx + .NET + SQL Server) |

## 项目结构

```
StockSystem/                     # 后端
├── Controllers/                 # API 控制器
│   ├── LoginController.cs       # 登录认证
│   ├── MaterialsController.cs   # 物料 CRUD + Excel 导入导出
│   ├── StockController.cs       # 出入库操作 + 日志查询
│   └── UsersController.cs       # 用户管理
├── Services/                    # 业务逻辑层
├── Repositories/                # 数据访问层
├── Models/                      # 实体模型 (Material, User, StockLog)
├── Data/                        # DbContext + 种子数据
├── Common/                      # ApiResult, JwtHelper, GlobalExceptionFilter
└── Migrations/                  # EF Core 数据迁移

StockSystemWeb/                  # 前端
└── src/
    ├── views/                   # 7 个页面组件
    ├── router/                  # 路由 + 鉴权守卫
    ├── store/                   # Pinia 状态管理
    └── utils/                   # Axios 实例 (JWT 拦截)
```

## API 接口

所有接口返回统一格式 `{ code, msg, data }`。

### 登录（公开）
| 方法 | 路由 | 说明 |
|------|------|------|
| POST | `/api/Login` | 登录，返回 JWT token |

### 物料管理（需认证）
| 方法 | 路由 | 说明 |
|------|------|------|
| GET | `/api/Materials/page` | 分页查询（支持名称/编码筛选） |
| POST | `/api/Materials` | 新增物料 |
| PUT | `/api/Materials/{id}` | 编辑物料 |
| DELETE | `/api/Materials/{id}` | 删除物料 |
| GET | `/api/Materials/Export` | 导出 Excel |
| POST | `/api/Materials/Import` | 导入 Excel |

### 出入库（需认证）
| 方法 | 路由 | 说明 |
|------|------|------|
| POST | `/api/Stock/In` | 入库（事务保护） |
| POST | `/api/Stock/Out` | 出库（事务保护） |
| GET | `/api/Stock/LogList` | 库存日志分页查询 |

### 用户管理（需认证）
| 方法 | 路由 | 说明 |
|------|------|------|
| GET | `/api/Users/page` | 用户分页查询 |
| POST | `/api/Users` | 新增用户 |
| PUT | `/api/Users/{id}` | 修改角色 |
| DELETE | `/api/Users/{id}` | 删除用户（admin 保护） |
| PUT | `/api/Users/MyProfile` | 修改个人信息 |

## 数据库

三张核心表：

```
Materials  ──┬── StockLogs
(id, name,   │   (id, materialId, operateType,
 code,       │    changeCount, beforeStock,
 stockNumber,│    afterStock, operateUser, operateTime)
 remark)     │
             │
Users ───────┘
(id, username, password, role)
```

启动时自动建表并创建 admin 用户。

## Docker 部署

```bash
cd D:\Code\Test
docker compose up -d
```

```
                 stock-net (bridge)
┌──────────────┬─────────────────┬──────────────────┐
│  sqlserver   │    backend      │    frontend      │
│  :1433       │    :5000        │    :80           │
│  SQL 2022    │    .NET 10      │    Nginx + Vue3  │
└──────────────┴─────────────────┴──────────────────┘
       ↑               ↑                   ↑
    localhost:1433  localhost:5000    localhost:80
```

| 服务 | 端口 | 环境变量 |
|------|------|----------|
| sqlserver | 1433 | `MSSQL_SA_PASSWORD` |
| backend | 5000 | `CONNECTION_STRING`, `JWT_SECRET`, `STOCK_ADMIN_PASSWORD` |
| frontend | 80 | 无（Nginx 代理 /api → backend） |

访问地址：http://localhost

## 本地开发

```bash
# 后端
cd StockSystem
dotnet run

# 前端
cd StockSystemWeb
npm install
npm run dev
```

本地开发需要在 `appsettings.Development.json` 配置 JWT 密钥：

```json
{
  "Jwt": {
    "SecretKey": "至少32位的随机字符串"
  }
}
```

> 该文件已在 `.gitignore` 中，不会被提交到仓库。

## 环境变量

| 变量 | 作用 | 场景 |
|------|------|------|
| `CONNECTION_STRING` | 数据库连接串 | Docker 必需 |
| `JWT_SECRET` | JWT 签名密钥（≥16 字符） | 生产/Docker 必需 |
| `STOCK_ADMIN_PASSWORD` | 初始 admin 密码 | Docker 推荐，未设则随机生成 |

## 安全措施

- 密码使用 BCrypt 哈希存储
- JWT 令牌认证 + 前端路由守卫
- `.gitignore` 排除 `.env`、`appsettings.Development.json`、`secrets.*`
- admin 账户保护（不可删除）
