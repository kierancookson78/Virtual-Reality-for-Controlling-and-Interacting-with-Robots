# Virtual-Reality-for-Controlling-and-Interacting-with-Robots
Year 3 Dissertation Project Swansea University

Project Set Up Instructions:

1. Install Python 2.7 this can be found following this link:
       https://www.python.org/downloads/release/python-2718/

2. Install naoqi for python and ensure the framework is within your system path, instructons can be found on aldebarans website linked below:
       http://doc.aldebaran.com/2-5/dev/python/install_guide.html

3. In the cmd set the ALmath version to 1.6.8 the downloaded version with naoqi should be 1.8.3. Use this command:
       python -m pip install almath==1.6.8

4. Connect the NAO to the system using either WI-FI or ethernet, then press the button on his chest once to get his IP address

5. Clone this git reposity (simple folder name recommended)

7. Open the python folder in the chosen IDE (Pycharm recommended)

8. To test the NAO connection run the TestConnection.py file

9. Once connection is verified run the Test.py file to start the python server

10. Open the Unity project in Unity ver "2019.1.14f" (the unity project must be in this version)

11. Once Unity is open within the assets select NAO VR folder then select the Scenes folder and open the scene

12. Put the Meta Quest 2 headset on holding both controllers then connect to oculus link either via an Oculus link cable or Air link

13. Once connected your user should be in a white room. Once in that room enter the Unity play mode on the computer

14. When in the playmode you should see a virtual mock up of the robotics lab the robot was developed in, on the computer, type in the NAO ip address and press the connect button (ensure the button is deselcted after pressing)

15. If successfully conencted NAO should say 'Verbunden' once this happens he is ready to be controlled, the control instructions can be found on the whitebaord behind the users initial spawn location.


NOTE: This project uses a heavily modified version of the original PePut toolkit as well as original content and is intended for use with the NAO robot instead of the Pepper robot. 
      Full credit for the original version of the PePut toolkit is given to Elizabeth Ganal et al which can be found here: https://gitlab2.informatik.uni-wuerzburg.de/mi-development/peput.
      Enjoy!
