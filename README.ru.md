<a name="readme-top"></a>

[English](README.md) | **Русский**

# DuckDoku

Мобильная головоломка: клон механики Meowdoku («Queens») с
серверно-авторитетной метой на собственном бэкенде.

DuckDoku — pet-проект, в первую очередь площадка для отработки слоистой
клиент-серверной архитектуры: игровые правила живут в общей библиотеке,
которую компилируют и Unity-клиент, и ASP.NET-бэкенд, а весь прогресс и
экономика авторитетно считаются на сервере. Проект в активной
разработке — контент и механики продолжают дополняться.

[![Играть на itch.io](https://img.shields.io/badge/▶_играть-itch.io-fa5c5c?style=for-the-badge)](https://ksmk99.itch.io/duckdoku)

<details>
<summary>Оглавление</summary>

- [О игре](#о-игре)
- [Функционал](#функционал)
- [Технологии](#технологии)
- [Архитектура](#архитектура)
- [Запуск проекта](#запуск-проекта)
- [Генерация уровней](#генерация-уровней)
- [Дев-читы](#дев-читы-только-в-редакторе)

</details>

## О игре

Поле `N × N`, разбитое на `N` связных цветных областей. Нужно расставить
ровно `N` уток так, чтобы:

1. в каждой строке была ровно одна утка;
2. в каждом столбце была ровно одна утка;
3. в каждой цветной области была ровно одна утка;
4. никакие две утки не стояли в соседних клетках, включая диагональ.

Каждая головоломка сгенерирована так, что имеет ровно одно решение и
решается логикой, без перебора.

<p align="right">(<a href="#readme-top">наверх</a>)</p>

## Функционал

- Ввод двумя жестами: зажатый палец красит/стирает крестики-пометки,
  двойной тап ставит утку
- Неверная утка необратимо блокирует клетку и стоит одной из трёх
  попыток за раунд
- Карточки правил над доской — примеры прямо во время игры
- 100 уровней, сложность растёт от 6×6 до 10×10
- Подсказки — расходуемый ресурс, докупается за валюту
- Энергия с регеном по серверному времени — ограничивает число попыток
  подряд
- Гостевой аккаунт по device id, без регистрации

<p align="right">(<a href="#readme-top">наверх</a>)</p>

## Технологии

**Клиент**

[![Unity](https://img.shields.io/badge/Unity-6000.3.13f1-000000?style=flat&logo=unity&logoColor=white)](https://unity.com/)
[![Zenject](https://img.shields.io/badge/Zenject-DI-593d88?style=flat)](https://github.com/modesttree/Zenject)
[![UniTask](https://img.shields.io/badge/UniTask-async-2088FF?style=flat)](https://github.com/Cysharp/UniTask)
[![DOTween](https://img.shields.io/badge/DOTween-tweening-ff5f8f?style=flat)](https://dotween.demigiant.com/)

**Сервер**

[![.NET](https://img.shields.io/badge/.NET-10-512BD4?style=flat&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![EF Core](https://img.shields.io/badge/EF_Core-ORM-512BD4?style=flat)](https://learn.microsoft.com/ef/core)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-336791?style=flat&logo=postgresql&logoColor=white)](https://www.postgresql.org/)

<p align="right">(<a href="#readme-top">наверх</a>)</p>

## Архитектура

```mermaid
graph TD
    App --> Presentation
    App --> Domain
    Presentation --> Domain
    Puzzle[shared/DuckDoku.Puzzle] --> Domain
    Puzzle --> Server[server/DuckDoku.Api]
```

| Сборка | Что внутри |
| --- | --- |
| `Domain` | правила игры и состояние поля — чистый C#, без Unity API |
| `Presentation` | вью, анимации, обработка ввода |
| `App` | сцены, DI (Zenject), сетевые клиенты к серверу |
| `shared/DuckDoku.Puzzle` | генератор и валидатор головоломки — общий для клиента и сервера |

Сервер — ASP.NET Core Minimal API поверх EF Core/PostgreSQL: прогресс по
уровням, энергия, подсказки и валюта считаются на сервере, клиент только
отображает состояние.

<p align="right">(<a href="#readme-top">наверх</a>)</p>

## Запуск проекта

Нужны: Unity Hub (редактор **6000.3.13f1**), **.NET 10 SDK**, **Docker**.

### Сервер

```bash
cp .env.example .env        # задать POSTGRES_PASSWORD
docker compose up -d        # поднять PostgreSQL
```

Создать `server/src/DuckDoku.Api/appsettings.Development.local.json`
(в `.gitignore`, в репозитории его нет):

```json
{
  "ConnectionStrings": {
    "Database": "Host=localhost;Port=5432;Database=duckdoku;Username=duckdoku;Password=<тот же пароль, что в .env>"
  }
}
```

```bash
dotnet run --project server/src/DuckDoku.Api
```

Поднимется на `http://localhost:5190` (миграции — автоматически),
документация API — `/scalar`.

### Клиент

Открыть `client/` в Unity Hub (редактор 6000.3.13f1).

По умолчанию клиент настроен на прод-сервер. Чтобы подключить его к
локальному — в `Assets/Game/Resources/ProjectContext.prefab`
переключить `Server Config` на `Server Config Local`.

<p align="right">(<a href="#readme-top">наверх</a>)</p>

## Генерация уровней

Каталог `content/levels/catalog.json` руками не редактируется — его
печёт консольный тул `tools/level-baker`. Генерация детерминирована
(фиксированный master seed), поэтому повторный запуск без изменений в
прогрессии даёт тот же каталог.

Прогрессия уровней задаётся бакетами `(размер, сложность, количество)`
в `tools/level-baker/Core/ProgressionPlan.cs`:

| Размер | Сложность | Кол-во |
| --- | --- | --- |
| 6×6 | Easy | 10 |
| 7×7 | Easy / Normal | 8 / 7 |
| 8×8 | Normal | 25 |
| 9×9 | Normal / Hard | 15 / 15 |
| 10×10 | Hard | 20 |

Чтобы добавить уровни — дописать бакет и перегенерировать каталог:

```bash
dotnet run --project tools/level-baker
```

Каждая головоломка генерируется через `DuckDoku.Puzzle.PuzzleGenerator`
и проверяется солвером: если решение не единственное, вариант
отбрасывается и генератор пробует заново (до 2000 попыток на уровень,
иначе тул падает с ошибкой). Награда за уровень считается в
`RewardPolicy` по размеру и сложности.

По умолчанию тул пишет в `content/levels/catalog.json`; другой путь
можно передать первым аргументом.

<p align="right">(<a href="#readme-top">наверх</a>)</p>

## Дев-читы (только в редакторе)

Все читы обёрнуты в `#if UNITY_EDITOR` — в собранной игре (в том числе
на itch.io) их нет физически, они существуют только при запуске из
Unity Editor.

| Хоткей | Эффект |
| --- | --- |
| `Space` | мгновенно решить текущий уровень |
| `Ctrl` + `Shift` + `E` | максимальная энергия |
| `Ctrl` + `Shift` + `Space` | +1000 к валюте |

Читы энергии и валюты дёргают дев-эндпойнты сервера
(`/api/v1/dev/energy/max`, `/api/v1/dev/currency/grant`), а сервер
регистрирует их только при `ASPNETCORE_ENVIRONMENT=Development` —
значит, эти два хоткея работают только с локальным сервером в
дев-режиме, на проде эндпойнтов просто нет. Автосолвер уровня работает
независимо от сервера.

<p align="right">(<a href="#readme-top">наверх</a>)</p>
