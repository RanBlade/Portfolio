MarketMaker:

Ok so this project was suppose to be my big break into the software development field. 

We were making a game to be released across all mobile devices and all browsers. I can't go into too many details on what the game did.

The tech we decided to go with for the front end was socket.io and node.js. I wrote all the socket.io code on the client and delivered in a format for the client programmer to be able to create the webpage without any knowlege of the network infastructure going on. After that I wrote a node.js/socket.io server to act as a middle man server between the client browser and the backend game server. This middle man server took messages from the client through a socket.io connection and then (using node.js) created a socket connection to a backend server. The backend game server was written completly in C++ and handledd all processing.



Disclaimer: This was a prototype version 1 to prove teh concept could work. It did work and it worked well but it was thrown together as fast as possible so there will yet again be very little commenting and very rushed code. This was all going to be re-written if we had decided to move forward with the game.
