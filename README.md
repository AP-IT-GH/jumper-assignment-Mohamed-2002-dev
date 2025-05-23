## Set-up

De omgeving bevat een agent die bewegende obstakels moet ontwijken door erover te springen. De obstakels hebben in elke episode een willekeurige snelheid en spawnen op random momenten.

## Doel

Het doel van de agent is om zo lang mogelijk te overleven door op tijd over obstakels te springen.

## Agents

De omgeving bevat één agent die de obstakels moet ontwijken.

## Beloningsfunctie (onafhankelijk)

- +0.5 bij het succesvol ontwijken van een obstakel door eroverheen te springen.

- -0.5 bij botsing met een obstakel.

- -0.01 bij onterecht springen (bijvoorbeeld als er geen obstakel in de buurt is).

## Gedragsparameters

Vector Observatieruimte: De agent heeft 10 observaties, bestaande uit:

- Positie van de agent (3 waarden: x, y, z)

- Snelheid van de agent (3 waarden: vx, vy, vz)

- Positie en snelheid van de 3 dichtstbijzijnde obstakels (6 waarden: x, y, z, vx, vy, vz)

## Acties: 1 discrete actie-branch met twee mogelijke acties:

- 0 = Niets doen

- 1 = Springen

## Gemiddelde benchmark-beloning

De gemiddelde beloning van de agent na 50.000 trainingsstappen zou ongeveer 0.8 moeten zijn, afhankelijk van de effectiviteit van de agent in het ontwijken van obstakels en het minimaliseren van negatieve straffen (zoals onterecht springen en botsingen).
