1、avidInSchedule 将专注于视频mxf的调度。剥离文稿的调度与处理；
avidinconfig.xml 增加节点 <ifGetLongTasks>1</ifGetLongTasks> 用于判断是否下发长时间的任务
系统配置中增加 longTaskTime 用于配置长任务的界定 00:20:00:00
判断avid ingest 是否离线 在xml的目录中写入一个特殊的  avidingest01.live 文件  用来告诉调度程序它还活着
超过1分钟没有响应就认为该avidingest 死机了 就不会给他发认为
2 1.0.0.3 修改删除的方式，改成循环删除最多重试10次 2017-01-06