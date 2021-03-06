# Higher

## Introduction
Higher is a digital playing card game that pits the player against a computer opponent. The main goal of the game is to select cards with a number value high enough that
when added to the score counter in the middle, will surpass the opponent's total score counter. The player and computer trade blows by picking valid cards until either one
of them runs out of cards or no longer has any valid cards in play. This game was heavily-inspired by the playing card minigame "Blade" from Nihon Falcom's "Trails of Cold Steel"
series.

Check out the Blade minigame here: https://youtu.be/IVmlHhTL7Bc?t=10

**NOTE: All of the free assets used in this project that require attribution will have the appropriate attribution listed below as well as in the credits 
page of the game (white button on the top left of the screen).**

## Types of Cards
There are 3 main types of cards: Planet, Sun, and Wormhole. 

### Planet Cards
Planet cards are the standard value cards with no intrinsic special ability (except for the Pluto card) modelled after the 9 original planets of our solar system.
Pluto holds the smallest value in the game being only worth 1 point while Jupiter holds the highest point value boasting 9 points! These cards are the main stay of your
celestial deck and will help push your score Higher and (hopefully) above the opponent's. 

### Sun Card
Sun cards are a special type of card that will turn over the most-recently played card by your opponent, removing that card's value from the opponent's score total. The
opponent has 1 chance to revive the turned-down card with a Pluto card if they have one. However, if they play a regular planet or wormhole card instead, the turned-down
card is now lost forever.

If a Sun card is used on an already-facedown card, it will cause the playing field to implode, destroying all active middle cards and forcing a redraw.

### Wormhole Card
Wormhole cards are a special type of card that will switch both the player and computer's scores. This card is particularly devastating when the opponent has a high point lead. 
Be aware, however, that the opponent can also reply with a Wormhole of their own, switching the scores right back!

## Typical Game Process

### Phase 1: Game Setup
#### Shuffling
Each player has a deck of 32 cards which will be shuffled using the Fisher-Yates algorithm (a popular way of randomizing a set of values).

#### Dealing
10 cards are dealt out from the player's and computer's deck onto the bottom and top portions of the screen, respectively. You are not allowed to view the opponent's cards.

### Phase 2: The Actual Game

#### Card Drawing
Both players must draw a single card from their deck by clicking on it. Both cards then get placed in the middle of the playing field, which is where all cards are "active" 
and will have their values added to their owners' point total.

Whoever drew the card with the lowest value gets to go first.

#### Selecting a Card from the Hand
Once both players have drawn a card from the first stage, the player that scored lower goes first. The player must draw a card that when added to their point total, will
surpass the point total of the opponent. After picking a card with a valid value, the opposing player goes next.

### Phase 3: Win Conditions

1. The player plays a valid card and the computer has no cards left to reply with.
2. The computer plays a card with insufficient value.
3. The computer plays a special card (Sun or Wormhole) as their last card. This is considered a foul and will result in an instant loss for the offender.

## Current WIPs

### Animations
I have most of the animations done currently (special thanks to the Unity plugin "iTween"). I plan to add cool animations for wormhole and sun card usage.

### Sounds
Sounds mostly done. Can't find a good shuffling + dealing sound effect, though. Music attribution is listed below, but the choice of background music is still subject to change.

### More Cards!!!
Of course, the sun and wormhole cards will not be the only special cards in the game. I am still coming up with new ideas on the flow :)

## Tools Used

### Unity
The bread and butter front and backend of the game. Programmed using C# and Unity's 2D renderer. 

### GIMP
Essentially photoshop-lite. Used to edit free-to-modify game assets like the cards.

### YouTube
Tons of card game tutorials out there that helped shape my workflow and game structure.

## Asset Sources

### Sprites and Images
* https://pixabay.com/
* https://www.fairway3games.com/free-poker-sized-card-templates/

### Sounds
* https://freesound.org/
* [Creative Commons Fire Burst.wav by SilverIllusionist is licensed under CC BY 3.0](https://freesound.org/people/SilverIllusionist/sounds/472688/)
* [Creative Commons Card Flip by f4ngy is licensed under CC BY 3.0](https://freesound.org/people/f4ngy/sounds/240776/)
* [Creative Commons Dealing Card by f4ngy is licensed under CC BY 3.0](https://freesound.org/people/f4ngy/sounds/240777/)
* [Creative Commons teleport.wav by Sergenious is licensed under CC BY 3.0](https://freesound.org/people/Sergenious/sounds/55853/)
* [Creative Commons Ricochet 2.wav by morganpurkis is licensed under CC BY 3.0](https://freesound.org/people/morganpurkis/sounds/392975/)
* [Creative Commons Single Water Drop III by MattiaGiovanetti is licensed under CC BY 3.0](https://freesound.org/people/MattiaGiovanetti/sounds/479451/)



### Music
Smooth Lovin by Kevin MacLeod<br>
Link: https://incompetech.filmmusic.io/song/4379-smooth-lovin<br>
License: http://creativecommons.org/licenses/by/4.0/
