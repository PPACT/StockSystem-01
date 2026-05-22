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

### 管理员初始密码（重要）

密码取决于运行环境，**Docker 和本地开发不同**：

| 场景 | 密码来源 | 密码值 |
|------|----------|--------|
| Docker | `.env` → `ADMIN_PASSWORD` | `Admin@2026`（默认值，可修改） |
| VS / 本地开发 | 无环境变量 → **随机生成 16 位 GUID** | 首次启动时控制台打印，请注意查看 |

代码逻辑（`Data/DbInitializer.cs`）：

```csharp
string adminPassword = Environment.GetEnvironmentVariable("STOCK_ADMIN_PASSWORD");
if (string.IsNullOrEmpty(adminPassword))
{
    adminPassword = Guid.NewGuid().ToString("N")[..16];  // 随机生成
    Console.WriteLine("随机管理员密码: " + adminPassword);
}
```

> **本地开发时**：首次运行 `dotnet run` 后请留意控制台输出，密码仅打印一次，不会写入任何配置文件。如需固定密码，在系统环境变量中设置 `STOCK_ADMIN_PASSWORD`。

## Docker 部署

```bash
cd D:\Code\Test
docker compose up -d
```

### 架构图

```
                 stock-net (bridge)
┌──────────────┬─────────────────┬──────────────────┐
│  sqlserver   │    backend      │    frontend      │
│  :1433       │    :5000        │    :80           │
│  SQL 2022    │    .NET 10      │    Nginx + Vue3  │
└──────────────┴─────────────────┴──────────────────┘
```

### 访问地址

| 服务 | 地址 | 说明 |
|------|------|------|
| 前端页面 | http://localhost:80 | Nginx 代理 `/api` 到 backend |
| 后端 API | http://localhost:5000 | 直接访问 API |
| Swagger 文档 | http://localhost:5000/swagger | 开发模式可用 |

### 端口配置

主机端口通过 `.env` 文件控制，默认值：

```
SQLSERVER_PORT=1433    # 数据库
BACKEND_PORT=5000      # 后端 API
FRONTEND_PORT=80       # 前端页面
```

**端口被占用？** 修改 `.env` 中对应端口即可，例如其他机器上 80 被占用时：

```
FRONTEND_PORT=8080
```

重启后访问 `http://localhost:8080`。容器内部端口不变，只改主机映射端口，服务间通信不受影响。`docker compose up -d` 会自动读取 `.env` 配置。

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

| 变量 | 默认值 | 作用 |
|------|--------|------|
| `SQLSERVER_PORT` | `1433` | 数据库主机映射端口 |
| `BACKEND_PORT` | `5000` | 后端 API 主机映射端口 |
| `FRONTEND_PORT` | `80` | 前端页面主机映射端口 |
| `SA_PASSWORD` | `Dev@123456` | SQL Server SA 密码 |
| `JWT_SECRET` | `StockSystem-...` | JWT 签名密钥（≥16 字符） |
| `ADMIN_PASSWORD` | `Admin@2026` | 初始 admin 登录密码 |

> 生产环境务必修改 `SA_PASSWORD`、`JWT_SECRET`、`ADMIN_PASSWORD`。端口按需调整。

## 安全措施

- 密码使用 BCrypt 哈希存储
- JWT 令牌认证 + 前端路由守卫
- `.gitignore` 排除 `.env`、`appsettings.Development.json`、`secrets.*`
- admin 账户保护（不可删除）
