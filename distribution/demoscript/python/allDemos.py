#!/usr/bin/env python
# **********************************************************************
#
# Copyright (c) 2003-2015 ZeroC, Inc. All rights reserved.
#
# This copy of Ice is licensed to you under the terms described in the
# ICE_LICENSE file included in this distribution.
#
# **********************************************************************

import os, sys

for toplevel in [".", "..", "../..", "../../..", "../../../.."]:
    toplevel = os.path.abspath(toplevel)
    if os.path.exists(os.path.join(toplevel, "demoscript")):
        break
else:
    raise RutimeError("can't find toplevel directory!")

sys.path.append(os.path.join(toplevel))
from demoscript import Util

#
# List of all basic demos.
#
demos = [
    "Ice/async",
    "Ice/bidir",
    "Ice/callback",
    "Ice/context",
    "Ice/converter",
    "Ice/hello",
    "Ice/latency",
    "Ice/minimal",
    "Ice/metrics",
    "Ice/session",
    "Ice/throughput",
    "Ice/value",
    "IceDiscovery/hello",
    "Manual/printer",
    "Manual/simpleFilesystem"
]

#
# In Windows service demos only run when using a bin dist,
# we don't build Ice services with VC100
#
if not Util.isWin32() or Util.isBinDist():
    demos += ["IceStorm/clock", "IceGrid/simple", "Glacier2/callback"]

if __name__ == "__main__":
    Util.run(demos)
