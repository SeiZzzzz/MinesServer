namespace MinesServer.Enums
{
    public enum SkillType
    {
        Unknown = -1,
        /// <summary>a | aacd | Защита от слизи</summary>
        AntiSlime,
        /// <summary>k | ablk | Анти-блок</summary>
        AntiBlock,
        /// <summary>j | adja | Смежное извлечение</summary>
        AdjacentExtraction,
        /// <summary>U | geol | Геология</summary>
        Geology,
        /// <summary>B | minb | Добыча синих</summary>
        MineBlue,
        /// <summary>G | ming | Добыча зеленых</summary>
        MineGreen,
        /// <summary>D | dest | Разрушение</summary>
        Destruction,
        /// <summary>x | anig | Аннигиляция</summary>
        Annihilation,
        /// <summary>y | crys | Кристаллография</summary>
        Crystallography,
        /// <summary>z | decn | Деконструкция</summary>
        Deconstruction,
        /// <summary>u | agun | Защита от пушек</summary>
        AntiGun,
        /// <summary>E | bldr | Стройка красных</summary>
        BuildRed,
        /// <summary>d | digg | Копание</summary>
        Digging,
        /// <summary>l | live | Защита</summary>
        Health,
        /// <summary>m | mine | Добыча</summary>
        MineGeneral,
        /// <summary>R | minr | Добыча красных</summary>
        MineRed,
        /// <summary>L | bldg | Стройка</summary>
        BuildGreen,
        /// <summary>Q | bldq | Стройка квадроблоков</summary>
        BuildQuadro,
        /// <summary>q | dete | Обнаружение</summary>
        Detection,
        /// <summary>M | moto | Передвижение</summary>
        Movement,
        /// <summary>Y | bldy | Стройка желтых</summary>
        BuildYellow,
        /// <summary>P | comp | Компрессия</summary>
        Compression,
        /// <summary>F | frig | Охлаждение</summary>
        Fridge,
        /// <summary>C | minc | Добыча голубых</summary>
        MineCyan,
        /// <summary>t | moro | Передвижение по дорогам</summary>
        RoadMovement,
        /// <summary>*U | upgr | Экспертное обучение</summary>
        Upgrade,
        /// <summary>Z | deac | Деактивация</summary>
        Deactivation,
        /// <summary>h | hcmp | Гиперкомпрессия</summary>
        HyperPacking,
        /// <summary>V | minv | Добыча фиолетовых</summary>
        MineViolet,
        /// <summary>p | pack | Вместимость</summary>
        Packing,
        /// <summary>b | pakb | Упаковка синих</summary>
        PackingBlue,
        /// <summary>c | pakc | Упаковка голубых</summary>
        PackingCyan,
        /// <summary>v | pakv | Упаковка фиолетовых</summary>
        PackingViolet,
        /// <summary>*M | mony | Оптимизация</summary>
        Discount,
        /// <summary>J | sort | Сортировка</summary>
        Sort,
        /// <summary>S | subl | Турбо-охлаждение</summary>
        Turbo,
        /// <summary>X | magn | Размагничивание</summary>
        DeMagnetizing,
        /// <summary>W | minw | Добыча белых</summary>
        MineWhite,
        /// <summary>r | pakr | Упаковка красных</summary>
        PackingRed,
        /// <summary>w | pakw | Упаковка белых</summary>
        PackingWhite,
        /// <summary>g | pakg | Упаковка зеленых</summary>
        PackingGreen,
        /// <summary>o | reco | Извлечение</summary>
        Extraction,
        /// <summary>e | repa | Ремонт</summary>
        Repair,
        /// <summary>*D | emin | Экспертная добыча</summary>
        ExpertMining,
        /// <summary>i | wash | Промывание</summary>
        Washing,
        /// <summary>f | frac | Дробление</summary>
        Fracturing,
        /// <summary>H | nano | Наноупаковка</summary>
        NanoPacking,
        /// <summary>O | opor | Стройка опор</summary>
        BuildStructure,
        /// <summary>A | road | Стройка дорог</summary>
        BuildRoad,
        /// <summary>*B | bldu | Универсальная стройка</summary>
        BuildUniversal,
        /// <summary>*L | warb | Военный блок</summary>
        BuildWar,
        /// <summary>*A | arch | Архитектура</summary>
        Architecture,
        /// <summary>*T | tods | Тотальное разрушение</summary>
        TotalDestruction,
        /// <summary>*u | ultr | Ультра-добыча белых</summary>
        UltraWhite,
        /// <summary>*J | jewl | Ювелирная добыча фиолетовых</summary>
        Jewlery,
        /// <summary>*I | indu | Индукция</summary>
        Induction,
        /// <summary>*a | acid | Слизевая добыча</summary>
        MineSlime,
        /// <summary>*d | deep | Глубинная добыча</summary>
        MineDeep,
        /// <summary>*g | gluo | Глюонная упаковка</summary>
        GluonPacking
    }
}
