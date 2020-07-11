# Publish-Subscribe Broker
### Created by Logan Giese
A relatively simple implementation of a publish-subscribe system in C# over TCP, made during Paycom's Summer Engagement Program 2020.

----
## Features
Publishers can:
* Create a new topic to publish to
* Publish a message to a topic

Subscribers can:
* Request and receive a list of active topics
* Subscribe to or unsubscribe from a topic
* Receive messages published to subscribed topics
