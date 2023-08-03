@echo off

::cd into the folder
cd /d %GAME_PROJECT_OUTPUTGITFOLDER%

::写入最新版本号到Versions.txt中
echo %~2 > Versions.txt
::将所有改动添加
git add .
::提交 后续跟上一个注释，这个注释是调起 bat 的第一个参数
git commit -a -m %~1
::设置 标签内容
git tag -a %~2 -m %~3
::强制推送本地版本到远程，作为最新版，忽略其他，因为内容的变化基于Excel，所以此处不处理冲突
git push origin --force

exit 0
