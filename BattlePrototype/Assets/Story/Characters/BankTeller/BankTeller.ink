VAR bank_money_amount = 0
== bank_teller ==
What can I do for you? -> choices
= choices
+ Deposit -> deposit
+ Withdraw -> withdraw
+ Nothing -> DONE

= deposit
~ bank_money_amount = 0
~ useSpinner(3, "bank_money_amount")
How much money do you want to deposit?
{ 
    - bank_money_amount > player_money:
It looks like you don't have that much. Deposit {player_money} instead?
+ Yes -> deposit_all
+ No -> anything_else
    - else:
    ~ transferMoney(bank_money_amount)
    -> anything_else
}
= deposit_all
~ transferMoney(player_money)
-> anything_else

= withdraw
~ bank_money_amount = 0
~ useSpinner(3, "bank_money_amount")
How much money do you want to withdraw?
{
    - bank_money_amount > player_bank_money:
It looks like you don't have that much. Withdraw {player_bank_money} instead?
+ Yes -> withdraw_all
+ No -> anything_else
    - else:
    ~transferMoney(-bank_money_amount)
    -> anything_else
}
=withdraw_all
~transferMoney(-player_bank_money)
-> anything_else

= anything_else
Anything else? -> choices

== function transferMoney(amount) ==
~ player_money = player_money - amount
~ player_bank_money = player_bank_money + amount