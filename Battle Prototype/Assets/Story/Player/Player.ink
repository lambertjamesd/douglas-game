VAR player_Colt45_count = 12
VAR player_colt45_pistol_shots = 6

==on_player_death==
You have died.
...
~ setTextBoxVisible(false)
~ setTimeScale(1)
~ createObject("Death", getPlayerX() - 1, getPlayerY())
-> DONE