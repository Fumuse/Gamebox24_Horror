# Gamebox24_Horror — Надежда "Fumuse" Шенфельд
## Архитектура проекта

- _Gamebox24_Horror/: корень проекта
    - Animations/: анимации
    - Audio/: звуки
    - Materials/: материалы
    - Models/: модели
    - Prefabs/: префабы
    - Scenes/: сцены
    - **Scripts/**: скрипты
      - Ambience/: скрипты для создания атмосферы
        - DarknessHaunting.cs: *обработка событий "Тьмы" - преследование игрока, отработка триггера вхождения во Тьму*
        - EndGameTrigger.cs: *обработка пересечения триггера в конце сцены для завершения игры*
        - FallingBookHandler.cs: *анимация подения книги со стола при пересечении триггера*
        - LightSwitcher.cs: *затухание источников света при пересечении триггера*
        - RostrumHandler.cs: *отображение стартовой книги при пересечении триггера трибуны*
      - Audio/: скрипты для звуков
        - AudioClipEntry.cs: *сущность для хранения звука в библиотеке*
        - AudioLibrary.cs: *библиотека звуков для быстрого доступа из кода*
      - Controls/: скрипты управления
        - CameraControls.cs: *управление зумом камеры*
        - Controls.cs: *Input System*
        - InputReader.cs: *слушатель событий управления*
      - Player/: скрипты управления персонажем
        - States/: стадии персонажа
          - PlayerBaseState.cs: *базовая стадия, от которой наследуются остальные. Реализует базовые методы перемещения*
          - PlayerFallState.cs: *управление стадией при "падении" персонажа с высоты/после прыжка*
          - PlayerJumpState.cs: *управление стадией прыжка персонажа*
          - PlayerMoveState.cs: *управление стадией перемещения персонажа (шаг)*
          - PlayerSprintState.cs: *управление стадией перемещения персонажа (спринт)*
        - PlayerAnimateEvents.cs: *обработка событий анимаций персонажа (звуки шагов)*
        - PlayerStateMachine.cs: *автомат состояний, управление компонентами персонажа*
      - StateMachine/: фундамент автомата состояний
        - State.cs: *база состояния для наследования других состояний*
        - StateMachine.cs: *база автомата состояний для наследований других автоматов*
      - UI/: скрипты для взаимодействия с UI
        - MainMenuController.cs: *управление стартовым меню и его кнопками*
      - Utilities/: различные хелперы
    - Settings/: файлы настроек (аудио, постобработки, dotween)
    - Sprites/: спрайты (UI)
    - Textures/: текстуры (для материалов)