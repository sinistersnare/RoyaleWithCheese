# Royale With Cheese #

A game.... That has robots? Yes! Robots!

Robot fighting/challenge game.

See video here for trailer: https://www.youtube.com/watch?v=ByWkJWwgsco

## Credits ##

All game mechanics and features were written solely by me, Davis Silverman. No otherstudent helped in any way regarding working on this game. Except of course, for these external resources:

## External Resources ##

* `XboxCtrlrInput` library was used for cross-platform Xbox controller support.
    * If you are testing on a Mac, install this [360Controller](https://github.com/360Controller/360Controller/releases)
* `NavMeshComponents` for creating nav meshes at runtime.
* Unity3D was used for the engine.
* Music was provided royalty free by Incompetech, Kevin Macleod.
* Models and animations were procured from Mixamo.
* Pictures of Xbox 360 controller parts were not made by me, I downloaded them from the internet.

***Xbox 360 controller required to play***

## Short Description ##

Royale with Cheese (stylized with a capital ‘W’) is a 3rd-person robot-fighting
action game. In challenge mode, the player races to find 5 pink pickups for a
high-score. In battle bode, two robots face off to look for the pickups.
Weapons are enabled, and destroying a robot resets their score.
First to 5 wins, but for each pickup gained, the player slows,
making for easy dispatch.

Weapons include two different guns, which need to be picked up to use.
The standard gun will shoot small and fast bullets, which can easily target
the enemy. The BIG fires large projectiles, traveling slowly. On impact,
BIG explodes, damaging all robots in a small radius. Pods are locked onto
the target robot, and seek them out throughout the battlefield.
The bomb is an arced projectile, doing massive damage on hit,
with heavy splash damage.

## Gameplay Description ##

Please note that 1 Xbox controller is required for challenge mode,
and 2 for battlemode.

The battle mode is the most fleshed out part of the game.
Two players are locked onto each-other, in pursuit of 5 pink items.
If a player collects 5 without dying, then they win.
For each pickup gained, the player is slowed down, making them an easier
target for more powerful attacks. Guns have to be picked up to use,
so find one before your enemy does! The two gun types are typical
slow-but-strong and fast-but-weak. They are both useful strategically,
depending on enemy progression. When a player dies, they reset to a score of 0,
and must now attempt to gain the advantage once more.

Challenge Mode is for the high-score junkies. The same level is provided as you
would find in battle mode, but with weapons disabled.
The player must dash through the levellooking for the five items. The time ticks
upwards, and when all five are found, the game reports the time taken,
and returns to the main menu.

Level Generation uses perlin noise to create levels. Unity does not natively
support creating a navigation mesh at runtime, but there is a new library
called NavMeshComponents that I used to fashion the level with a navmesh on demand.
Items are added to the level at start-time, and more are added as time goes by.

The players are robots, with dashing capabilities. Jump, and then jump again to dash
in the direction the player is moving. The pods use the nav mesh created at run time
to find the enemy. If a path can not be constructed, the pod will die, and a new pod
can be spawned immediately. Aside from this case, all weapons have a cool-down timer,
which can be seen from the player’s side of the screen. The bomb is a simple arc from
the player’s position at launch time to the target’s position at launch time.
This makes the bomb a good weapon to use when the enemy is heavily slowed by items.

There is also a menu system in the game, hit the ‘start’ button on your Xbox controller
to open the pause menu, where the player can return to the main menu.
On the main menu, the credits and controls share a sub-screen, and the game modes
can be accessed simply by using the Xbox controllers letter-buttons (A,B,X,Y).
