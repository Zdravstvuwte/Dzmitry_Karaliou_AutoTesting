Пункт 4 задания (Test Runners): артефакты для сравнения NUnit и xUnit
================================================================

Как получить файлы
------------------
Из корня репозитория выполните в PowerShell:

  powershell -ExecutionPolicy Bypass -File scripts\Generate-ComparisonArtifacts.ps1

  (или pwsh, если установлен PowerShell 7+)

Папка с результатом создаётся здесь:

  artifacts/output/<дата_время>/

Внутри будут:
  nunit-results.trx   — отчёт VSTest для проекта NUnit
  xunit-results.trx   — отчёт VSTest для проекта xUnit
  nunit-console.log   — вывод консоли при прогоне NUnit
  xunit-console.log   — вывод консоли при прогоне xUnit
  summary.txt         — длительность прогона и коды выхода

Только категория Smoke (пример):

  powershell -ExecutionPolicy Bypass -File scripts\Generate-ComparisonArtifacts.ps1 -Filter Smoke

Папка artifacts/output/ указана в .gitignore — при необходимости приложите
сгенерированную подпапку к отчёту (zip) вручную.

Что сравнивать
--------------
1. TRX: одинаковое ли число пройденных/упавших тестов, имена тест-кейсов.
2. Длительность (summary.txt и заголовки в логах) при одинаковом фильтре.
3. Формат сообщений об ошибках и порядок выполнения в console.log.

Требуются установленные .NET SDK, Chrome и доступ в интернет (Selenium).
