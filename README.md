# Treasure Hunters

A turn-based virtual board game set around finding treasure and sabotaging your opponents.
**Not yet fully implemented**

The game concept was created as part of a university course with my 3 teammates, Martynas Ramonas​, Arnas Parafinovicius, and Tomas Dailide. We designed an actual board game for the course, and I developed this prototype so we could quickly playtest. I am really proud of our concept, our execution of it and this prototype I developed, which I intend to improve. The initial commit was developed over four days. 

## Gameplay

The game starts on a hexagon playing board where various types of cells are randomly placed. Each player starts in one corner.
(This prototype generates a triangular map for a three-player setup. The game would be playable with more players.)

Turn order is determined via dice roll. Each turn, the player rolls a die to determine their max movement range. They may choose to move up to the max range in one of the six directions in a straight line. When a cell is moved over, it's revealed to everyone, and its effect is applied.

### Cell Types

- Plain cells have no effect,
- Obstacle cells interrupt your movement,
- Treasure cells reward you with gold,
- Artifact cells give you a random Artifact from the Artifact pile,
- Wheel cells either give you a random Artifact/Curse or makes you gain/lose some treasure.

After a cell is revealed, its effects are disabled for the rest of the game. (A cell cannot be used twice.) Each turn, players have the option of using one of the Artifacts in their hand. Some Artifacts are additional to the regular movement while some override the movement.

### Artifacts

Artifacts are action cards that the players add to their hand and may use at the start of their turn. Once an Artifact is used, it's discarded. Artifacts should be hidden from opponents.

- Move an extra cell
- Move through an obstacle
- Reveal 3 cells in a straight line
- Defend from a Steal (treasure or Artifact)
- Steal treasure from a player
- Steal an Artifact from a player
- Teleport to a cell in a range of 4

### Curses

Curses are immediate negative effects applied to the player. In the current version, the only way to get a Curse is on a Wheel cell. There should probably be Curse cells on the map in the future.

- Move 1 less cell on your next turn
- Other players may each reveal 2 tiles around them
- Give away treasure to the next player
- Give away an Artifact to the next player
- Cannot use Artifacts on your next turn
- Reveal your Artifacts to other players

### Winning

After every cell is revealed or a certain number of turns have been reached, the player with the most gold wins.

## Missing features

- Anticipatory dice roll screen
- Spin-the-wheel screen
- Artifacts (tricky on a local game)
- Configurable map size and player count
