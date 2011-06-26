#!/bin/bash

set -e -x

wd=${BUILD_LANE_MAX_REVISION//\//_}

mkdir -p /home/mico/monkeywrench/sources
cd /home/mico/monkeywrench/sources

git clone --reference upstream.git $BUILD_REPOSITORY $wd

cd $wd

git reset --hard $BUILD_REVISION
