## MeasureApp  

![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)

---

MeasureApp 是一款功能通用的桌面测量与测试应用程序，适用于 Windows 平台。它采用 WPF 和 MVVM 架构构建，旨在连接各类硬件仪器，以实现测试自动化、数据采集、可视化及分析。

该应用的核心通信能力由 **CarrotLink.NET** 框架提供支持。CarrotLink.NET 为通过不同物理接口（如串口、GPIB、USB）和协议与设备交互提供了灵活且可扩展的基础。

## 主要功能

MeasureApp 为实验室和生产测试环境提供了一套全面的工具集：

*   **设备连接与控制:**
    *   可同时管理与多个硬件设备的连接。
    *   内置对标准接口的支持，如串口和 NI-VISA (GPIB, USB-TMC 等)。
    *   包含针对特定仪器（如是德/安捷伦 3458A 数字万用表）的专用控制面板和逻辑。

*   **测试自动化脚本引擎:**
    *   一个强大且易于使用的脚本引擎，通过类 XML 标签来定义测试序列。它支持：
        *   直接的仪器命令 (`<measure>`, `<delay>`)。
        *   循环 (`<for>`) 和条件 (`<if>`) 等控制流逻辑。
        *   变量和动态表达式求值 (`<env key="..." value="{i+1}"/>`)。

*   **数据采集与存储:**
    *   一个集中的 `DataLogService` 服务，用于从脚本命令、仪器读数和协议流等多种来源捕获数据。
    *   数据被组织到用户定义的通道或“键”中，便于管理。
    *   支持将数据集保存为 JSON 文件或从 JSON 文件加载。

*   **数据可视化:**
    *   集成了 ScottPlot 库，用于高性能的实时数据绘图和可视化。

*   **寄存器映射编辑器:**
    *   一个用于底层硬件交互的图形用户界面。
    *   允许用户定义寄存器映射，并对单个寄存器及特定的位域进行读写操作。

*   **调试工具:**
    *   包含一个设备调试视图，用于向连接的仪器发送原始命令并查看其响应，辅助脚本开发和问题排查。
    *   为应用程序事件和通信数据流提供全面的日志记录。

## 项目结构

项目遵循 MVVM (Model-View-ViewModel) 设计模式构建：

*   **View:** 包含所有与 UI 相关的文件（`.xaml` 和 `.xaml.cs`），定义了应用程序的布局和外观。
*   **ViewModel:** 连接 View 和 Model 的表示逻辑层。它管理应用程序的状态，并向 UI 暴露数据和命令。
*   **Model:** 代表应用程序的数据和业务逻辑，包括仪器通信、数据结构和分析算法。
*   **Services:** 一个关键的层次，封装了核心功能，如设备管理、日志记录、配置管理和脚本执行引擎。这些服务通过依赖注入（DryIoc）进行管理。

**关于 `V1` 目录的重要说明:**
项目结构中存在的多个 `V1` 目录（例如 `MeasureApp/Model/V1`, `MeasureApp/ViewModel/V1`）包含了应用早期版本的遗留代码。这些旧版功能包括 **基于Roslyn的动态C#脚本执行** 和 **FFT（快速傅里叶变换）频谱分析** 等。虽然这些代码仍存在于代码树中，但它们在很大程度上已被弃用，并未被当前的模块化架构所使用。现代化的实现主要位于顶层的 `ViewModel`、`View` 和 `Services` 目录中。

## 核心技术

*   **框架:** .NET / WPF
*   **架构:** MVVM (使用 CommunityToolkit.Mvvm)
*   **通信:** CarrotLink.NET
*   **绘图:** ScottPlot
*   **依赖注入:** DryIoc
*   **脚本编辑器:** AvalonEdit
*   **硬件驱动:** NI-VISA, FTD2XX_NET (通过 CarrotLink.NET)

## 构建

1. **克隆仓库**
   ```bash
   git clone https://github.com/CRThu/MeasureApp.git 
   git clone https://github.com/CRThu/CarrotLink.NET.git
   ```

2. **使用Visual Studio打开解决方案**
   - 打开`MeasureApp.sln`文件
   - 设置`MeasureApp`为启动项目

3. **生成并运行**

## 快速开始

- [下载最新Release包](https://github.com/CRThu/MeasureApp/releases/)
- 可能需要安装NI4882_1550f0、FT2232等驱动才可正常运行某些功能 

## 许可证

Apache License 2.0

---
*本README由 Gemini 2.5 Pro 生成*
