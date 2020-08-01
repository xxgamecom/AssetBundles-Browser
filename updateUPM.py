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

def updateUPM(packageName: str, tag: str):
    os.system("git checkout -f master")
    shutil.move("Assets/{0}".format(packageName), ".git/")
    
    os.system("git checkout -f upm")
    os.system("git reset --hard")
    os.system("git clean -fd")
    os.system("git rm -rf --ignore-unmatch *")

    for d in os.listdir(".git/{0}/".format(packageName)):
        shutil.move(".git/AssetBundles-Browser/" + d, "./")
    shutil.rmtree(".git/AssetBundles-Browser/")

    os.system("git add -A")
    os.system("git commit -m 'update upm to {0}'".format(tag))
    os.system("git tag {0}".format(tag))
    os.system("git push origin upm --tags")

if __name__ == "__main__":
    tag = "1.8.5"
    packageName = "AssetBundles-Browser"
    # updateUPM(packageName,tag)
    pass
