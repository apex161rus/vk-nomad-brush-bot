
### Удаление мусора из архивов

```bash
zip -d Name.zip "__MACOSX/*"
zip -d Name.zip "__MACOSX/*" "*/._*"
```
### Опционально удаляем .DS_Store
```bash
zip -d Name.zip "*.DS_Store"
```