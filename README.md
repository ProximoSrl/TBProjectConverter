# TBProjectConverter
---
Conversione dei file di progetto TaskBuilder c++ da vs2010 a vs2017 come indicato nella guida alla migrazione https://github.com/Microarea/MigrazioneMago4/wiki/Passaggio-a-Visual-Studio-2017#modifiche-ai-file-di-progetto-vcxproj

## Prerequisiti
- DotNetCore 2.x 

## Build dai sorgenti
    git clone https://github.com/ProximoSrl/TBProjectConverter.git
    cd TBProjectConverter\PrxmTbUtils
    dotnet build

## Utilizzo
Aprire la solution per convertire i progetti nel nuovo formato di vs2017 ed annullare l'upgrade delle impostazioni previste da Visual Studio premendo Cancel sulla dialog di richiesta.

Lanciare

    dotnet run  C:<dev_root>\<mago_version>\Standard\Applications\<tb_app_name>\
