@echo off

::主要用于组装json文件
del package.json
echo { >> package.json
echo    "displayName": "_GameConfigs", >> package.json
echo    "name": "com.wb.gameconfigs", >> package.json
echo    "version": "%~1", >> package.json
echo    "unity": "2021.3", >> package.json
echo    "description": "包含了项目中的最新配置文件集合", >> package.json
echo    "keywords": [   >> package.json
echo        "wenbin", >> package.json
echo        "gameconfigs" >> package.json
echo    ], >> package.json
echo    "category": "Unity", >> package.json
echo    "samples": [ >> package.json
echo        { >> package.json      
echo            "displayName": "ConfigUpdatePanelPackage", >> package.json
echo            "description": "配置更新面板插件", >> package.json
echo            "path": "Samples~/UnityTool" >> package.json
echo        } >> package.json
echo    ] >> package.json
echo } >> package.json
