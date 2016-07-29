# Mario World 1-1 Clone
![alt text](https://github.com/Mszauer/Mario-World1-1/blob/master/Screenshots/Mario1.PNG "Start Screen")
![alt text](https://github.com/Mszauer/Mario-World1-1/blob/master/Screenshots/Mario2.PNG "In-Game Screen")
![alt text](https://github.com/Mszauer/Mario-World1-1/blob/master/Screenshots/Mario3.PNG "In-Game Screen")

##Purpose
Having just finished learning about OpenTK and Tile-based games, I tried to recreate a very iconic simple game. I had just written the managers that handle input, display sprites on screen, and play audio so it was time to put them to use in an actual game. Mario is a very old game game with simple logic, perfect to try and recreate for learning!

##Learning Objectives
###Planning
There wasn't too much planning for this project as Mario is an old enough game that everything can be found online and are free to use. Majority of my planning went into how to create the map, have it load, and make it so that you could load multiple levels.

###Inheritence
This was one of my first projects where inheritence was introduced. I tried to figure out commonalities between the objects that needed to be displayed and have their own class. Ultimately it boiled down to three things: Mario, Enemies, Items. This definitely helped keep things more managable mentally for myself and kept my files a lot smaller.

###Data Serialization
This was my first time using data serialization, and it was a brand new concept. This reinforced a lot of the Datastructures that I learned not too long before this as I had to store variables multiple ways and then have them transfer over to in-game.

###Debugging / QA
I ran into a few tunneling issues that were brand new, as this was one of my first tile-based games created. Tying sounds to actions went very smoothly and had minimal bugs.
