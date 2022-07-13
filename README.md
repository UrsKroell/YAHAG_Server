# YAHAG_Server
#### Server for planned game "YAHAG (Yet another hacking game)"

## Techstack
 * Rest API for status information
 * EntityFramework and MariaDB for Database and Database Access
 * SeriLog as logging interface
 * [Toemsel/Network](https://github.com/Toemsel/Network) as network stack

## Annotations
+ It should already work as service on UNIX systems (more checking needed)

## TODO
+ [ ] SeriLog server setup
+ [ ] Dockerize
  + partly done on solution level

+ API
  + [ ] Admin interface for remote administration
  + [ ] Display server status (connected clients, etc.)
  + [ ] Display world status (count of (running) systems, count of active npcs, etc.)
  + [ ] Endpoint for user status

+ Scripting
  + [ ] Implement server side implementation of ingame scripting functions
  + [ ] Sandboxing all scripts which might run server side

+ World
  + [ ] Serializable list of NPC systems
  + [ ] NPC systems
    + [ ] Gov systems
    + [ ] private companies
    + [ ] mail providers
    + [ ] System to dynamically create "vulnerabilites" in these systems
    + [ ] Stock Exchange and auctions sites
  + [ ] Vulnerability system
    + Listing like CVE
    + Has to be dynamic
    + Going by OWASP Top 10 for the beginning
  
+ Mission system
  + [ ] Static mission creator for story missions
  + [ ] Dynamic mission creator for fluff content
  + [ ] Ability for players to create and advertise custom missions
    + [ ] mission complete and fail states