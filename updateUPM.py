#!/usr/bin/env python
# -*- encoding: utf-8 -*-
"""
@File    :   updateUPM.py
@Time    :   2020/08/01 14:13:54
@Author  :   JunQiang
@Contact :   354888562@qq.com
@Desc    :   
"""

# here put the import lib
import os
import shutil


def updateUPM():
    os.system("git checkout -f master")
    shutil.move("Assets/AssetBundles-Browser",".git/")
    os.system("git checkout -f upm")
    os.system("git reset --hard")
    os.system("git clean -fd")
    os.system("git rm -rf --ignore-unmatch *")
    shutil.move(".git/AssetBundles-Browser/","./")
    pass

if __name__ == "__main__":
    updateUPM()
    pass