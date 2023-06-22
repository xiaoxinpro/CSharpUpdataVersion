# CSharpUpdataVersion
用于C#项目更新版本号的程序

## 使用说明

### 下载软件

- 开发版本：[CSharpUpdataVersion.exe](https://github.com/xiaoxinpro/CSharpUpdataVersion/blob/master/bin/Debug/CSharpUpdataVersion.exe)
- 稳定版本：[Releases](https://github.com/xiaoxinpro/CSharpUpdataVersion/releases)

### 部署软件

将下载的软件放到项目的根目录下

### 自动更新版本配置

> 目标项目->属性->生成事件->生成前事件命令行

输入命令：

```
start CSharpUpdataVersion.exe $(ProjectDir)\Properties\AssemblyInfo.cs
```

### 多项目同步版本配置

> 目标项目->属性->生成事件->生成前事件命令行

输入命令

```
start CSharpUpdataVersion.exe $(SolutionDir)\源项目\Properties\AssemblyInfo.cs $(SolutionDir)\目标项目\Properties\AssemblyInfo.cs
```
