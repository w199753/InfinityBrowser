# InfinityEngine
InfinityEngine是一个现代化思想的基于.Net 5以提供高性能和易使用的渲染引擎架构。其中会涉及到ECS的模块并利用其高效的特性组织渲染数据，以及多线程渲染和RHI线程并行准备渲染数据与提交GPU。为了能够最大化并行CPU和GPU渲染器本身设计设计了一套RenderGraph以及现代化的RHI和TaskSystem。RHI以SRP类似风格的思想来提供HighLevel编程体验，RDG在此之上能够非常方便的进行每个Pass的Parallel CommandExecute也正是借助了TaskSystem来实现。TaskSystem本身是为了弥补C#本身的Task不能分发给特定功能线程，不能在其他线程Dispatch，不能嵌套Dispatch的缺陷。
