@echo off

::cd into the folder
cd /d %GAME_PROJECT_OUTPUTGITFOLDER%

::更新版本号什么的内容，放到远端，读取远端文件内容，看看是否是最新版本，同时查看本地hash码是否一致，不一致表示有更新
::看是否需要列出所有版本号，并且保存其hash码，用于切换到对应hash码的提交状态
::目前确定的是 往git库中写入版本号，并提交生成文件，并且提交，提交后进行推送并且打上tag版本号
::unity这块提供检测功能，检测版本号信息，同时对比最后一条的两个信息，看看是否有更新，如果有更新，那就提示是否确定更新，确定再进行处理。

::将所有改动添加
echo git add
::提交
echo git commit
::
echo git tag -a $~1 -m $~2
echo git push origin --force
