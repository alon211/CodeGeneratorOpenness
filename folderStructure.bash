CodeGeneratorOpenness/
├── Sample/                         # 示例程序入口/测试工具
│   └── Program.cs                 # 控制台主入口（可多项目调用）
│
├── Config/                         # ✅ 所有项目的结构配置都放这里
│   ├── projectConfig.json         # 🌟 项目索引（用于映射项目 → 配置路径）
│   ├── Projects/                  # 多项目结构配置（结构定义）
│   │   ├── Project1/
│   │   │   ├── Unit1_structure.json
│   │   │   ├── Unit2_structure.json
│   │   └── ProjectN/
│   │       └── ...
│   └── Params/                    # 每项目参数（配方变量、实例化参数等）
│       ├── Project1.params.json
│       └── ...
│
├── Template/                      # 💡 所有 SCL 模板源码（占位符结构）
│   ├── FB_Template.scl
│   ├── DB_Template.scl
│   └── ...
│
├── Generator/                     # 🔁 每次生成的实际代码（模板 + 配置生成）
│   ├── Project1/
│   │   ├── Sources/
│   │   ├── Types/
│   │   └── Blocks/
│   └── ProjectN/
│
├── Types/
│   ├── SystemTypes/              # Siemens 系统 UDT 模板（官方格式）
│   ├── UserTypes/                # ✅ 用户自建变量类型（优先导入）
│
├── UserLibrary/                  # ✅ 用户提供的常用功能模块库（带注释 FB/FC 等）
│   └── Project1/
│       └── SomeFB.scl
│
├── Output/                       # 📦 生成导出（中间件、文档、Log 等）
│   ├── Log/
│   ├── Result/
│   └── Reports/
├── Data/
│   ├── codegen.db
├── Services/                      # ✅ 数据服务层（数据库访问/日志接口）
│   ├── DbContext.cs               # SQLite 初始化/封装
│   ├── ILogService.cs             # 自定义日志接口（用于注入）
│   └── Models/
│       ├── Project.cs
│       └── Block.cs
├── GUI/                           # ✅ 图形界面（可选 WPF 或 WinForms）
│   ├── App.xaml.cs
│   └── MainWindow.xaml
├── Core/                          # ✅ 核心功能模块（模板替换/结构解析）
│   ├── CodeGenerator.cs           # 执行模板生成逻辑
│   ├── TemplateEngine.cs          # ✅ 使用 RazorLight/Scriban 实现
│   ├── StructureParser.cs         # 解析结构 JSON
│   └── Importer.cs                # 生成后导入 TIA 工程（Openness 调用）
├── Tests/                        # 单元测试项目
│   └── ConfigTests.cs
│   └── TemplateEngineTests.cs
│   └── GeneratorTests.cs
├── Logging/                       # ✅ 日志模块封装（Serilog/NLog 配置）
│   ├── SerilogConfigurator.cs     # Serilog 配置类
│   ├── LogSink.cs                 # 自定义输出（写入 Log/）
│   └── logging.json               # 配置文件（格式如 Serilog）
