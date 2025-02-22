#!/bin/bash

cd /opt/FlightNode

pushd packages
nuget install xunit.runner.console
popd

mono packages/xunit.runner.console.2.1.0/tools/xunit.console.exe test/FlightNode.Common.UnitTests/bin/Debug/FlightNode.Common.UnitTests.dll
mono packages/xunit.runner.console.2.1.0/tools/xunit.console.exe test/FlightNode.DataCollection.UnitTests/bin/Debug/FlightNode.DataCollection.Domain.UnitTests.dll
mono packages/xunit.runner.console.2.1.0/tools/xunit.console.exe test/FlightNode.Identity.UnitTests/bin/Debug/FlightNode.Identity.UnitTests.dll
mono packages/xunit.runner.console.2.1.0/tools/xunit.console.exe test/FlightNode.Common.Api.UnitTests/bin/Debug/FligthNote.Common.Api.UnitTests.dll
