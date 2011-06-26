#!/bin/bash

set -e -x

wd=${BUILD_LANE_MAX_REVISION//\//_}

cd /home/mico/monkeywrench/sources/$wd

make check

