#!/bin/bash

set -e -x

wd=${BUILD_LANE_MAX_REVISION//\//_}

cd /home/mico/monkeywrench/sources/$wd

./autogen.sh --prefix=/home/mico/monkeywrench/$wd --enable-dependency-tracking --enable-nunit-tests
make -j 5

