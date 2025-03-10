# F&C Dave Co.

## Description

F&C Dave Co. is a multinational insurance and banking services company headquartered on 71-Gordion.
It has an agreement with the Company to offer its services to Company employees.

## Credits

Thank you to the following people / groups for inspiration, documentation, libraries, and references.

- [domkalanus](https://thunderstore.io/c/lethal-company/p/domkalanus/): [lethalinsurance](https://thunderstore.io/c/lethal-company/p/domkalanus/lethalinsurance/)
- [Um_no](https://thunderstore.io/c/lethal-company/p/Um_no/): [LethalCredit](https://thunderstore.io/c/lethal-company/p/Um_no/LethalCredit/)
- [Unofficial Lethal Company Community](https://github.com/LethalCompanyCommunity): [Lethal Company Modding Wiki](https://lethal.wiki/)
- [Distractic](https://github.com/Distractic): [LethalCompanyTemplate](https://github.com/Distractic/LethalCompanyTemplate)
- [BMX](https://thunderstore.io/c/lethal-company/p/BMX/): [LobbyCompatibility](https://thunderstore.io/c/lethal-company/p/BMX/LobbyCompatibility/)
- [MaxWasUnavailable](https://thunderstore.io/c/lethal-company/p/MaxWasUnavailable/): [LethalModDataLib](https://thunderstore.io/c/lethal-company/p/MaxWasUnavailable/LethalModDataLib/)
- [WhiteSpike](https://thunderstore.io/c/lethal-company/p/WhiteSpike/): [Interactive Terminal API](https://thunderstore.io/c/lethal-company/p/WhiteSpike/Interactive_Terminal_API/)
- [xilophor](https://thunderstore.io/c/lethal-company/p/xilophor/): [LethalNetworkAPI](https://thunderstore.io/c/lethal-company/p/xilophor/LethalNetworkAPI/)
- [Sigurd](https://thunderstore.io/c/lethal-company/p/Sigurd/): [CSync](https://thunderstore.io/c/lethal-company/p/Sigurd/CSync/)
- [tinyhoot](https://thunderstore.io/c/lethal-company/p/tinyhoot/): [ShipLoot](https://thunderstore.io/c/lethal-company/p/tinyhoot/ShipLoot/)
- [akechii](https://thunderstore.io/c/lethal-company/p/akechii/): [DiscountAlert](https://thunderstore.io/c/lethal-company/p/akechii/DiscountAlert/)
- [guy7cc](https://thunderstore.io/c/lethal-company/p/guy7cc/): [LCNoPropsLost](https://thunderstore.io/c/lethal-company/p/guy7cc/LCNoPropsLost/)
- [dancemoon](https://thunderstore.io/c/lethal-company/p/dancemoon/): [DanceTools](https://thunderstore.io/c/lethal-company/p/dancemoon/DanceTools/)
- [DaXcess](https://thunderstore.io/c/lethal-company/p/DaXcess/): [NoPenaltyReimagined](https://thunderstore.io/c/lethal-company/p/DaXcess/NoPenaltyReimagined/)

## Features

- Configurable values: if you don't like these numbers / options, you can change them
- Disables the death credit penalty only if there is a current insurance policy
- Insurance for when everyone dies and you lose your loot
	- Get an insurance policy using the terminal
		- You will be charged for the initial payment
		- Tier options are Economic, Standard, and Bespoke
			- Economic
				- Base premium is 7% of the coverage
				- Deductible is 25% with a minimum of 15% of the coverage and a maximum of 60% of the coverage
			- Standard
				- Base premium is 15% of the coverage
				- Deductible is 10% with a minimum of 10% of the coverage and a maximum of 30% of the coverage
			- Bespoke
				- Base premium is 35% of the coverage
				- No deductibles
			- **Premium will increase by 25% every time it is used, based on usage within the past 10 days**
		- Coverage options (in credits) are 150, 400, 800, 1500, 2250, 3600, 5500, 8000, 11125, 15250, 18000
	- Make a claim using the terminal when you lose all your loot due to full crew death
		- You must pay the deductible if your policy tier requires it
		- A gold bar with the value of the lost scrap (maximum is the coverage amount) will be given to you
- Loan system for when you are short of the quota
	- Get a loan using the terminal (only one loan may be taken out at a time)
	- The loan will pay for your quota in full
	- An interest of 5% will be added to the amount required to be paid back
	- Pay it off using the terminal within 4 days for no penalty
	- If you do not pay within that time, 10% of your credits will be garnished each day for loan payments

## Terminal commands

### Insurance

- `insurance info`, `insurance information`, `insurance policy`
	- Get information on the current insurance policy
- `insurance select`, `insurance get`
	- Select a new insurance policy (or cancel your current insurance policy)
- `insurance claim`, `insurance claims`, `insurance make claim`
	- Confirm an insurance claim
	- You must have a insurance policy
	- Look below you for the gold bar

### Bank

- `bank loan info`, `bank loan information`
	- Get information on the current loan
	- You must have an unpaid loan
- `bank loan get`, `bank loan`
	- Get a loan covering your current quota
	- You must not have an unpaid loan
- `bank loan pay`, `bank loan payment`
	- Pay the current loan off
	- You can pay either in full or only a specific percent of the loan amount
	- You must have an unpaid loan

## Contact

You can:
* use the [GitHub issue tracker](https://github.com/tesinormed/FAndCDaveCo/issues)
* join the [Amaranth Legacy Discord](https://discord.amaranth-legacy.community/) and ping `tesinormed`
