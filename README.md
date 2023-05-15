# project-petrichor

Demo video: https://drive.google.com/file/d/1cU_KYdZr0YrRUFbrof1-jfzTVT6XVptj/view?usp=sharing

All art and sound effects are created by Me.
For other sources, see the README file in "/Project Petrichor/Assets"

The "Project Petrichor" folder is the actual folder for the unity project. 

How to play:
* Left-click to attack in the direction of the mouse
* Right-click or Left shift to parry in the direction of the mouse
* Space to dash in a direction
* WASD to move

* Your meter is constantly filling and performing any actions or taking damage increase the meter further.
* Once your meter reaches full, you die.
* Killing enemies, landing successful attacks, and landing successful parries all deplete your meter.

List of features:
* Animation based, 4 directional attack system (you attack in the direction of your mouse)
* Direction based parrying system (you must parry in the direction of incoming attacks)
* Animation cancelling (animations can be interrupted with new actions before the previous action is "done")
* Input buffering (some actions can be queued before you are able to animation cancel)
* Player-centered enemy spawning (enemies will spawn at a random point centered from the player, making sure not to spawn in walls)
* Finite State Machine based logic
* Desire-vector based enemy movement (enemies will try to approach a target, while avoiding obstacles and each other. The desire vector can be weighted to  change the behavior of enemy movement {you can make enemies flee from the player by giving a weight of -1} ).
* Auto-timed music (once the music stops playing, the game will wait for a random amount of time before playing the next track to give your ears a break)
* A star pathfinding
