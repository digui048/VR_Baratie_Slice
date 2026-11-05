# Baratie Slice

<p align="center">
  <img src="https://static.wikia.nocookie.net/onepiece/images/0/05/OPGC_8.jpg/revision/latest/scale-to-width-down/1000?cb=20180412173739&path-prefix=es" width="1080">
</p>
Virtual reality game, made with Unity, for Meta Quest 3. It takes place in the kitchen of the Baratie, the floating restaurant from One Piece.

## Game objective

You play as Sanji, Zeff's apprentice. Before the dinner rush, Zeff sends you to the kitchen to help another cook. This cook is hidden behind the counter in front of you and throws you all the ingredients that need to be cut.

You have a knife in each hand and 60 seconds to slice as many ingredients as you can before they fall to the floor. At the end of the round you get your score and a rating depending on how many ingredients you cut: Sous Chef, Head Chef or Sanji-level. From the end screen you can play again or go back to the menu.

## Building Blocks used

- Camera Rig: the player's view and head tracking.
- Controller Tracking: the knives are attached to the controllers, so each hand holds one.
- Interactions (ray): the menus, Zeff's dialogue and the end screen are used by pointing with the controller and pressing the trigger.
- Haptics: the controller vibrates when its knife cuts an ingredient.
- Spatial Audio: the throw and cut sounds come from where the ingredient is, and Zeff's voice comes from in front of the player.

## Innovation aspect

The game starts with a short story in visual novel style: Zeff appears with his portrait, his lines and a voice, and explains what you have to do. The ingredients don't appear out of nowhere, they come from behind the counter as if the other cook was throwing them.

A cut only counts if the knife moves fast enough, and the ingredient splits in two halves following the direction of the knife. Only the hand that made the cut vibrates. In the menus you see the controllers and the ray, but while playing you only see the knives. The music changes between the menu and the round, and ingredients come faster as time goes on.

## Summary of the development process

The project was built in different phases. First, the project was set up for Quest 3 and the kitchen was built with free 3D models, together with the camera and the controllers.

After that came the main mechanic: the knives in the hands, the slicing, the ingredients split in halves and the system that throws them in arcs from behind the counter.

Then the game loop was added: the menu, Zeff's introduction, the countdown, the timer, the score and the end screen. Finally it got the haptics, the sounds, the music, the sea and sky outside the windows, and it was tested with the headset through Air Link and with the APK.

## Problems encountered and solutions applied

- The knives were held like a pistol and didn't fit the controller. Their position and angle were adjusted until the handle matched the controller's handle.
- When testing through Air Link the camera started at floor height, so the game now forces the height to start from the real floor.
- After closing the game from the headset menu, Air Link kept it loading forever. It was solved by restarting Unity, and since then tests are always stopped from Unity.
- The menus were all in the same place and a hidden one could block the ray of the visible one, so each menu now turns its ray off when it is hidden.
- Some sounds were much louder or quieter than others, so the volumes were balanced and the background sounds go down during the round.
- Some fruit halves didn't match the whole fruit, so they were resized or left out of the game.

## Contribution of each team member

| Member | Tasks carried out |
| --- | --- |
| Mario | Kitchen scene and 3D models, game loop, menus and Zeff's dialogue |
| Dídac | Project settings, slicing, haptics, sound and music, view outside the windows |
| Ton | Knives in the hands, ingredient launcher and tracking fixes |

## How to run

- On Quest 3: install the APK on the headset (developer mode on) and open Baratie Blade from the apps under unknown sources.
- In the editor: open the Baratie_Kitchen scene, connect the headset with Quest Link or Air Link and press Play. Stop the test from Unity, not from the headset menu.

## Credits

- 3D models: Food Kit, Furniture Kit and Pirate Kit by Kenney ([www.kenney.nl](https://www.kenney.nl)), free to use.
- Haptic clip: Meta Haptics SDK samples.
- One Piece characters belong to Eiichiro Oda and Toei Animation. Zeff's image is from One Piece Bounty Rush.
