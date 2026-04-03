<#!
  Артефакты для сравнения NUnit и xUnit (п. 4):
  - TRX (VSTest) — открыть в Visual Studio, сравнить XML в diff-инструменте
  - nunit-console.log / xunit-console.log — вывод раннера
  - summary.txt — длительность и коды выхода

  Из корня репозитория:
    powershell -ExecutionPolicy Bypass -File scripts\Generate-ComparisonArtifacts.ps1

  Только Smoke (фильтр подставляется в формат каждого раннера):
    powershell -ExecutionPolicy Bypass -File scripts\Generate-ComparisonArtifacts.ps1 -Filter Smoke

  Явные фильтры VSTest:
    powershell -ExecutionPolicy Bypass -File scripts\Generate-ComparisonArtifacts.ps1 -NUnitFilter "TestCategory=Data" -XUnitFilter "Category=Data"
#>
param(
    [string] $Filter = "",
    [string] $NUnitFilter = "",
    [string] $XUnitFilter = ""
)

$ErrorActionPreference = "Stop"

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$sln = Join-Path $repoRoot "Web UI Automation.sln"
$nunitProj = Join-Path $repoRoot "Web UI Automation.NUnit\Web UI Automation.NUnit.csproj"
$xunitProj = Join-Path $repoRoot "Web UI Automation.xUnit\Web UI Automation.xUnit.csproj"

function Resolve-Filters {
    param([string] $Generic, [string] $Nu, [string] $Xu)

    if ($Nu -or $Xu) {
        return [pscustomobject]@{
            NUnit = $Nu
            XUnit = $Xu
        }
    }

    if (-not $Generic) {
        return [pscustomobject]@{ NUnit = ""; XUnit = "" }
    }

    if ($Generic -eq "Smoke") {
        return [pscustomobject]@{
            NUnit = "TestCategory=Smoke"
            XUnit = "Category=Smoke"
        }
    }

    if ($Generic -like "TestCategory=*") {
        $v = $Generic.Substring("TestCategory=".Length)
        return [pscustomobject]@{
            NUnit = $Generic
            XUnit = "Category=$v"
        }
    }

    if ($Generic -like "Category=*") {
        $v = $Generic.Substring("Category=".Length)
        return [pscustomobject]@{
            NUnit = "TestCategory=$v"
            XUnit = $Generic
        }
    }

    return [pscustomobject]@{ NUnit = $Generic; XUnit = $Generic }
}

function Invoke-TestProject {
    param(
        [string] $ProjectPath,
        [string] $TrxFileName,
        [string] $LogFileName,
        [string] $TestFilter,
        [string] $RunnerLabel
    )

    $trxPath = Join-Path $outDir $TrxFileName
    $logPath = Join-Path $outDir $LogFileName

    $argList = @(
        "test",
        $ProjectPath,
        "-c", "Release",
        "--no-build",
        "--logger", "console;verbosity=normal",
        "--logger", "trx;LogFileName=$trxPath"
    )

    if ($TestFilter) {
        $argList += @("--filter", $TestFilter)
    }

    Write-Host "=== $RunnerLabel ===" -ForegroundColor Cyan
    $sw = [System.Diagnostics.Stopwatch]::StartNew()
    $prevEap = $ErrorActionPreference
    $ErrorActionPreference = "Continue"
    try {
        # Вывод в консоль и в лог; в выход функции не попадаем (иначе ломается $results).
        & dotnet @argList 2>&1 | Tee-Object -FilePath $logPath | ForEach-Object { Write-Host $_ } | Out-Null
    }
    finally {
        $ErrorActionPreference = $prevEap
    }
    $sw.Stop()

    return [pscustomobject]@{
        Runner   = $RunnerLabel
        ExitCode = $LASTEXITCODE
        Seconds  = [math]::Round($sw.Elapsed.TotalSeconds, 2)
    }
}

$filters = Resolve-Filters -Generic $Filter -Nu $NUnitFilter -Xu $XUnitFilter

$stamp = Get-Date -Format "yyyyMMdd_HHmmss"
$outDir = Join-Path $repoRoot "artifacts\output\$stamp"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null

Push-Location $repoRoot
try {
    Write-Host "Сборка Release..." -ForegroundColor Yellow
    dotnet build $sln -c Release
    if ($LASTEXITCODE -ne 0) { throw "Сборка завершилась с кодом $LASTEXITCODE" }

    $results = @(
        (Invoke-TestProject -ProjectPath $nunitProj -TrxFileName "nunit-results.trx" -LogFileName "nunit-console.log" -TestFilter $filters.NUnit -RunnerLabel "NUnit")
        (Invoke-TestProject -ProjectPath $xunitProj -TrxFileName "xunit-results.trx" -LogFileName "xunit-console.log" -TestFilter $filters.XUnit -RunnerLabel "xUnit")
    )

    $summaryPath = Join-Path $outDir "summary.txt"
    @(
        "Артефакты сравнения раннеров (п. 4)",
        "Время запуска (UTC): $(Get-Date -Format 'o')",
        "Фильтр NUnit: $(if ($filters.NUnit) { $filters.NUnit } else { '(нет)' })",
        "Фильтр xUnit:  $(if ($filters.XUnit) { $filters.XUnit } else { '(нет)' })",
        "",
        ($results | ForEach-Object { "$($_.Runner): ExitCode=$($_.ExitCode), Duration_sec=$($_.Seconds)" }),
        "",
        "Файлы в этой папке:",
        "  nunit-results.trx, nunit-console.log",
        "  xunit-results.trx, xunit-console.log",
        "",
        "Сравнение: откройте оба .trx в Visual Studio Test Explorer или сравните XML;",
        "в логах — порядок тестов, сообщения, время."
    ) | Set-Content -Path $summaryPath -Encoding UTF8

    Write-Host "`nГотово: $outDir" -ForegroundColor Green
    Get-ChildItem $outDir | Format-Table Name, Length -AutoSize
}
finally {
    Pop-Location
}
