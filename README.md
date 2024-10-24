# Usage
Собрать проект в Visual studio или через dotnet.

Далее:
``` bash
  # Перейти в директорию с исполняемым файлом
  cd E:\Testtask\ConsoleApp1\bin\Debug\net8.0
  #-h или любой некорректный ввод выведит help
  ConsoleApp1.exe -h
  # Установить начальный конфиг и запустить программу. 
  ConsoleApp1.exe -l E:\Testtask\log.txt -o E:\Testtask\order.txt -i E:\Testtask\input.txt -d District1 -fdt 2024-10-24 14:32:28
  # если не указывать параметры при запуске, будут использованы параметры предыдущего успешного запуска.
```
Время и район в данной команде установлено на тестовые данные в input.txt

В случае командной строки Windows (cmd.exe) может потребоваться установика нужной кодировки для корректного отображения русского текста
``` bash
  chcp 1251
```
