Erik  [8:46 AM]
hm, maybe OutboxHostedService:Enabled should also not write - or maybe another setting is justified (in case still want to write rows, but not publish for a while)
Cort  [8:49 AM]
there are a few things going on there -- one is the message publisher, which can be to an outbox or to a broker -- the forwarding publisher that is the outbox publisher that will take queued messages and async to the message publisher publish them to a broker -- and then accepting messages from a broker
[8:49 AM]if you want to just "ignore" published events, then i would replace the message publisher in the ioc chain

Cort  [11:26 AM]
shit!  i don't remember what the issue was here now and it's now older than the free version will let me see
Braden Edmunds  [12:58 PM]
I have no idea.... lol
Erik  [1:00 PM]
more upgrade hooks.... was it something like items vs results in paginated rest responses?
Cort  [1:00 PM]
that's it!
[1:00 PM]thanks
Troy Horton  [1:04 PM]
ask ai
Cort  [1:08 PM]
good idea!
