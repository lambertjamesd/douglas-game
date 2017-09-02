VAR player_has_Colt45 = false
VAR player_Colt45_count = 12
VAR player_colt45_pistol_shots = 6

VAR player_money = 100

==on_player_death==
You have died.
[...]
~ setTextBoxVisible(false)
~ setTimeScale(1)
~ createObject("Death", getPlayerX() - 1, getPlayerY())
~ setTimeout(2, -> greet_death)
-> DONE