# CodeGeneratorOpenness
Siemens TIA Portal Code Generator via Openness Interface

Since we are doing import of Graph step sequence through an Excel sheet using Openness, I was thinking of building a "structure" code generator for the TIA portal. Right now, most important functions are working so it would be possible to
<br>
-build a group tree<br>
-import any kind of blocks like FB, FC, OB or data types<br>
-generate step sequences as graph (not 100% complete yet) - no config right now

This covers mainly the function we can find in the Openness scripter. But I want to go a little further to build structures more variable. Not sure where it leads to but my goal will be to use templates to build FB with n-times valve FBs through a scripter or later on maybe a tool to configure this.

Based on the description found here:

https://cache.industry.siemens.com/dl/files/886/109826886/att_1163875/v1/TIAPortalOpenness_enUS_en-US.pdf



This version is based on TIA V19.

## 安装依赖
- 安装 .NET Framework 4.8 Developer Pack
- 安装 Visual Studio Build Tools https://visualstudio.microsoft.com/visual-cpp-build-tools/
- 安装 Siemens TIA Portal V19
## 添加权限到openness
1. 将该电脑权限添加到openness用户组
2. 重启电脑使其生效
## 编译命令
先执行该命令编译
```
& "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe" CodeGeneratorOpenness\CodeGeneratorOpenness.csproj /p:Configuration=Debug /p:Platform=AnyCPU 
```
后续可以用下面命令编译
```
dotnet build CodeGeneratorOpenness.sln
```


## 不同版本编译需要修改的地方
CodeGeneratorOpenness\CodeGeneratorOpenness.csproj：
```
 <Reference Include="Siemens.Engineering, Version=19.0.0.0, Culture=neutral, PublicKeyToken=d29ec89bac048f84, processorArchitecture=MSIL">
      <SpecificVersion>False</SpecificVersion>
      <HintPath>C:\Program Files\Siemens\Automation\Portal V19\PublicAPI\V19\Siemens.Engineering.dll</HintPath>
    </Reference>
```
Program.cs:
 位置: foreach (string n in names)
                {
```
if (n.Contains("TIA Portal V19"))
                    {
                        Version = "19.0";
                        Api = @"C:\Program Files\Siemens\Automation\Portal V19\PublicAPI\V19";
                    }
```
frmMainForm.cs:
位置：
using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
```
 string filter = "V19 project files (*.ap19)|*.ap19|All files (*.*)|*.*";
                        if (Program.Version == "18.0") filter = "V18 project files (*.ap18)|*.ap18|All files (*.*)|*.*";
                        if (Program.Version == "17.0") filter = "V17 project files (*.ap17)|*.ap17|All files (*.*)|*.*";

```


