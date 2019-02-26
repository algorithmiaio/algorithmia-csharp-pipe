# algorithmia-c-sharp-dev
A package enabling deployment of C# algorithms on Algorithmia.


# What this is
This package takes an `Algorithmia` algorithm, and wraps it in a language agnostic `pipe` format.

When executed with an algorithm:
* The module will listen to incoming requests on `stdin`
* When a request is recieved, it will pass that json message to the algorithm's apply function
* After a response is created, we send it to a 