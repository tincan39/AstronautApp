To run the tests and generate results for code coverage, navigate to the StargateTestBed directory. Then run this command:

```
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
```

This is how you display the code coverage results in a readable format. Note: You must delete all prior test results and coverge reports
```
reportgenerator -reports:"StargateTestBed\TestResults\*\coverage.cobertura.xml" -targetdir:"coverage-report" -reporttypes:Html
```

Open the index.html file generated to read the results