#!/bin/bash

# Find the path to this script: https://stackoverflow.com/a/246128
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null && pwd )"
echo "Seq data will be stored at '$DIR/.seqdata'"

# This runs Seq via docker, see https://docs.getseq.net/v5/docs/docker

docker run \
  -t \
  -e ACCEPT_EULA=Y \
  -v $DIR/.seqdata:/data \
  -p 5341:80 \
  datalust/seq:latest

# Now open http://localhost:5341/ in a browser...
