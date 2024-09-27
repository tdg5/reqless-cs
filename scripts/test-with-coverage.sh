#!/bin/bash

rm -rf Reqless.Tests/TestResults/ Reqless.Worker.Tests/TestResults/

dotnet test --collect:"XPlat Code Coverage"

reportgenerator \
  -reports:'Reqless.Tests/TestResults/**/coverage.cobertura.xml;Reqless.Worker.Tests/TestResults/**/coverage.cobertura.xml' \
  -reporttypes:Html \
  -targetdir:"coveragereport"
