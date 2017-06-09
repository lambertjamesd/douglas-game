== test_a ==
Choose 
* A -> A_chosen
* B
- I am test a -> END
= A_chosen
apsduapodhvaoisdypasd -> END

== test_b ==
{player_Colt45_count == 0:{Looks like you could use some ammo|Already out? Have some more} -> give_ammo|If you run out of ammo, come talk to me -> END}

= give_ammo
~player_Colt45_count = player_Colt45_count + 20
-> END