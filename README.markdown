# F&C Dave Co.

## Description

F&C Dave Co. is a multinational insurance and banking services company headquartered on 71-Gordion.
It has an agreement with the Company to offer its services to Company employees.

## Features

- Insurance for when everyone dies and you lose your loot
	- Get a insurance policy using the terminal
		- Tier options are Economic, Standard, and Bespoke
			- Economic
				- Base premium is 10% of the coverage
				- Deductible is 30% with a minimum of 20% of the coverage and a maximum of 50% of the coverage
			- Standard
				- Base premium is 20% of the coverage
				- Deductible is 15% with a minimum of 10% of the coverage and a maximum of 25% of the coverage
			- Bespoke
				- Base premium is 35% of the coverage
				- No deductibles
			- **Premium will increase by 15% every time it is used, up to 75%**
		- Coverage options are $300, $650, $1500, $2750, $5000, $10000
	- Make a claim using the terminal when you lose all your loot due to full crew death
		- You must pay the deductible if your policy tier requires it.
- Loan system when you need it for the quota (**not implemented**).

## Terminal commands

- `insurance info`, `insurance information`, `insurance policy`
	- Get information on the current insurance policy
- `insurance select`, `insurance configure`
	- Select a new insurance policy
	- You will be charged for the initial payment
	- Can only be used as a host while in orbit
- `insurance claim`, `insurance claims`, `insurance make claim`
	- Confirm a insurance claim
	- A gold bar with the value of the lost scrap (or policy coverage, if over it) will be given to you
	- Can only be used as a host while in orbit
	- You must have a insurance policy