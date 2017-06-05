== test_a ==
I am test a -> END

== test_b ==
{Colt45_count == 0:{Looks like you could use some ammo|Already out? Have some more} -> give_ammo|If you run out of ammo, come talk to me -> END}

= give_ammo
~Colt45_count = Colt45_count + 20
-> END