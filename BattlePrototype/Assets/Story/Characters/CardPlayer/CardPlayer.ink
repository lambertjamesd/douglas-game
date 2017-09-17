VAR card_player_money = 1000

== card_player ==
~ useUnscaledTime("CardPlayer", true)
~ lookAt("CardPlayer", getPlayerX(), getPlayerY())
Hey stranger, do you want to play a game of cards?
+ Yes -> play_game
+ No -> dont_play_game
= play_game
{
    - player_money < 10:
You need at least $10 to buy in. Maybe later -> done_talking
    - else:
Lets play!
~ playCards("card_player", "Saloon:card_game", -> after_game)
-> DONE
}
= dont_play_game
Alright, come back if you change your mind
-> done_talking

= after_game
~ useUnscaledTime("CardPlayer", true)
~ lookAt("CardPlayer", getPlayerX(), getPlayerY())
~ lookAt("Player", getPlayerX(), getPlayerY() + 1)
Well played, come back and play any time
[...]
-> done_talking
= done_talking
~ useUnscaledTime("CardPlayer", false)
~ lookAt("CardPlayer", getCharacterX("CardPlayer") - 1,  getCharacterY("CardPlayer"))
 -> DONE