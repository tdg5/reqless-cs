#!/bin/bash

rm -rf Reqless.Tests/TestResults/ Reqless.Extensions.Hosting.Tests/TestResults/

dotnet test --collect:"XPlat Code Coverage"

reportgenerator \
  -reports:'Reqless.Tests/TestResults/**/coverage.cobertura.xml;Reqless.Extensions.Hosting.Tests/TestResults/**/coverage.cobertura.xml' \
  -reporttypes:Html \
  -targetdir:"coveragereport"
