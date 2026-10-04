# CurrencyRates

Сервис курсов валют ЦБ РФ на .NET 8 и PostgreSQL. Фоновый сервис загружает курсы с сайта ЦБ. Пользователь регистрируется, выбирает избранные валюты и получает их курсы. Авторизация через JWT, все запросы идут через API Gateway.

## Состав решения

| Проект | Что делает | Порт (локально) |
|---|---|---|
| `src/Migrator` | Применяет миграции EF Core и завершается | — |
| `src/CurrencyUpdater` | Фоновый сервис: при старте и затем раз в час загружает `XML_daily.asp` и обновляет таблицу `currency` | — |
| `src/UserService/*` | Регистрация, логин, логаут (Clean Architecture + CQRS) | 5001 |
| `src/FinanceService/*` | Курсы избранных валют пользователя (Clean Architecture + CQRS) | 5002 |
| `src/Gateway` | API Gateway на YARP, единая точка входа | 5000 |
| `tests/UserService.UnitTests`, `tests/FinanceService.UnitTests` | Unit-тесты обработчиков команд и запросов | — |

UserService и FinanceService разбиты на четыре проекта каждый:

```
Domain          сущности, ни от чего не зависит
Application     команды и запросы (MediatR), их обработчики, интерфейсы репозиториев
Infrastructure  EF Core и репозитории; в UserService ещё хеширование пароля и выпуск JWT
Api             контроллеры, регистрация зависимостей, JWT-аутентификация
```

Зависимости направлены внутрь: `Api → Infrastructure → Application → Domain`. Application не знает про EF Core и ASP.NET Core, поэтому обработчики тестируются с подменёнными зависимостями, без базы.

## Требования

- Docker (Docker Desktop) — для запуска всей системы или только PostgreSQL.
- .NET SDK 8 или новее — для сборки без Docker. Если установлен SDK 8, используется он, иначе ближайший более новый (например, SDK 9 из Visual Studio 2022 17.12+). Это задано в `global.json`.
- .NET 8 Runtime (ASP.NET Core) — для запуска сервисов без Docker. Входит в .NET 8 SDK.
- Visual Studio 2022 или Rider — по желанию.

## Запуск в Docker

```bash
docker compose up -d --build
```

Порядок старта: `postgres` → `migrator` (применяет миграции и завершается) → `currency-updater`, `user-service`, `finance-service` → `gateway`.

- Gateway: **http://localhost:5000** — единственный открытый наружу сервис.
- Swagger UI: **http://localhost:5000/swagger**.
- PostgreSQL: `localhost:5433`, база `currency`, пользователь `postgres`, пароль `postgres`.

Остановить: `docker compose stop`. Данные хранятся в volume `pgdata`.

## Запуск из Visual Studio, Rider или консоли

1. Поднять только базу: `docker compose up -d postgres`.
2. Запустить `Migrator` и дождаться, пока он завершится. Он создаёт таблицы, без них остальные сервисы не работают.
3. Запустить `CurrencyUpdater`, `UserService.Api`, `FinanceService.Api` и `Gateway`. В Visual Studio для этого можно выбрать несколько запускаемых проектов.

Из консоли (из корня репозитория):

```bash
dotnet run --project src/Migrator
dotnet run --project src/CurrencyUpdater
dotnet run --project src/UserService/UserService.Api
dotnet run --project src/FinanceService/FinanceService.Api
dotnet run --project src/Gateway
```

Gateway в Docker и Gateway, запущенный локально, используют один порт 5000, поэтому одновременно их запускать нельзя.

## Тесты

```bash
dotnet test
```

## API

Все адреса указаны относительно Gateway (`http://localhost:5000`). Готовые запросы лежат в [`requests.http`](requests.http), их можно выполнить из Visual Studio или Rider.

### Swagger

Swagger UI для обоих сервисов открывается через Gateway: **http://localhost:5000/swagger**. Сервис выбирается в списке справа вверху (UserService или FinanceService).

1. В UserService выполнить `POST /api/users/register`, затем `POST /api/users/login` и скопировать `token` из ответа.
2. Нажать **Authorize** и вставить токен без слова `Bearer`.
3. Переключиться на FinanceService и выполнять запросы. Токен запоминается в браузере, поэтому вводить его заново при переключении сервиса или перезагрузке страницы не нужно.

Описания API сервисы отдают сами (`/swagger/v1/swagger.json`), а Gateway проксирует их по адресам `/swagger/users/swagger.json` и `/swagger/finance/swagger.json`. Запросы из Swagger UI идут через Gateway так же, как от любого другого клиента.

| Метод | Путь | Авторизация | Описание | Ответы |
|---|---|---|---|---|
| POST | `/api/users/register` | — | Регистрация `{ "name", "password" }` | 200 `{ "id" }`, 400, 409 |
| POST | `/api/users/login` | — | Логин `{ "name", "password" }` | 200 `{ "token", "expiresAt" }`, 401 |
| POST | `/api/users/logout` | Bearer | Отзыв текущего токена | 204, 401 |
| GET | `/api/finance/rates` | Bearer | **Курсы избранных валют текущего пользователя** | 200, 401 |
| GET | `/api/finance/currencies` | Bearer | Все валюты с курсами, чтобы выбрать избранное | 200, 401 |
| POST | `/api/finance/favorites/{currencyId}` | Bearer | Добавить валюту в избранное | 204, 401, 404 |
| DELETE | `/api/finance/favorites/{currencyId}` | Bearer | Убрать валюту из избранного | 204, 401 |

Коды: 400 — не прошла валидация (тело в формате ProblemDetails), 401 — неверное имя или пароль либо нет действующего токена, 404 — нет такой валюты, 409 — имя занято.

Пример:

```bash
curl -X POST http://localhost:5000/api/users/register -H "Content-Type: application/json" -d '{"name":"alex","password":"secret123"}'
curl -X POST http://localhost:5000/api/users/login    -H "Content-Type: application/json" -d '{"name":"alex","password":"secret123"}'
# дальше подставить token из ответа на логин
curl http://localhost:5000/api/finance/currencies           -H "Authorization: Bearer <token>"
curl -X POST http://localhost:5000/api/finance/favorites/16 -H "Authorization: Bearer <token>"
curl http://localhost:5000/api/finance/rates                -H "Authorization: Bearer <token>"
```

## База данных

| Таблица | Колонки |
|---|---|
| `currency` | `id`, `name` (уникальный), `rate` |
| `user` | `id`, `name` (уникальный), `password` |
| `user_favorite_currency` | `user_id`, `currency_id`: составной первичный ключ, внешние ключи с каскадным удалением |
| `revoked_token` | `jti` (первичный ключ), `expires_at` |

Схемой владеет только Migrator. У каждого сервиса свой `DbContext` с маппингом только нужных ему таблиц, без миграций.

## Решения и допущения

- **Избранное.** Пользователя интересует определённый набор валют, но отдельной таблицы для этого в исходной схеме нет. Добавлена связь многие-ко-многим `user_favorite_currency` и эндпоинты для управления избранным.
- **`currency.name`** хранит буквенный код ISO 4217 (`USD`, `EUR`). Он уникален и не меняется, а русские названия у ЦБ согласованы с номиналом («Алжирских динаров»).
- **`currency.rate`** — курс за **одну** единицу валюты (поле `VunitRate`). `Value` у ЦБ указан за `Nominal` единиц, например за 100 DZD. Тип `numeric` без ограничения точности: у части валют курс приходит в виде `4,80018E-05`.
- **Ответ ЦБ** приходит в кодировке windows-1251, дробная часть отделена запятой. Кодировка подключается через `CodePagesEncodingProvider`, числа разбираются с `NumberStyles.Float`.
- **CurrencyUpdater** сделан без разбиения на слои: это небольшой фоновый процесс без бизнес-логики. Ошибка загрузки пишется в лог, следующая попытка — на следующем тике таймера. На каждую загрузку создаётся свой DI-scope и свой `DbContext`.
- **Пароль** хранится в колонке `password` в виде хеша: `PasswordHasher` из ASP.NET Core Identity (PBKDF2 с солью).
- **Логаут.** JWT нельзя удалить на сервере, поэтому при логауте `jti` токена записывается в `revoked_token`. Оба сервиса проверяют эту таблицу в `OnTokenValidated`, поэтому после логаута токен перестаёт работать везде.
- **Курсы «по пользователю».** Id пользователя берётся только из токена (claim `sub`), а не из URL или тела запроса. Иначе можно было бы запросить или изменить чужое избранное.
- **Ожидаемые исходы без исключений.** Обработчик возвращает `null` или `false` («имя занято», «неверный пароль», «нет такой валюты»), а контроллер переводит это в 409, 401 или 404. При неверном имени и неверном пароле ответ одинаковый, чтобы по нему нельзя было узнать, какие имена зарегистрированы.
- **Идемпотентность.** Повторное добавление валюты в избранное и удаление отсутствующей не считаются ошибкой и возвращают 204.
- **Общая база.** Сервис миграций один, поэтому и база одна на все сервисы. Для полноценных микросервисов правильнее отдельная база у каждого.
- **Ключ подписи JWT** в `appsettings.json` предназначен только для разработки и одинаков у обоих сервисов. В продакшене его передают через переменную окружения `Jwt__SigningKey` из хранилища секретов.
- **MediatR** закреплён на версии 12.x: начиная с 13-й библиотека распространяется по коммерческой лицензии.

## Что можно улучшить

- Периодически удалять из `revoked_token` записи с истёкшим `expires_at`, либо перенести чёрный список в Redis.
- Добавить refresh-токены и сократить срок жизни access-токена.
- Вынести одинаковую настройку JWT-аутентификации сервисов в общую библиотеку.
- Покрыть репозитории интеграционными тестами на настоящем PostgreSQL (Testcontainers).
- Добавить повторные попытки при обращении к ЦБ (`Microsoft.Extensions.Http.Resilience`).
- Помечать или удалять валюты, которые ЦБ перестал публиковать.
- Добавить health checks сервисов, а в Gateway — rate limiting.
