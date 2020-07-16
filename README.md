# Publish-Subscribe Broker
### Created by Logan Giese
An implementation of a publish-subscribe system in C# over TCP, made during Paycom's Summer Engagement Program 2020.

## Features
Publishers can:
* Create a new topic to publish to
* Publish a message to a topic

Subscribers can:
* Request and receive a list of active topics
* Subscribe to or unsubscribe from a topic
* Automatically receive messages published to subscribed topics

## About the Test Application
Upon starting the test application (in Program.cs), five other console windows should appear.  Three are publisher clients, which will each create a topic and publish random string data to it.  The other two are subscriber clients, which will each request a list of topics and subscribe to two topics (covering all three topics between the two of them).

Every message published to a topic will be sent automatically to the topic's subscriber(s).  You can watch the exchange in real time, or check over the console output later to verify correctness.

## About the Project
The Networking package provides flexible methods to send serialized objects reliably over TCP, and also provides client and server classes that can be easily extended for custom protocols.

The PubSubProtocol package includes an implementation of a publish-subscribe protocol with the broker (server) side as well as the publisher/subscriber (client) side.  It was designed to be moderately robust and flexible, using a request-response pattern and an asynchonous/thread-safe structure.  Published messages are delivered to subscribers quickly and automatically.

NetworkingTest.cs contains a test application for the example protocol provided in the Networking package, where user input is sent to a server.

Program.cs contains a test application for the publish-subscribe protocol provided in the PubSubProtocol package (described above).

## Additional Information
This project was created as part of Paycom's Summer Engagement Program 2020, to obtain feedback from industry professionals in the software engineering field.  The engagement program was intended as a partial replacement for their internship program that was cancelled due to the COVID-19 pandemic.
