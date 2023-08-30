## Требования перед запуском приложения

### Настройка IIS сервера:

В диспетчере служб IIS необходимо создать новый пул приложения со следующими настройками:
Имя: AbsenceTrackApp
Версия среды CLR .NET: Без управляемого кода
Режим управляемого конвейера: Встроенный
Отметить Немедленный заупск пула приложения

Также необходимо добавить новый сайт со следующими настройками:
Имя сайта: AbsenceTrackApp
Пул приложений: AbsenceTrackApp
Физический путь: <указать путь до папки с приложением>
Порт: 32561
Имя узла: localhost
Остальные параметры по умолчанию

### Настройка SQL Server Management Studio:

Имя базы данных: TESTN7
SQL скрипт создания таблицы timesheet:

	CREATE TABLE [dbo].[timesheet](
		[id] [int] IDENTITY(1,1) NOT NULL,
		[reason] [tinyint] NOT NULL,
		[start_date] [date] NOT NULL,
		[duration] [tinyint] NOT NULL,
		[discounted] [bit] NOT NULL,
		[description] [varchar](1024) NOT NULL,
	 CONSTRAINT [PK_timesheet] PRIMARY KEY CLUSTERED 
	(
		[id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]

Необходимо создать новое имя для входа:
На странице Общие в поле Имя для входа ввести: IIS APPPOOL\AbsenceTrackApp и нажать кнопку ОК

В базу данных необходимо также добавить нового пользователя со следующими настройками:
На странице Общие в выпадающем списке Тип пользователя выбрать Пользователь Windows
Имя пользователя: IIS APPPOOL\AbsenceTrackApp
Имя для входа: IIS APPPOOL\AbsenceTrackApp
Схема по умолчанию: dbo
На странице Защищаемые объекты нажать на кнопку Найти -> Все объекты, принадлежащие схеме и выбрать схему по умолчанию и отметить следующие разрешения: Вставка, Выборка, Изменение, Обновление, Удаление
Нажать кнопку ОК

## Запуск приложения

Открыть решение AbsenceTrackApp в Visual Studio, выбрать профиль IIS и запустить приложение из Visual Studio (при запуске исполняемого файла вылетает ошибка 404)
