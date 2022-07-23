# Match-3
Этот проект представляет из себя игру жанра "3 в ряд" (match-3) с механикой "разрушения", которая заключается в нажатии на скопления плиток одного типа, что приводит к их разрушению.
Игра написана на Unity для Android-устройств.
# Правила игры
В начале игры предоставляется 10 ходов (количество можно настроить в инспекторе), которые тратятся при разрушении одиночных плиток.
При нажатии на одну плитку - она разрушается, при этом отнимается 1 ход и начисляется 1 очко. При нажатии на скопление из 3 и более плиток - они разрушаются, при этом начисляются как ходы, так и очки.
Если было разрушено скопление из 3-х плиток, то начисляется 2 хода, если из 4-х плиток - 3 хода, из 5-ти и более плиток - 4 хода.
Очки начисляются по формуле:
ReceivedScore = MatchedTilesCount * 2 - 1
# Таблица рекордов
При первом старте таблица рекордов считывается из файла Assets/Resources/Leaderboard.csv. Она сохраняется в PlayerPrefs и при последующих запусках загружается оттуда.
Если таблица рекордов полностью заполнена, то, чтобы попасть в неё, нужно побить предыдущий результат.
Если же таблица заполнена не полностью, то попадание в неё обеспечено вне зависимости от количества набранных очков.
