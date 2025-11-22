# -----------------------------
# 1. Сборка приложения (ARM64)
# -----------------------------
FROM --platform=linux/arm64 mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем решение и все проекты для корректного восстановления зависимостей
COPY vk_bot_img.sln ./
COPY vk_bot_img.csproj ./ 
# Если есть другие проекты, их тоже можно копировать отдельно
# COPY OtherProject.csproj ./OtherProject/

# Восстанавливаем зависимости по решению
RUN dotnet restore vk_bot_img.sln

# Копируем весь исходный код
COPY . ./

# Публикуем через решение, указываем конкретный проект, если нужно
RUN dotnet publish vk_bot_img.csproj -c Release -o /app /p:StartupProject=VkNet

# -----------------------------
# 2. Финальный образ с Runtime (ARM64)
# -----------------------------
FROM --platform=linux/arm64 mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Копируем скомпилированное приложение из сборочного образа
COPY --from=build /app ./

# Копируем папку с слоями
COPY Overlay ./Overlay/

# Указываем команду запуска
ENTRYPOINT ["dotnet", "vk_bot_img.dll"]
