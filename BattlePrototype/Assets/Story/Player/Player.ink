VAR player_has_colt45_pistol = false
VAR player_Colt45_count = 12
VAR player_colt45_pistol_shots = 6

VAR player_has_winchester_rifle = false
VAR player__44Magnum_count = 12
VAR player_winchester_rifle_shots = 4

VAR player_has_shotgun = false
VAR player_shotgun_shots = 2
VAR player_ShotgunShell_count = 8

VAR player_money = 100
VAR player_bank_money = 1000

VAR player_is_dead = false

==on_player_death==
~ setTextBoxVisible(false)
~ setTimeScale(1)
~ createObject("Death", getPlayerX() - 1, getPlayerY())
~ useUnscaledTime("Death", true)
~ setTimeout(2, -> greet_death)
-> DONE