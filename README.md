# Virtual-Reality-for-Controlling-and-Interacting-with-Robots
Year 3 Dissertation Project Swansea University

This project is a modification of the project by Josh Wott which can be found at https://github.com/Joshwott/Virtual-Reality-for-Controlling-and-Interacting-with-Robots

Follow the instructions in Josh's repository to set up the project.

This project implements a use case scenario which is of a firefighting scene.

NOTE: If using the Choregraphe simulation instead of a physical NAO robot the connection instructions differ slightly.

1. To find its port head to Edit > Preferences > Virtual Robot. 
	
2. Enter this port into the config.py file in the python folder and enter this port within the Unity project in Assets > RobotPort.cs

3. Now Enter local host IP "127.0.0.1" into the IP address field when in Unity playmode then click "Connect"

The firefighting scene also uses some assets from the unity store:

UBS Country House #2 - Construction Set H33 by Universal Building System at: https://assetstore.unity.com/packages/3d/environments/ubs-country-house-2-construction-set-h33-131539

Water 5 FX Particle Pack by Stars Products at: https://assetstore.unity.com/packages/vfx/particles/environment/water-5-fx-particle-pack-192211

Fire & Spell Effects by Digital Ruby (Jeff Johnson) at: https://assetstore.unity.com/packages/vfx/particles/fire-explosions/fire-spell-effects-36825

The project by Josh Wott is a modified version of PePUT by Ganal et al which can be found at: https://gitlab2.informatik.uni-wuerzburg.de/mi-development/peput