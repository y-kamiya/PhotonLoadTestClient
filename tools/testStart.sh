#!/bin/bash

# you can use this script to deploy and start load test in linux(x86_64) servers
# set the varialbe `targets`

# servers to deploy including target directory (scp target style)
# ex. targets=("hoge@hoge.jp:fuga" "ubuntu@10.1.2.14:" ...)
targets=()

# path to executable file (except file extension)
source_path=$1

# time of execution
timer=$2

# optional.
# pass 1 if you want to deploy executable file
doScp=${3:-0}

filename=`basename $source_path`

if [ $doScp -eq 1 ]; then
    for target in ${targets[@]}
    do
        scp -r ${source_path}* $target
    done
fi

for target in ${targets[@]}
do
    server=`echo $target | cut -d: -f1`
    dir=`echo $target | cut -d: -f2`
    ssh $server "echo; echo SSH to \"$server\"; rm -rf ${dir}/${filename}_Data/LoadTestLog; timeout $timer ./${dir}/${filename}.x86_64" &
done

wait
echo "all process finished"
