VAR card_player_money = 1000

== card_player ==
Hey stranger, do you want to play a game of cards?
+ Yes -> play_game
+ No -> dont_play_game
= play_game
~ playCards("card_player", "Saloon:card_game", -> after_game)
-> DONE
= dont_play_game
Alright, come back if you change your mind -> DONE

= after_game
~ lookAt("Player", getPlayerX(), getPlayerY() + 1)
Well played, come back and play any time -> DONE