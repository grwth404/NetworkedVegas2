# Networked Vegas 2

A command line application to help connect Rainbow Six Vegas 2 across the internet.

## Installation

1. [Download the latest release](https://github.com/Turnerj/NetworkedVegas2/releases/latest) on the machine that is the game client.
2. Open your preferred terminal and run the application, specifying the IP address of the game server.
3. In the game itself, navigate to LAN where it will search for servers. Your game server should now appear.

## Frequently Asked Questions

### Why would I need this?

Some games, like Rainbow Six Vegas 2, don't have an option to directly connect to a server by specifying an IP address.
Instead, they work by either broadcasting out to the local network looking for servers or servers broadcast their game to any clients listening.

Rainbow Six Vegas 2 does the former, where the game client sends a request to the broadcast IP (255.255.255.255).
If however the server isn't in the local network, it won't receive this.
This behaviour would be fine except if you want to play a LAN game over the internet.

Tools like [Tailscale](https://tailscale.com/) help a lot with the networking side of things but there were still issues getting the client's request to the server - that's where Networked Vegas 2 helps.

### How does it work?

When you run Networked Vegas 2, it will effectively proxy the initial handshake between client and server.
The broadcast request that was being troublesome is instead intercepted and forwarded to a specific IP address directly.
The server's response for that request is then forwarded back to the client.

This is where Networked Vegas 2 ends and the game's net code takes full control.
Effectively, all we are doing is tricking the game to make it think the server is local when it isn't.

### Does this work for other games?

Maybe for other Unreal Engine 3 games? It depends a lot on the networking system for that specific game.
I have only tested Rainbow Six Vegas 2 and don't really have any intention of supporting other games.

### It doesn't work!

Probably the most important thing to know is that Networked Vegas 2 isn't a VPN or tunneling piece of software.
If the client machine and the server can't actually connect to each other via sending a PING back and forth, there is nothing Networked Vegas 2 can do to help.

If the client and server can otherwise reach each other on the network, and there are no firewall restrictions, it could be to do with different patch versions of the game.

Beyond that, I can't really help you - debugging the nuances of individual setups gets complicated.