@ECHO OFF
rem switch to master
git checkout -f master
rem  # Snapshot of the files
move Assets/AssetBundles-Browser .git/
rem switch to upm branch
git checkout -f upm
git reset --hard
git clean -fd
git rm -rf --ignore-unmatch *
rem move /y .git/AssetBundles-Browser/*.* ./  => 移动目录下所有文件到当前目录
git add -A
git commit -m "update upm to x.x.x"
git tag "x.x.x"
git push origin upm --tags