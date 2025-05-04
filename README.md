# Virtual-Reality-for-Controlling-and-Interacting-with-Robots
Year 3 Dissertation Project Swansea University

This project is a modification of the project by Josh Wott which can be found at https://github.com/Joshwott/Virtual-Reality-for-Controlling-and-Interacting-with-Robots

Follow the instructions in Josh's repository to set up the project.

This project implements a use case scenario which is of a firefighting scene.

NOTE: If using the Choregraphe simulation instead of a physical NAO robot the connection instructions differ slightly.
	1. To find its port we head to Edit > Preferences > Virtual Robot. 
	
	2. Enter this port into the config.py file in the python folder and enter this port within the Unity project in Assets > RobotPort.cs

	3. Now Enter local host IP "127.0.0.1" into the IP address field when in Unity playmode then click "Connect"