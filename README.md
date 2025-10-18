# 🎯 Roulette Popup Test Task

## 📘 Основная идея
UI-система для рулетки с анимацией и пулом объектов.  
Код ориентирован на производительность, отсутствие GC и чистую архитектуру (MVP-подход).

## ⚙️ Технические решения
- **Архитектура:** View / Presenter / ServiceLocator (DI).
- **Анимации:** DOTween с Target-контролем и безопасным Kill.
- **Пулы:** ObjectPool для иконок наград (без Instantiate в рантайме).
- **Оптимизация:** 2 draw call.

## 🧩 Используемые технологии
Unity 2022.3+, DOTween, TextMeshPro, ObjectPool API.

