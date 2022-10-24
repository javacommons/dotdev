set -uvx
set -e
cwd=`pwd`
archive () {
	cd $cwd/../dotdev.template/$1
	git archive HEAD -o $cwd/$1.zip
	cd $cwd
}
archive CUI
archive GUI
archive WPF
archive LIB
cd $cwd
rm -rf obj bin
dotnet build -c Release
rm -rf ./dotdev.exe
ilmerge -wildcards -out:dotdev.exe bin/Release/net472/*.exe bin/Release/net472/*.dll

