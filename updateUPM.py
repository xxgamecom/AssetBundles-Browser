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
import json


def updateUPM(packageName: str, version_tag: str):
    os.system("git checkout -f master")
    modify_packageJson("Assets/{0}/package.json".format(packageName), version_tag)
    if os.path.exists(f".git/{packageName}"):
        shutil.rmtree(f".git/{packageName}")
    shutil.move("Assets/{0}".format(packageName), ".git/")

    os.system("git checkout -f upm")
    os.system("git reset --hard")
    os.system("git clean -fd")
    os.system("git rm -rf --ignore-unmatch *")

    dir_name = ".git/{0}/".format(packageName)
    for d in os.listdir(dir_name):
        shutil.move(dir_name + d, "./")
    shutil.rmtree(dir_name)

    os.system("git add -A")
    os.system("git commit -m update_upm_to_{0}".format(version_tag))
    os.system("git tag {0}".format(version_tag))
    os.system("git push origin upm --tags")

    os.system("git checkout -f master")


def modify_packageJson(package_path: str, version_tag: str):
    json_item = ""
    with open(package_path, 'r') as fileItem:
        json_item = json.load(fileItem)
        json_item["version"] = version_tag
    with open(package_path, 'wb') as fileItem:
        fileItem.write(json.dumps(json_item, separators=(",", ":"), indent=4).encode())
        fileItem.flush()
    pass


if __name__ == "__main__":
    tag = "1.9.2"
    packageName = "AssetBundles-Browser"
    # updateUPM(packageName,tag)
    pass
